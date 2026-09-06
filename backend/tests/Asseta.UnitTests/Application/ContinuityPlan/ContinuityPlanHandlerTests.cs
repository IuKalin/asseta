using Asseta.Application.Common.Exceptions;
using Asseta.Application.Features.ContinuityPlan.Commands.ToggleActionCardCompletion;
using Asseta.Application.Features.ContinuityPlan.Commands.UpdateActionCardStage;
using Asseta.Application.Features.ContinuityPlan.Queries.GetContinuityPlan;
using Asseta.Application.Features.ContinuityPlan.Queries.GetMyDelegatedPlan;
using Asseta.Application.Features.ContinuityPlan.Queries.GetPlanEmergencyBrief;
using Asseta.Application.Features.ContinuityPlan.Queries.GetPlanReadinessAudit;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Asseta.UnitTests.Application.ContinuityPlan;

public class ContinuityPlanHandlerTests
{
    private AssetaDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AssetaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AssetaDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetContinuityPlan_OwnerWithCards_Returns4StagesAndReadinessMetrics()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var category = new ContinuityCategory(Guid.NewGuid(), "FINANCE", "Tài chính", "Finance", "wallet", 1);
        context.ContinuityCategories.Add(category);

        var delegate1 = new TrustedPerson("Nguyễn Văn A", "a@test.com", "0901234567", "Vợ", 2, ownerId);
        context.TrustedPeople.Add(delegate1);

        var card1 = new ActionCard(
            Guid.NewGuid(), ownerId, category.Id,
            "Thanh toán lãi vay ngân hàng",
            UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL,
            assignedTrustedPersonId: delegate1.Id,
            documentLocationHint: "Két sắt phòng ngủ");

        var card2 = new ActionCard(
            Guid.NewGuid(), ownerId, category.Id,
            "Hợp đồng thuê nhà 72h",
            UrgencyStage.FIRST_72_HOURS, PriorityLevel.IMPORTANT,
            assignedTrustedPersonId: null,
            documentLocationHint: null);

        context.ActionCards.AddRange(card1, card2);
        await context.SaveChangesAsync();

        var handler = new GetContinuityPlanQueryHandler(context);
        var result = await handler.Handle(new GetContinuityPlanQuery(ownerId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(ownerId, result.OwnerId);
        Assert.Equal(2, result.TotalCardsCount);
        Assert.Equal(4, result.Stages.Count);

        var immediateStage = result.Stages.First(s => s.Stage == UrgencyStage.IMMEDIATE);
        Assert.Single(immediateStage.Cards);
        Assert.Equal("Thanh toán lãi vay ngân hàng", immediateStage.Cards[0].Title);
        Assert.False(immediateStage.Cards[0].HasStageGap);
        Assert.Equal("Nguyễn Văn A (Vợ)", immediateStage.Cards[0].AssignedTrustedPersonName);

        var stage72 = result.Stages.First(s => s.Stage == UrgencyStage.FIRST_72_HOURS);
        Assert.Single(stage72.Cards);
        Assert.True(stage72.Cards[0].HasStageGap);
        Assert.Equal(1, stage72.GapCardsCount);
    }

    [Fact]
    public async Task UpdateActionCardStage_ValidRequest_UpdatesStageAndCreatesAuditLog()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var category = new ContinuityCategory(Guid.NewGuid(), "BUSINESS", "Doanh nghiệp", "Business", "briefcase", 1);
        context.ContinuityCategories.Add(category);

        var card = new ActionCard(
            Guid.NewGuid(), ownerId, category.Id,
            "Bàn giao điều hành",
            UrgencyStage.FIRST_72_HOURS, PriorityLevel.CRITICAL);
        context.ActionCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new UpdateActionCardStageCommandHandler(context);
        var command = new UpdateActionCardStageCommand(card.Id, ownerId, UrgencyStage.IMMEDIATE, 1);

        var updatedCardDto = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(UrgencyStage.IMMEDIATE, updatedCardDto.Urgency);
        Assert.Equal(2, updatedCardDto.RowVersion);

        var auditLog = await context.ContinuityAuditLogs.FirstOrDefaultAsync(l => l.Action == "ACTION_CARD_STAGE_UPDATED");
        Assert.NotNull(auditLog);
        Assert.Equal(ownerId, auditLog.OwnerId);
    }

    [Fact]
    public async Task UpdateActionCardStage_ConcurrentMismatch_ThrowsConcurrencyException()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var category = new ContinuityCategory(Guid.NewGuid(), "DOCS", "Hồ sơ", "Docs", "file", 1);
        context.ContinuityCategories.Add(category);

        var card = new ActionCard(
            Guid.NewGuid(), ownerId, category.Id,
            "Di chúc",
            UrgencyStage.LONGER_TERM, PriorityLevel.LOW);
        context.ActionCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new UpdateActionCardStageCommandHandler(context);
        var command = new UpdateActionCardStageCommand(card.Id, ownerId, UrgencyStage.FIRST_7_DAYS, 999);

        await Assert.ThrowsAsync<ConcurrencyException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ToggleActionCardCompletion_ValidCard_TogglesIsCompleted()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var category = new ContinuityCategory(Guid.NewGuid(), "FINANCE", "Tài chính", "Finance", "wallet", 1);
        context.ContinuityCategories.Add(category);

