using Asseta.Application.Common.Exceptions;
using Asseta.Application.Features.ActionCards.Commands.AddActionStep;
using Asseta.Application.Features.ActionCards.Commands.AddKeyContact;
using Asseta.Application.Features.ActionCards.Commands.CreateActionCard;
using Asseta.Application.Features.ActionCards.Commands.CreateActionCardFromItem;
using Asseta.Application.Features.ActionCards.Commands.DeleteActionCard;
using Asseta.Application.Features.ActionCards.Commands.UpdateActionCard;
using Asseta.Application.Features.ActionCards.EventHandlers;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Domain.Services;
using Asseta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Asseta.UnitTests.Application.ActionCards;

public class ActionCardHandlerTests
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
    public async Task CreateActionCard_ShouldPersistCardAndAuditLog()
    {
        var context = CreateInMemoryContext();
        var category = await context.ContinuityCategories.FirstAsync();
        var ownerId = Guid.NewGuid();

        var handler = new CreateActionCardCommandHandler(context);
        var command = new CreateActionCardCommand(
            ownerId,
            category.Id,
            "Quản lý khoản vay",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL,
            "Ghi chú",
            Guid.NewGuid(),
            "Tủ tài liệu");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Quản lý khoản vay", result.Title);
        Assert.Equal("FIRST_72_HOURS", result.Urgency);
        Assert.Equal("CRITICAL", result.Priority);

        var cardInDb = await context.ActionCards.FindAsync(result.Id);
        Assert.NotNull(cardInDb);
        Assert.Equal(ownerId, cardInDb.OwnerId);

        var auditInDb = await context.ContinuityAuditLogs.FirstOrDefaultAsync(a => a.ItemId == result.Id);
        Assert.NotNull(auditInDb);
        Assert.Equal("ACTION_CARD_CREATED", auditInDb.Action);
    }

    [Fact]
    public async Task CreateActionCardFromItem_WithTemplate_ShouldInheritStepsAndRoles()
    {
        var context = CreateInMemoryContext();
        var category = await context.ContinuityCategories.FirstAsync();
        var ownerId = Guid.NewGuid();

        var item = new ContinuityItem(
            ownerId,
            category.Id,
            "Vay thế chấp VPBank",
            PriorityLevel.CRITICAL,
            "Hộc tủ A",
            Guid.NewGuid());

        context.ContinuityItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new CreateActionCardFromItemCommandHandler(context);
        var command = new CreateActionCardFromItemCommand(
            ownerId,
            item.Id,
            "TPL_BANK_LOAN");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(item.Id, result.ContinuityItemId);
        Assert.NotEmpty(result.Steps); // Should inherit 3 steps from TPL_BANK_LOAN
        Assert.NotEmpty(result.Contacts); // Should inherit suggested roles from template

        var updatedItem = await context.ContinuityItems.FindAsync(item.Id);
        Assert.Equal(result.Id, updatedItem!.ActionCardId);
    }

    [Fact]
    public async Task UpdateActionCard_VersionMismatch_ShouldThrowConcurrencyException()
    {
        var context = CreateInMemoryContext();
        var category = await context.ContinuityCategories.FirstAsync();
        var ownerId = Guid.NewGuid();

        var card = new ActionCard(
            Guid.NewGuid(),
            ownerId,
            category.Id,
            "Ban đầu",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.IMPORTANT);

        context.ActionCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new UpdateActionCardCommandHandler(context);
        var command = new UpdateActionCardCommand(
            card.Id,
            ownerId,
            "Tiêu đề mới",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL,
            RowVersion: 99); // Wrong RowVersion

        await Assert.ThrowsAsync<ConcurrencyException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task AddActionStep_ExceedingLimit_ShouldThrowInvalidOperationException()
    {
        var context = CreateInMemoryContext();
        var category = await context.ContinuityCategories.FirstAsync();
        var ownerId = Guid.NewGuid();

        var card = new ActionCard(
            Guid.NewGuid(),
            ownerId,
            category.Id,
            "Thẻ 20 bước",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL);

        for (int i = 1; i <= 20; i++)
        {
            card.AddStep($"Bước số {i}");
        }

        context.ActionCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new AddActionStepCommandHandler(context);
        var command = new AddActionStepCommand(card.Id, ownerId, "Bước thứ 21 vượt giới hạn");

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task AddKeyContact_ExceedingLimit_ShouldThrowInvalidOperationException()
    {
        var context = CreateInMemoryContext();
        var category = await context.ContinuityCategories.FirstAsync();
        var ownerId = Guid.NewGuid();

        var card = new ActionCard(
            Guid.NewGuid(),
            ownerId,
            category.Id,
            "Thẻ 5 contacts",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL);

        for (int i = 1; i <= 5; i++)
        {
            card.AddContact($"Người {i}", "Luật sư");
        }

        context.ActionCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new AddKeyContactCommandHandler(context);
        var command = new AddKeyContactCommand(card.Id, ownerId, "Người thứ 6", "Kế toán");

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ActionCardCompletedEventHandler_ShouldClearGapOnContinuityItem()
    {
        var context = CreateInMemoryContext();
        var category = await context.ContinuityCategories.FirstAsync();
        var ownerId = Guid.NewGuid();

        // Item with a gap (no contact, no location)
        var item = new ContinuityItem(
            ownerId,
            category.Id,
            "Tài khoản chứng khoán VPS",
            PriorityLevel.CRITICAL);

        Assert.True(item.HasContinuityGap);

        context.ContinuityItems.Add(item);
        await context.SaveChangesAsync();

        var trustedPersonId = Guid.NewGuid();
        var card = new ActionCard(
            Guid.NewGuid(),
            ownerId,
            category.Id,
            "Xử lý tài khoản VPS",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL,
            continuityItemId: item.Id,
            assignedTrustedPersonId: trustedPersonId,
            documentLocationHint: "Sổ tay bảo mật");

        card.AddStep("Đăng nhập tài khoản kiểm tra số dư");
        card.EvaluateCompletion();
        Assert.True(card.IsCompleted);

        context.ActionCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new ActionCardCompletedEventHandler(context);
        await handler.SyncCardCompletionAsync(card.Id, ownerId);

        var updatedItem = await context.ContinuityItems.FindAsync(item.Id);
        Assert.NotNull(updatedItem);
        Assert.False(updatedItem.HasContinuityGap); // Gap is CLEARED
        Assert.True(updatedItem.IsCompleted);
        Assert.Equal("Sổ tay bảo mật", updatedItem.DocumentLocationHint);
        Assert.Equal(trustedPersonId, updatedItem.AssignedTrustedPersonId);
    }
}
