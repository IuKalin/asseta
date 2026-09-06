using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Asseta.Api.Controllers;
using Asseta.Api.Models;
using Asseta.Application.Features.ContinuityPlan.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Asseta.IntegrationTests;

public class ContinuityPlanApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ContinuityPlanApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetContinuityPlan_Returns200_With4StagesAndCalculatedMetrics()
    {
        var ownerId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
            var cat = new ContinuityCategory(Guid.NewGuid(), "FINANCE_" + Guid.NewGuid().ToString("N")[..6], "Tài chính", "Finance", "wallet", 1);
            db.ContinuityCategories.Add(cat);

            var tp = new TrustedPerson("Delegate Test", "del@api.com", "0911223344", "Vợ", 2, ownerId);
            db.TrustedPeople.Add(tp);

            var card = new ActionCard(Guid.NewGuid(), ownerId, cat.Id, "Thẻ khẩn cấp 24h", UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL, assignedTrustedPersonId: tp.Id, documentLocationHint: "Két sắt");
            db.ActionCards.Add(card);
            await db.SaveChangesAsync();
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/continuity-plan");
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ContinuityPlanDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.Equal(ownerId, envelope.Data.OwnerId);
        Assert.Equal(1, envelope.Data.TotalCardsCount);
        Assert.Equal(4, envelope.Data.Stages.Count);

        var immediateStage = envelope.Data.Stages.First(s => s.Stage == UrgencyStage.IMMEDIATE);
        Assert.Single(immediateStage.Cards);
        Assert.Equal("Thẻ khẩn cấp 24h", immediateStage.Cards[0].Title);
        Assert.Equal("Delegate Test (Vợ)", immediateStage.Cards[0].AssignedTrustedPersonName);
    }

    [Fact]
    public async Task UpdateStage_Returns200_UpdatesUrgencyAndIncrementsRowVersion()
    {
        var ownerId = Guid.NewGuid();
        Guid cardId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
            var cat = new ContinuityCategory(Guid.NewGuid(), "CAT_" + Guid.NewGuid().ToString("N")[..6], "Doanh nghiệp", "Biz", "briefcase", 1);
            db.ContinuityCategories.Add(cat);

            var card = new ActionCard(Guid.NewGuid(), ownerId, cat.Id, "Bàn giao tài khoản", UrgencyStage.FIRST_72_HOURS, PriorityLevel.IMPORTANT);
            db.ActionCards.Add(card);
            await db.SaveChangesAsync();
            cardId = card.Id;
        }

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/continuity-plan/cards/{cardId}/stage")
        {
            Content = JsonContent.Create(new UpdateStageRequest(UrgencyStage.IMMEDIATE, 1))
        };
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ContinuityPlanCardItemDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.Equal(UrgencyStage.IMMEDIATE, envelope.Data.Urgency);
        Assert.Equal(2, envelope.Data.RowVersion);
    }

    [Fact]
    public async Task ToggleCompletion_Returns200_TogglesIsCompleted()
    {
        var ownerId = Guid.NewGuid();
        Guid cardId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
            var cat = new ContinuityCategory(Guid.NewGuid(), "CAT_" + Guid.NewGuid().ToString("N")[..6], "Hồ sơ", "Docs", "file", 1);
            db.ContinuityCategories.Add(cat);

            var card = new ActionCard(Guid.NewGuid(), ownerId, cat.Id, "Kiểm tra hợp đồng", UrgencyStage.FIRST_7_DAYS, PriorityLevel.LOW);
            db.ActionCards.Add(card);
            await db.SaveChangesAsync();
            cardId = card.Id;
        }

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/continuity-plan/cards/{cardId}/toggle-completion")
        {
            Content = JsonContent.Create(new ToggleCompletionRequest(1))
        };
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ContinuityPlanCardItemDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.True(envelope.Data.IsCompleted);
        Assert.Equal(2, envelope.Data.RowVersion);
    }

    [Fact]
    public async Task GetEmergencyBrief_Returns200_ZeroKnowledgeSummary()
    {
        var ownerId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
            var cat = new ContinuityCategory(Guid.NewGuid(), "CAT_" + Guid.NewGuid().ToString("N")[..6], "Gia đình", "Fam", "heart", 1);
            db.ContinuityCategories.Add(cat);

            var tp = new TrustedPerson("Người thân A", "a@brief.com", "0933445566", "Bố", 2, ownerId);
            db.TrustedPeople.Add(tp);

            var card = new ActionCard(Guid.NewGuid(), ownerId, cat.Id, "Khẩn cấp y tế", UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL, assignedTrustedPersonId: tp.Id, documentLocationHint: "Ngăn kéo bàn");
            card.AddStep("Gọi bác sĩ gia đình", "15 phút");
            card.AddContact("Bác sĩ Tuấn", "Bác sĩ", "0900112233");

            db.ActionCards.Add(card);
            await db.SaveChangesAsync();
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/continuity-plan/emergency-brief");
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<OfflineEmergencyBriefDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.NotEmpty(envelope.Data.PrimaryContacts);
        Assert.Equal(4, envelope.Data.Stages.Count);

        var immediateStage = envelope.Data.Stages.First(s => s.Stage == UrgencyStage.IMMEDIATE);
        Assert.Single(immediateStage.ActionItems);
        Assert.Equal("Khẩn cấp y tế", immediateStage.ActionItems[0].Title);
        Assert.Single(immediateStage.ActionItems[0].KeySteps);
        Assert.Single(immediateStage.ActionItems[0].KeyContacts);
    }

    [Fact]
    public async Task GetAudit_Returns200_DiagnosesGapsAndRecommendations()
    {
        var ownerId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
            var cat = new ContinuityCategory(Guid.NewGuid(), "CAT_" + Guid.NewGuid().ToString("N")[..6], "Tài chính", "Fin", "wallet", 1);
            db.ContinuityCategories.Add(cat);

            var card = new ActionCard(Guid.NewGuid(), ownerId, cat.Id, "Thẻ chưa có người", UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL);
            db.ActionCards.Add(card);
            await db.SaveChangesAsync();
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/continuity-plan/audit");
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<PlanReadinessAuditDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.Single(envelope.Data.IdentifiedGaps);
        Assert.NotEmpty(envelope.Data.ActionableRecommendations);
    }
}
