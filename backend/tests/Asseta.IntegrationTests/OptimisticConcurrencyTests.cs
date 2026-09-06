using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Asseta.Api.Controllers;
using Asseta.Api.Models;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Constants;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Asseta.IntegrationTests;

public class OptimisticConcurrencyTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testOwnerId = Guid.NewGuid();

    public OptimisticConcurrencyTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateItem_WithStaleRowVersion_Returns409Conflict()
    {
        // 1. Get Category
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var categoryId = db.ContinuityCategories.First(c => c.Code == CategoryCodes.Financial).Id;

        // 2. Create Item
        var createPayload = new CreateContinuityItemRequest(
            categoryId,
            "Tài khoản tiết kiệm MB Bank",
            PriorityLevel.IMPORTANT,
            "Ngăn tủ phòng ngủ");

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

        // 3. Client A updates item with rowVersion = 1 -> succeeds, version becomes 2
        var updatePayloadA = new UpdateContinuityItemRequest(
            "Tài khoản tiết kiệm MB Bank - Đã cập nhật bởi A",
            PriorityLevel.IMPORTANT,
            "Ngăn tủ phòng ngủ",
            null,
            null,
            null,
            null,
            RowVersion: 1);

        var updateReqA = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/continuity-items/{itemId}")
        {
            Content = JsonContent.Create(updatePayloadA)
        };
        updateReqA.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var updateResA = await _client.SendAsync(updateReqA);
        Assert.Equal(HttpStatusCode.OK, updateResA.StatusCode);

        var updatedItemA = await updateResA.Content.ReadFromJsonAsync<ApiResponse<ContinuityItemDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(updatedItemA?.Data);
        Assert.Equal(2, updatedItemA.Data.RowVersion);

        // 4. Client B attempts to update item with stale rowVersion = 1 -> must fail with 409 Conflict
        var updatePayloadB = new UpdateContinuityItemRequest(
            "Tài khoản tiết kiệm MB Bank - Đã cập nhật bởi B (Stale)",
            PriorityLevel.IMPORTANT,
            "Ngăn tủ phòng làm việc",
            null,
            null,
            null,
            null,
            RowVersion: 1); // Stale version

        var updateReqB = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/continuity-items/{itemId}")
        {
            Content = JsonContent.Create(updatePayloadB)
        };
        updateReqB.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var updateResB = await _client.SendAsync(updateReqB);

        // Assert 409 Conflict
        Assert.Equal(HttpStatusCode.Conflict, updateResB.StatusCode);

        var errorEnvelope = await updateResB.Content.ReadFromJsonAsync<ApiResponse<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(errorEnvelope);
        Assert.False(errorEnvelope.Success);
        Assert.Equal("CONCURRENT_STATE_MUTATION", errorEnvelope.Error?.Code);
    }
}
