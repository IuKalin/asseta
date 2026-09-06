using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Asseta.Api.Controllers;
using Asseta.Api.Models;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Constants;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Asseta.IntegrationTests;

public class ZeroKnowledgeVerificationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testOwnerId = Guid.NewGuid();

    public ZeroKnowledgeVerificationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ZeroKnowledge_DatabaseAudit_ConfidentialPlaintextNeverStoredInServerOrLogs()
    {
        // Arrange
        const string secretNotesPlaintext = "Hồ sơ thế chấp vay vốn 2 tỷ gửi anh Hoàng kế toán giữ bản cứng tại tầng 2";
        const string mockCipherBlob = "U2FsdGVkX1+MockEncryptedPayloadZeroKnowledgeProof==";
        const string mockNonce = "96BitNonceBase64==";
        const string mockAuthTag = "128BitAuthTagBase64==";

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var category = await db.ContinuityCategories.FirstAsync(c => c.Code == CategoryCodes.Financial);

        var createPayload = new CreateContinuityItemRequest(
            category.Id,
            "Khoản vay thế chấp kinh doanh",
            PriorityLevel.CRITICAL,
            "Ngăn tủ tài liệu phòng khách",
            null,
            mockCipherBlob,
            mockNonce,
            mockAuthTag);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(createPayload)
        };
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        // Act
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ContinuityItemDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(envelope?.Data);
        var createdItemId = envelope.Data.Id;

        // Assert 1: Query database entity directly
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var dbItem = await verifyDb.ContinuityItems
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Id == createdItemId);

        Assert.NotNull(dbItem);

        // Assert 2: Database only contains cipher blob, nonce, and auth tag
        Assert.Equal(mockCipherBlob, dbItem.CipherNotesBlob);
        Assert.Equal(mockNonce, dbItem.CipherNonce);
        Assert.Equal(mockAuthTag, dbItem.CipherAuthTag);

        // Assert 3: Database columns NEVER contain the raw plaintext
        Assert.DoesNotContain(secretNotesPlaintext, dbItem.Name);
        Assert.DoesNotContain(secretNotesPlaintext, dbItem.DocumentLocationHint ?? "");
        Assert.DoesNotContain(secretNotesPlaintext, dbItem.CipherNotesBlob);

        // Assert 4: Audit logs do NOT capture the confidential notes
        var auditLogs = await verifyDb.ContinuityAuditLogs
            .Where(a => a.ItemId == createdItemId)
            .ToListAsync();

        Assert.NotEmpty(auditLogs);
        foreach (var log in auditLogs)
        {
            Assert.DoesNotContain(secretNotesPlaintext, log.PayloadSnapshot);
            Assert.DoesNotContain(mockCipherBlob, log.PayloadSnapshot); // Even cipher blob is not in snapshot
        }
    }
}
