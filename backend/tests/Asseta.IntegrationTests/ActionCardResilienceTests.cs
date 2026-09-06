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

public class ActionCardResilienceTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testOwnerId = Guid.NewGuid();
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ActionCardResilienceTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Idempotency_StormRetries_CreatesExactlyOneActionCardInDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var category = await db.ContinuityCategories.FirstAsync(c => c.Code == CategoryCodes.Property);

        var idempotencyKey = Guid.NewGuid().ToString();
        var cardTitle = $"Thẻ bảo dưỡng bất động sản {Guid.NewGuid():N}";
        var payload = new CreateActionCardRequest(
            category.Id,
            cardTitle,
            UrgencyStage.FIRST_7_DAYS,
            PriorityLevel.IMPORTANT);

        // Act: Send 5 consecutive requests with identical Idempotency-Key
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 5; i++)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/v1/action-cards")
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

        // Verify database: Exactly 1 record created with this title
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var count = await verifyDb.ActionCards
            .CountAsync(c => c.OwnerId == _testOwnerId && c.Title == cardTitle);

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Concurrency_ParallelMutations_RejectsConflictingStaleVersions()
    {
        // 1. Create initial action card
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var category = await db.ContinuityCategories.FirstAsync(c => c.Code == CategoryCodes.Documents);

        var createPayload = new CreateActionCardRequest(
            category.Id,
            "Thẻ ủy quyền pháp lý kinh doanh",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL);

        var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/action-cards")
        {
            Content = JsonContent.Create(createPayload)
        };
        createReq.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var createRes = await _client.SendAsync(createReq);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);

        var createdCard = await createRes.Content.ReadFromJsonAsync<ApiResponse<ActionCardDto>>(JsonOpts);
        Assert.NotNull(createdCard?.Data);
        var cardId = createdCard.Data.Id;
        var initialRowVersion = createdCard.Data.RowVersion;
        Assert.Equal(1, initialRowVersion);

        // 2. Client A updates successfully with RowVersion = 1
        var updatePayloadA = new UpdateActionCardRequest(
            "Thẻ ủy quyền pháp lý - Cập nhật bởi Client A",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL,
            initialRowVersion,
            "Ghi chú bổ sung từ Client A",
            null,
            "Văn phòng công chứng số 1");

        var updateReqA = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/action-cards/{cardId}")
        {
            Content = JsonContent.Create(updatePayloadA)
        };
        updateReqA.Headers.Add("X-Owner-Id", _testOwnerId.ToString());

        var resA = await _client.SendAsync(updateReqA);
        Assert.Equal(HttpStatusCode.OK, resA.StatusCode);

        var updatedCardA = await resA.Content.ReadFromJsonAsync<ApiResponse<ActionCardDto>>(JsonOpts);
        Assert.NotNull(updatedCardA?.Data);
        Assert.Equal(2, updatedCardA.Data.RowVersion);

        // 3. Client B attempts update with stale RowVersion = 1 (must be rejected with 409 Conflict)
        var updatePayloadB = new UpdateActionCardRequest(
            "Thẻ ủy quyền pháp lý - Cập nhật bởi Client B (Xung đột)",
            UrgencyStage.LONGER_TERM,
            PriorityLevel.LOW,
            initialRowVersion); // Stale version 1

        var updateReqB = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/action-cards/{cardId}")
        {
            Content = JsonContent.Create(updatePayloadB)
        };
        updateReqB.Headers.Add("X-Owner-Id", _testOwnerId.ToString());

        var resB = await _client.SendAsync(updateReqB);
        Assert.Equal(HttpStatusCode.Conflict, resB.StatusCode);
    }
}
