using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Asseta.Api.Controllers;
using Asseta.Api.Models;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Constants;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Asseta.IntegrationTests;

public class ActionCardApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testOwnerId = Guid.NewGuid();
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ActionCardApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private Guid GetFinancialCategoryId()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        return db.ContinuityCategories.First(c => c.Code == CategoryCodes.Financial).Id;
    }

    [Fact]
    public async Task GetTemplates_Returns200_With6PreconfiguredTemplates()
    {
        var response = await _client.GetAsync("/api/v1/action-cards/templates");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ActionCardTemplateDto>>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.Equal(6, envelope.Data.Count);
        Assert.Contains(envelope.Data, t => t.TemplateCode == "TPL_BANK_LOAN");
        Assert.Contains(envelope.Data, t => t.TemplateCode == "TPL_LIFE_INSURANCE");
    }

    [Fact]
    public async Task CreateActionCard_WithValidData_Returns201()
    {
        var categoryId = GetFinancialCategoryId();
        var payload = new CreateActionCardRequest(
            categoryId,
            "Quản lý thẻ tín dụng Sacombank",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.IMPORTANT,
            "Cần thông báo vợ",
            Guid.NewGuid(),
            "Hộc bàn làm việc",
            "https://cloud.asseta.vn/file1");

        var response = await _client.PostAsJsonAsync("/api/v1/action-cards", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<ActionCardDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.Equal("Quản lý thẻ tín dụng Sacombank", envelope.Data.Title);
        Assert.Equal("FIRST_72_HOURS", envelope.Data.Urgency);
    }

    [Fact]
    public async Task CreateActionCard_WithCreditCardPattern_Returns422()
    {
        var categoryId = GetFinancialCategoryId();
        var payload = new CreateActionCardRequest(
            categoryId,
            "Thẻ visa 4532 0150 1234 5678", // Sensitive data
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL);

        var response = await _client.PostAsJsonAsync("/api/v1/action-cards", payload);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddStep_And_Reorder_WorksCorrectly()
    {
        var categoryId = GetFinancialCategoryId();
        var createPayload = new CreateActionCardRequest(
            categoryId,
            "Thẻ thao tác các bước",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL);

        var createRes = await _client.PostAsJsonAsync("/api/v1/action-cards", createPayload);
        var card = (await createRes.Content.ReadFromJsonAsync<ApiResponse<ActionCardDto>>(JsonOpts))!.Data!;

        // Add Step 1
        var step1Res = await _client.PostAsJsonAsync($"/api/v1/action-cards/{card.Id}/steps", new AddStepRequest("Bước 1: Gọi ngân hàng"));
        Assert.Equal(HttpStatusCode.Created, step1Res.StatusCode);
        var step1 = (await step1Res.Content.ReadFromJsonAsync<ApiResponse<ActionCardStepDto>>(JsonOpts))!.Data!;

        // Add Step 2
        var step2Res = await _client.PostAsJsonAsync($"/api/v1/action-cards/{card.Id}/steps", new AddStepRequest("Bước 2: Tìm hợp đồng"));
        Assert.Equal(HttpStatusCode.Created, step2Res.StatusCode);
        var step2 = (await step2Res.Content.ReadFromJsonAsync<ApiResponse<ActionCardStepDto>>(JsonOpts))!.Data!;

        // Reorder (reverse order: step2 then step1)
        var reorderRes = await _client.PostAsJsonAsync(
            $"/api/v1/action-cards/{card.Id}/steps/reorder",
            new ReorderStepsRequest(new List<Guid> { step2.Id, step1.Id }));

        Assert.Equal(HttpStatusCode.OK, reorderRes.StatusCode);
        var reordered = (await reorderRes.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ActionCardStepDto>>>(JsonOpts))!.Data!;

        Assert.Equal(step2.Id, reordered[0].Id);
        Assert.Equal(1, reordered[0].StepOrder);
        Assert.Equal(step1.Id, reordered[1].Id);
        Assert.Equal(2, reordered[1].StepOrder);
    }

    [Fact]
    public async Task AddContact_And_DeleteContact_WorksCorrectly()
    {
        var categoryId = GetFinancialCategoryId();
        var createPayload = new CreateActionCardRequest(
            categoryId,
            "Thẻ kiểm tra liên hệ",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.IMPORTANT);

        var createRes = await _client.PostAsJsonAsync("/api/v1/action-cards", createPayload);
        var card = (await createRes.Content.ReadFromJsonAsync<ApiResponse<ActionCardDto>>(JsonOpts))!.Data!;

        // Add Contact
        var contactRes = await _client.PostAsJsonAsync(
            $"/api/v1/action-cards/{card.Id}/contacts",
            new AddContactRequest("Nguyễn Văn A", "Luật sư riêng", "0912345678", "lawyer@example.com", "Gọi ngay"));

        Assert.Equal(HttpStatusCode.Created, contactRes.StatusCode);
        var contact = (await contactRes.Content.ReadFromJsonAsync<ApiResponse<ActionCardContactDto>>(JsonOpts))!.Data!;
        Assert.Equal("Nguyễn Văn A", contact.ContactName);

        // Delete Contact
        var deleteRes = await _client.DeleteAsync($"/api/v1/action-cards/{card.Id}/contacts/{contact.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteRes.StatusCode);
    }

    [Fact]
    public async Task CreateFromItem_WithTemplate_InheritsTemplateSteps()
    {
        var categoryId = GetFinancialCategoryId();

        // Create Continuity Item first
        var itemPayload = new CreateContinuityItemRequest(
            categoryId,
            "Khoản vay thế chấp mua nhà",
            PriorityLevel.CRITICAL,
            "Két sắt gia đình",
            Guid.NewGuid(),
            null, null, null);

        var itemRes = await _client.PostAsJsonAsync("/api/v1/continuity-items", itemPayload);
        var item = (await itemRes.Content.ReadFromJsonAsync<ApiResponse<ContinuityItemDto>>(JsonOpts))!.Data!;

        // Create Action Card from Item using TPL_BANK_LOAN
        var fromItemPayload = new CreateFromItemRequest(
            item.Id,
            "TPL_BANK_LOAN",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL);

        var fromItemRes = await _client.PostAsJsonAsync("/api/v1/action-cards/from-item", fromItemPayload);
        Assert.Equal(HttpStatusCode.Created, fromItemRes.StatusCode);

        var card = (await fromItemRes.Content.ReadFromJsonAsync<ApiResponse<ActionCardDto>>(JsonOpts))!.Data!;
        Assert.Equal(item.Id, card.ContinuityItemId);
        Assert.NotEmpty(card.Steps); // Has 3 steps from bank loan template
        Assert.NotEmpty(card.Contacts); // Has suggested roles
    }
}
