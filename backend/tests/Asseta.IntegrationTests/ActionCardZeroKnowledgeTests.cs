using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Asseta.Api.Controllers;
using Asseta.Api.Models;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Constants;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Asseta.IntegrationTests;

public class ActionCardZeroKnowledgeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testOwnerId = Guid.NewGuid();
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ActionCardZeroKnowledgeTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ZeroKnowledge_DatabaseAudit_ActionCardConfidentialCipherOnly()
    {
        // Arrange
        const string secretInstructionsPlaintext = "Mật khẩu két sắt gia đình là 889900 và chìa khóa dự phòng nằm dưới chậu cây";
        const string mockCipherBlob = "U2FsdGVkX1+ActionCardEncryptedPayloadZeroKnowledgeProof==";
        const string mockNonce = "96BitCardNonceBase64==";
        const string mockAuthTag = "128BitCardAuthTagBase64==";

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var category = await db.ContinuityCategories.FirstAsync(c => c.Code == CategoryCodes.Financial);

        var payload = new CreateActionCardRequest(
            category.Id,
            "Thẻ xử lý tài chính khẩn cấp gia đình",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL,
            "Ghi chú thông thường không bảo mật",
            null,
            "Tủ phòng ngủ tầng 2",
            "https://cloud.asseta.vn/loan-contract.pdf",
            mockCipherBlob,
            mockNonce,
            mockAuthTag);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/action-cards")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        // Act
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ActionCardDto>>(JsonOpts);
        Assert.NotNull(envelope?.Data);
        var createdCardId = envelope.Data.Id;

        // Assert 1: Query database directly
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var dbCard = await verifyDb.ActionCards
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == createdCardId);

        Assert.NotNull(dbCard);

        // Assert 2: Database only stores ciphertext, nonce and auth tag
        Assert.NotNull(dbCard.CipherInstructions);
        Assert.Equal(mockCipherBlob, dbCard.CipherInstructions.CipherBlob);
        Assert.Equal(mockNonce, dbCard.CipherInstructions.Nonce);
        Assert.Equal(mockAuthTag, dbCard.CipherInstructions.AuthTag);

        // Assert 3: Database columns NEVER contain the raw plaintext
        Assert.DoesNotContain(secretInstructionsPlaintext, dbCard.Title);
        Assert.DoesNotContain(secretInstructionsPlaintext, dbCard.Summary ?? "");
        Assert.DoesNotContain(secretInstructionsPlaintext, dbCard.DocumentLocationHint ?? "");
        Assert.DoesNotContain(secretInstructionsPlaintext, dbCard.CipherInstructions.CipherBlob);

        // Assert 4: Audit logs do not contain raw plaintext
        var auditLogs = await verifyDb.ContinuityAuditLogs
            .Where(a => a.ItemId == createdCardId)
            .ToListAsync();

        foreach (var log in auditLogs)
        {
            Assert.DoesNotContain(secretInstructionsPlaintext, log.PayloadSnapshot);
            Assert.DoesNotContain(mockCipherBlob, log.PayloadSnapshot);
        }
    }

    [Theory]
    [InlineData("Khóa cá nhân ví: 0x4f3edf983ac636a65a842ce7c78d9aa706d3b113bce9c46f30d7d21715b23b1d")]
    [InlineData("Số thẻ thanh toán 4532 0150 1234 5678")]
    [InlineData("-----BEGIN RSA PRIVATE KEY----- MIIEowIBAAKCAQEA... -----END RSA PRIVATE KEY-----")]
    public async Task SensitiveDataInspector_UnencryptedSensitiveStrings_RejectsWith422(string unencryptedSensitiveString)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var category = await db.ContinuityCategories.FirstAsync(c => c.Code == CategoryCodes.Financial);

        var payload = new CreateActionCardRequest(
            category.Id,
            $"Thẻ nhạy cảm {unencryptedSensitiveString}",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/action-cards")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
}
