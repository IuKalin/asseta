using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Xunit;

namespace Asseta.UnitTests.Domain;

public class ActionCardTests
{
    [Fact]
    public void Constructor_WithValidArguments_CreatesInstanceSuccessfully()
    {
        var ownerId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var card = new ActionCard(
            Guid.NewGuid(),
            ownerId,
            categoryId,
            "Khoản vay thế chấp Vietcombank",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL);

        Assert.Equal("Khoản vay thế chấp Vietcombank", card.Title);
        Assert.Equal(UrgencyStage.FIRST_72_HOURS, card.Urgency);
        Assert.Equal(PriorityLevel.CRITICAL, card.Priority);
        Assert.False(card.IsCompleted);
        Assert.Equal(1, card.RowVersion);
        Assert.Empty(card.Steps);
        Assert.Empty(card.Contacts);
    }

    [Fact]
    public void Constructor_WithEmptyTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ActionCard(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL));
    }

    [Fact]
    public void AddStep_IncrementsOrder_AndAddsToCollection()
    {
        var card = new ActionCard(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Vay mua xe",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.IMPORTANT);

        var step1 = card.AddStep("Bước 1: Gọi ngân hàng", "15m");
        var step2 = card.AddStep("Bước 2: Tìm hồ sơ xe", "30m");

        Assert.Equal(1, step1.StepOrder);
        Assert.Equal(2, step2.StepOrder);
        Assert.Equal(2, card.Steps.Count);
    }

    [Fact]
    public void AddStep_BeyondTwenty_ThrowsInvalidOperationException()
    {
        var card = new ActionCard(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Kế hoạch dài hạn",
            UrgencyStage.LONGER_TERM,
            PriorityLevel.LOW);

        for (int i = 0; i < 20; i++)
        {
            card.AddStep($"Bước {i + 1}");
        }

        var ex = Assert.Throws<InvalidOperationException>(() => card.AddStep("Bước 21 vượt giới hạn"));
        Assert.Contains("cannot exceed 20 active steps", ex.Message);
    }

    [Fact]
    public void AddContact_BeyondFive_ThrowsInvalidOperationException()
    {
        var card = new ActionCard(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Kế hoạch doanh nghiệp",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL);

        for (int i = 0; i < 5; i++)
        {
            card.AddContact($"Liên hệ {i + 1}", "Đối tác");
        }

        var ex = Assert.Throws<InvalidOperationException>(() => card.AddContact("Liên hệ 6", "Đối tác"));
        Assert.Contains("cannot exceed 5 active key contacts", ex.Message);
    }

    [Fact]
    public void EvaluateCompletion_WhenAllConditionsMet_SetsIsCompletedTrue()
    {
        var trustedPersonId = Guid.NewGuid();
        var card = new ActionCard(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Hồ sơ bảo hiểm nhân thọ",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL,
            assignedTrustedPersonId: trustedPersonId,
            documentLocationHint: "Tủ tài liệu phòng khách");

        Assert.False(card.IsCompleted);

        card.AddStep("Nộp hồ sơ bồi thường cho Prudential");
        Assert.True(card.IsCompleted);
    }

    [Fact]
    public void ReorderSteps_UpdatesOrderCorrectly()
    {
        var card = new ActionCard(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Bàn giao công việc",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL);

        var stepA = card.AddStep("Bước A");
        var stepB = card.AddStep("Bước B");
        var stepC = card.AddStep("Bước C");

        card.ReorderSteps(new[] { stepC.Id, stepA.Id, stepB.Id });

        Assert.Equal(1, stepC.StepOrder);
        Assert.Equal(2, stepA.StepOrder);
        Assert.Equal(3, stepB.StepOrder);
    }
}
