using System.Net;
using System.Net.Http.Headers;
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

public class ContinuityMapApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testOwnerId = Guid.NewGuid();

    public ContinuityMapApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private Guid GetFirstCategoryId()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        return db.ContinuityCategories.First(c => c.Code == CategoryCodes.Financial).Id;
    }

    [Fact]
    public async Task GetContinuityMap_Returns200_WithEnvelopeStructure()
    {
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/continuity-map?ownerId={_testOwnerId}");
        request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ContinuityMapDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.Equal(6, envelope.Data.Categories.Count);
        Assert.NotNull(envelope.Meta.CorrelationId);
    }

    [Fact]
    public async Task CreateContinuityItem_WithValidData_Returns201_AndCreatesItem()
    {
        // Arrange
        var categoryId = GetFirstCategoryId();
        var payload = new CreateContinuityItemRequest(
            categoryId,
            "Sổ tiết kiệm Agribank",
            PriorityLevel.CRITICAL,
            "Két sắt gia đình",
            null,
            "U2FsdGVkX19dummyCipherBlob",
            "d1e2f3a4b5c6",
            "authTag12345");

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ContinuityItemDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.Equal("Sổ tiết kiệm Agribank", envelope.Data.Name);
        Assert.Equal(1, envelope.Data.RowVersion);
        Assert.True(envelope.Data.HasConfidentialNotes);
    }

    [Fact]
    public async Task CreateContinuityItem_WithEmptyName_Returns400_ValidationFailed()
    {
        // Arrange
        var categoryId = GetFirstCategoryId();
        var payload = new CreateContinuityItemRequest(
            categoryId,
            "",
            PriorityLevel.IMPORTANT);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.False(envelope.Success);
        Assert.Equal("VALIDATION_FAILED", envelope.Error?.Code);
    }

    [Fact]
    public async Task CreateContinuityItem_WithUnencryptedCreditCard_Returns422_SensitiveDataDetected()
    {
        // Arrange
        var categoryId = GetFirstCategoryId();
        var payload = new CreateContinuityItemRequest(
            categoryId,
            "Visa Card 4111 2222 3333 4444",
            PriorityLevel.CRITICAL);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.False(envelope.Success);
        Assert.Equal("SENSITIVE_DATA_DETECTED", envelope.Error?.Code);
    }

    [Fact]
    public async Task CreateContinuityItem_WithUnencryptedPrivateKey_Returns422_SensitiveDataDetected()
    {
        // Arrange
        var categoryId = GetFirstCategoryId();
        var payload = new CreateContinuityItemRequest(
            categoryId,
            "My Wallet",
            PriorityLevel.CRITICAL,
            "Private key: 0x4f3edf983ac636a65a842ce7c78d9aa706d3b113bce9c46f30d7d21715b23b1d");

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.False(envelope.Success);
        Assert.Equal("SENSITIVE_DATA_DETECTED", envelope.Error?.Code);
    }

    [Fact]
    public async Task GetItemById_WhenNonExistent_Returns404_NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/continuity-items/{nonExistentId}");
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.False(envelope.Success);
        Assert.Equal("CONTINUITY_ITEM_NOT_FOUND", envelope.Error?.Code);
    }

    [Fact]
    public async Task GetItemById_WhenDifferentOwner_Returns403_Forbidden()
    {
        // Arrange: create item by owner A
        var categoryId = GetFirstCategoryId();
        var createPayload = new CreateContinuityItemRequest(categoryId, "Bảo hiểm nhân thọ", PriorityLevel.IMPORTANT);
        var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(createPayload)
        };
        createReq.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var createRes = await _client.SendAsync(createReq);
        var created = await createRes.Content.ReadFromJsonAsync<ApiResponse<ContinuityItemDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var itemId = created!.Data!.Id;

        // Act: try to get item as owner B
        var otherOwnerId = Guid.NewGuid();
        var getReq = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/continuity-items/{itemId}");
        getReq.Headers.Add("X-Owner-Id", otherOwnerId.ToString());
        var response = await _client.SendAsync(getReq);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.False(envelope.Success);
        Assert.Equal("UNAUTHORIZED_RESOURCE_ACCESS", envelope.Error?.Code);
    }

    [Fact]
    public async Task DeleteItem_Returns200_AndSoftDeletesItem()
    {
        // Arrange: create item
        var categoryId = GetFirstCategoryId();
        var createPayload = new CreateContinuityItemRequest(categoryId, "Tài khoản chứng khoán", PriorityLevel.LOW);
        var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(createPayload)
        };
        createReq.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var createRes = await _client.SendAsync(createReq);
        var created = await createRes.Content.ReadFromJsonAsync<ApiResponse<ContinuityItemDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var itemId = created!.Data!.Id;

        // Act: delete item
        var deleteReq = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/continuity-items/{itemId}");
        deleteReq.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var deleteRes = await _client.SendAsync(deleteReq);

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteRes.StatusCode);

        // Verify it is not returned in GetById
        var getReq = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/continuity-items/{itemId}");
        getReq.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        var getRes = await _client.SendAsync(getReq);
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }

    [Fact]
    public async Task SubmitAssessment_Returns201_WithGeneratedItemsAndHistory()
    {
        // Arrange
        var answers = new List<AssessmentAnswerDto>
        {
            new("Q1", CategoryCodes.Financial, "Tài khoản ngân hàng", true, PriorityLevel.CRITICAL, "App Mobile"),
            new("Q2", CategoryCodes.Insurance, "Bảo hiểm sức khỏe", true, PriorityLevel.IMPORTANT, "Email cá nhân"),
            new("Q3", CategoryCodes.Property, "Nhà riêng", false)
        };
        var payload = new SubmitAssessmentRequest(answers, "v1");

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-map/assessment")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("X-Owner-Id", _testOwnerId.ToString());

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<AssessmentResultDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.Equal(2, envelope.Data.ItemsGeneratedCount);
    }

    [Fact]
    public async Task GetGaps_Returns200_WithGapsList()
    {
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/continuity-map/gaps?ownerId={_testOwnerId}");
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ContinuityGapDto>>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
    }

    [Fact]
    public async Task Idempotency_WhenSameKeySentTwice_ReturnsCachedResponse()
    {
        // Arrange
        var categoryId = GetFirstCategoryId();
        var payload = new CreateContinuityItemRequest(
            categoryId,
            "Sổ đỏ căn hộ Thủ Thiêm",
            PriorityLevel.CRITICAL);

        var idempotencyKey = Guid.NewGuid().ToString();

        var request1 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(payload)
        };
        request1.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        request1.Headers.Add("Idempotency-Key", idempotencyKey);

        // Act 1
        var response1 = await _client.SendAsync(request1);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        var content1 = await response1.Content.ReadAsStringAsync();

        // Act 2 with same Idempotency-Key
        var request2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/continuity-items")
        {
            Content = JsonContent.Create(payload)
        };
        request2.Headers.Add("X-Owner-Id", _testOwnerId.ToString());
        request2.Headers.Add("Idempotency-Key", idempotencyKey);

        var response2 = await _client.SendAsync(request2);

        // Assert 2
        Assert.True(response2.StatusCode == HttpStatusCode.Created || response2.StatusCode == HttpStatusCode.OK);
        var content2 = await response2.Content.ReadAsStringAsync();
        Assert.Equal(content1, content2);
    }
}
