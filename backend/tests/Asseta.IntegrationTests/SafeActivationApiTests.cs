using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Asseta.Api.Controllers;
using Asseta.Api.Models;
using Asseta.Application.Features.SafeActivation.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Asseta.IntegrationTests;

public class SafeActivationApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public SafeActivationApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStatus_Returns200_WithDefaultConfig()
    {
        var ownerId = Guid.NewGuid();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/safe-activation/status");
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ActivationStatusDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.Equal(ownerId, envelope.Data.OwnerId);
        Assert.Equal(30, envelope.Data.CheckInIntervalDays);
        Assert.Equal(48, envelope.Data.GracePeriodHours);
        Assert.Equal("ACTIVE", envelope.Data.HeartbeatStatus);
    }

    [Fact]
    public async Task VitalityCheckIn_Returns200_AndRefreshesCheckIn()
    {
        var ownerId = Guid.NewGuid();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/safe-activation/check-in");
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ActivationStatusDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.Equal("ACTIVE", envelope.Data.HeartbeatStatus);
    }

    [Fact]
    public async Task UpdateConfig_ValidParameters_Returns200()
    {
        var ownerId = Guid.NewGuid();

        var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/safe-activation/config")
        {
            Content = JsonContent.Create(new UpdateConfigRequest(60, 72, 2))
        };
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ActivationConfigDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.Equal(60, envelope.Data.CheckInIntervalDays);
        Assert.Equal(72, envelope.Data.GracePeriodHours);
        Assert.Equal(2, envelope.Data.MinConfirmationsRequired);
    }

    [Fact]
    public async Task InitiateActivation_And_CancelByOwner_Flow()
    {
        var ownerId = Guid.NewGuid();
        var delegateUserId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
            var tp = new TrustedPerson("Luật sư A", "lawyer@test.com", "0900000001", "Luật sư", 2, ownerId, status: TrustedPersonStatus.Active);
            tp.MarkAsPaired(delegateUserId);
            db.TrustedPeople.Add(tp);
            await db.SaveChangesAsync();
        }

        // Delegate initiates activation request
        var initReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/safe-activation/requests")
        {
            Content = JsonContent.Create(new InitiateActivationApiRequest(ownerId, "Chủ tài sản đang phẫu thuật"))
        };
        initReq.Headers.Add("X-Owner-Id", delegateUserId.ToString());

        var initResponse = await _client.SendAsync(initReq);
        Assert.Equal(HttpStatusCode.Created, initResponse.StatusCode);

        var initEnvelope = await initResponse.Content.ReadFromJsonAsync<ApiResponse<ActivationRequestDto>>(JsonOpts);
        Assert.NotNull(initEnvelope);
        var requestId = initEnvelope.Data.Id;
        Assert.Equal("PendingGracePeriod", initEnvelope.Data.Status);

        // Owner 1-tap cancels the request
        var cancelReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/safe-activation/requests/{requestId}/cancel");
        cancelReq.Headers.Add("X-Owner-Id", ownerId.ToString());

        var cancelResponse = await _client.SendAsync(cancelReq);
        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

        // Verify status query shows Active and no active pending request
        var statusReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/safe-activation/status");
        statusReq.Headers.Add("X-Owner-Id", ownerId.ToString());

        var statusResponse = await _client.SendAsync(statusReq);
        var statusEnvelope = await statusResponse.Content.ReadFromJsonAsync<ApiResponse<ActivationStatusDto>>(JsonOpts);
        Assert.NotNull(statusEnvelope);
        Assert.Equal("ACTIVE", statusEnvelope.Data.HeartbeatStatus);
        Assert.Null(statusEnvelope.Data.ActiveRequest);
    }

    [Fact]
    public async Task InitiateActivation_Level1Delegate_Returns403Forbidden()
    {
        var ownerId = Guid.NewGuid();
        var level1UserId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
            var tp = new TrustedPerson("Người nhận tin", "notice@test.com", "0900000002", "Bạn", 1, ownerId, status: TrustedPersonStatus.Active);
            tp.MarkAsPaired(level1UserId);
            db.TrustedPeople.Add(tp);
            await db.SaveChangesAsync();
        }

        var initReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/safe-activation/requests")
        {
            Content = JsonContent.Create(new InitiateActivationApiRequest(ownerId, "Thử kích hoạt Level 1"))
        };
        initReq.Headers.Add("X-Owner-Id", level1UserId.ToString());

        var response = await _client.SendAsync(initReq);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
