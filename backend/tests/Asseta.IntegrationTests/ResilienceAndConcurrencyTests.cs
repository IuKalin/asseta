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

public class ResilienceAndConcurrencyTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testOwnerId = Guid.NewGuid();

    public ResilienceAndConcurrencyTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Idempotency_StormRetries_CreatesExactlyOneRecordInDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var category = await db.ContinuityCategories.FirstAsync(c => c.Code == CategoryCodes.Property);

        var idempotencyKey = Guid.NewGuid().ToString();
        var payload = new CreateContinuityItemRequest(
            category.Id,
            "Căn hộ cao cấp Masteri An Phú",
            PriorityLevel.CRITICAL,
            "Hồ sơ lưu tại két sắt");

        // Act: Send 5 consecutive requests with identical Idempotency-Key
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 5; i++)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
            {
                Content = JsonContent.Create(payload)
            };
            req.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
            req.Headers.Add("Idempotency-Key", idempotencyKey);

            var res = await _client.SendAsync(req);
            responses.Add(res);
        }

        // Assert: All responses must be successful
        foreach (var res in responses)
        {
            Assert.True(res.StatusCode == HttpStatusCode.Created || res.StatusCode == HttpStatusCode.OK);
        }

        // Verify database: Exactly 1 record created with this name for this owner
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var count = await verifyDb.ContinuityItems
            .CountAsync(i => i.OwnerId == _testOwnerId && i.Name == "Căn hộ cao cấp Masteri An Phú");

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Concurrency_ParallelMutations_RejectsConflictingStaleVersions()
    {
        // 1. Create initial item
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var category = await db.ContinuityCategories.FirstAsync(c => c.Code == CategoryCodes.Insurance);

        var createPayload = new CreateContinuityItemRequest(
            category.Id,
            "Bảo hiểm liên kết đầu tư AIA",
            PriorityLevel.IMPORTANT,
            "Email tài khoản AIA cá nhân");

        var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(createPayload)
        };
        createReq.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var createRes = await _client.SendAsync(createReq);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);

        var createdItem = await createRes.Content.ReadFromJsonAsync<ApiResponse<ContinuityItemDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(createdItem?.Data);
        var itemId = createdItem.Data.Id;
        Assert.Equal(1, createdItem.Data.RowVersion);

        // 2. Client 1 updates item with rowVersion = 1 -> succeeds
        var update1 = new UpdateContinuityItemRequest(
            "Bảo hiểm AIA - Sửa bởi Client 1",
            PriorityLevel.IMPORTANT,
            "Email tài khoản cá nhân",
            null,
            null,
            null,
            null,
            RowVersion: 1);

        var updateReq1 = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/continuity-items/{itemId}")
        {
            Content = JsonContent.Create(update1)
        };
        updateReq1.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var res1 = await _client.SendAsync(updateReq1);
        Assert.Equal(HttpStatusCode.OK, res1.StatusCode);

        // 3. Client 2 tries updating same item with rowVersion = 1 (stale) -> must be rejected with 409 Conflict
        var update2 = new UpdateContinuityItemRequest(
            "Bảo hiểm AIA - Sửa bởi Client 2 (Stale)",
            PriorityLevel.IMPORTANT,
            "Email tài khoản công ty",
            null,
            null,
            null,
            null,
            RowVersion: 1);

        var updateReq2 = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/continuity-items/{itemId}")
        {
            Content = JsonContent.Create(update2)
        };
        updateReq2.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var res2 = await _client.SendAsync(updateReq2);
        Assert.Equal(HttpStatusCode.Conflict, res2.StatusCode);
    }
}