        var card = new ActionCard(
            Guid.NewGuid(), ownerId, category.Id,
            "Thẻ kiểm tra",
            UrgencyStage.IMMEDIATE, PriorityLevel.IMPORTANT);
        context.ActionCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new ToggleActionCardCompletionCommandHandler(context);
        var result = await handler.Handle(new ToggleActionCardCompletionCommand(card.Id, ownerId, 1), CancellationToken.None);

        Assert.True(result.IsCompleted);
        Assert.Equal(2, result.RowVersion);
    }

    [Fact]
    public async Task GetPlanEmergencyBrief_GeneratesTimelineSummaryWithStepsAndContacts()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var category = new ContinuityCategory(Guid.NewGuid(), "FAMILY", "Gia đình", "Family", "heart", 1);
        context.ContinuityCategories.Add(category);

        var delegatePerson = new TrustedPerson("Trần Văn C", "c@test.com", "0988776655", "Anh trai", 2, ownerId);
        context.TrustedPeople.Add(delegatePerson);

        var card = new ActionCard(
            Guid.NewGuid(), ownerId, category.Id,
            "Chăm sóc con gái khẩn cấp",
            UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL,
            assignedTrustedPersonId: delegatePerson.Id,
            documentLocationHint: "Sổ tiêm chủng trong balo học sinh");

        card.AddStep("Đón cháu tại trường tiểu học", "1 giờ");
        card.AddContact("Cô giáo chủ nhiệm", "Giáo viên", "0911223344");

        context.ActionCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new GetPlanEmergencyBriefQueryHandler(context);
        var brief = await handler.Handle(new GetPlanEmergencyBriefQuery(ownerId), CancellationToken.None);

        Assert.NotNull(brief);
        Assert.Equal(ownerId, brief.OwnerId);
        Assert.NotEmpty(brief.PrimaryContacts);

        var immStage = brief.Stages.First(s => s.Stage == UrgencyStage.IMMEDIATE);
        Assert.Single(immStage.ActionItems);
        var item = immStage.ActionItems[0];
        Assert.Equal("Chăm sóc con gái khẩn cấp", item.Title);
        Assert.Equal("Trần Văn C (Anh trai)", item.DelegateName);
        Assert.Equal("0988776655", item.DelegatePhone);
        Assert.Single(item.KeySteps);
        Assert.Single(item.KeyContacts);
    }

    [Fact]
    public async Task GetPlanReadinessAudit_IdentifiesGapsAndRecommendations()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var category = new ContinuityCategory(Guid.NewGuid(), "FINANCE", "Tài chính", "Finance", "wallet", 1);
        context.ContinuityCategories.Add(category);

        var cardWithGap = new ActionCard(
            Guid.NewGuid(), ownerId, category.Id,
            "Thẻ bị thiếu thông tin",
            UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL);
        context.ActionCards.Add(cardWithGap);
        await context.SaveChangesAsync();

        var handler = new GetPlanReadinessAuditQueryHandler(context);
        var audit = await handler.Handle(new GetPlanReadinessAuditQuery(ownerId), CancellationToken.None);

        Assert.NotNull(audit);
        Assert.Single(audit.IdentifiedGaps);
        Assert.Equal(cardWithGap.Id, audit.IdentifiedGaps[0].CardId);
        Assert.NotEmpty(audit.ActionableRecommendations);
    }

    [Fact]
    public async Task GetMyDelegatedPlan_ScopedDelegate_ReturnsOnlyPermittedCards()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var delegateUserId = Guid.NewGuid();

        var category1 = new ContinuityCategory(Guid.NewGuid(), "FINANCE", "Tài chính", "Finance", "wallet", 1);
        var category2 = new ContinuityCategory(Guid.NewGuid(), "REALESTATE", "Bất động sản", "Real Estate", "home", 2);
        context.ContinuityCategories.AddRange(category1, category2);

        var trustedPerson = new TrustedPerson("Delegate User", "del@test.com", "0933445566", "Cộng sự", 2, ownerId);
        trustedPerson.MarkAsPaired(delegateUserId);
        trustedPerson.SetPermissions(new[]
        {
            TrustedPersonPermission.CreateForCategory(trustedPerson.Id, category1.Id, true)
        });
        context.TrustedPeople.Add(trustedPerson);

        var cardInCat1 = new ActionCard(Guid.NewGuid(), ownerId, category1.Id, "Tài chính A", UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL);
        var cardInCat2 = new ActionCard(Guid.NewGuid(), ownerId, category2.Id, "Bất động sản B", UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL);
        context.ActionCards.AddRange(cardInCat1, cardInCat2);
        await context.SaveChangesAsync();

        var handler = new GetMyDelegatedPlanQueryHandler(context);
        var plan = await handler.Handle(new GetMyDelegatedPlanQuery(delegateUserId), CancellationToken.None);

        Assert.NotNull(plan);
        Assert.Equal(1, plan.TotalCardsCount);
        var immStage = plan.Stages.First(s => s.Stage == UrgencyStage.IMMEDIATE);
        Assert.Single(immStage.Cards);
        Assert.Equal("Tài chính A", immStage.Cards[0].Title);
    }
}
