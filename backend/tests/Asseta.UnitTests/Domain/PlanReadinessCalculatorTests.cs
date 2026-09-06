using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Domain.Services;
using Xunit;

namespace Asseta.UnitTests.Domain;

public class PlanReadinessCalculatorTests
{
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    [Fact]
    public void CalculatePlanReadiness_EmptyCardsList_ReturnsZeroScore()
    {
        var report = PlanReadinessCalculator.CalculatePlanReadiness(new List<ActionCard>());

        Assert.Equal(0, report.OverallScore);
        Assert.Equal(0, report.TotalGapsCount);
        Assert.False(report.HasSinglePointOfFailureRisk);
    }

    [Fact]
    public void HasStageGap_ImmediateCardWithoutDelegate_ReturnsTrue()
    {
        var card = new ActionCard(
            Guid.NewGuid(), _ownerId, _categoryId,
            "Khẩn cấp: Trả nợ ngân hàng",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL,
            documentLocationHint: "Tủ hồ sơ",
            assignedTrustedPersonId: null);

        Assert.True(PlanReadinessCalculator.HasStageGap(card));
    }

    [Fact]
    public void HasStageGap_ImmediateCardWithDelegateAndDoc_ReturnsFalse()
    {
        var card = new ActionCard(
            Guid.NewGuid(), _ownerId, _categoryId,
            "Khẩn cấp: Trả nợ ngân hàng",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL,
            documentLocationHint: "Tủ hồ sơ",
            assignedTrustedPersonId: Guid.NewGuid());

        Assert.False(PlanReadinessCalculator.HasStageGap(card));
    }

    [Fact]
    public void DetectSinglePointOfFailure_SingleDelegateAssignedToOver70PercentOfUrgentCards_FlagsSPoF()
    {
        var dominantDelegate = Guid.NewGuid();
        var cards = new List<ActionCard>
        {
            new ActionCard(Guid.NewGuid(), _ownerId, _categoryId, "Card 1", UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL, assignedTrustedPersonId: dominantDelegate, documentLocationHint: "Hint"),
            new ActionCard(Guid.NewGuid(), _ownerId, _categoryId, "Card 2", UrgencyStage.IMMEDIATE, PriorityLevel.IMPORTANT, assignedTrustedPersonId: dominantDelegate, documentLocationHint: "Hint"),
            new ActionCard(Guid.NewGuid(), _ownerId, _categoryId, "Card 3", UrgencyStage.FIRST_72_HOURS, PriorityLevel.IMPORTANT, assignedTrustedPersonId: dominantDelegate, documentLocationHint: "Hint"),
            new ActionCard(Guid.NewGuid(), _ownerId, _categoryId, "Card 4", UrgencyStage.FIRST_72_HOURS, PriorityLevel.LOW, assignedTrustedPersonId: Guid.NewGuid(), documentLocationHint: "Hint")
        };

        var report = PlanReadinessCalculator.CalculatePlanReadiness(cards);

        Assert.True(report.HasSinglePointOfFailureRisk);
        Assert.Equal(dominantDelegate, report.DominantDelegateId);
        Assert.Equal(75.0, report.DominantDelegateRatio);
    }

    [Fact]
    public void ActionCard_UpdateUrgencyStage_ValidStage_UpdatesAndIncrementsRowVersion()
    {
        var card = new ActionCard(
            Guid.NewGuid(), _ownerId, _categoryId,
            "Chuyển stage thẻ",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.IMPORTANT);

        int initialVersion = card.RowVersion;
        card.UpdateUrgencyStage(UrgencyStage.IMMEDIATE);

        Assert.Equal(UrgencyStage.IMMEDIATE, card.Urgency);
        Assert.Equal(initialVersion + 1, card.RowVersion);
    }

    [Fact]
    public void ActionCard_ToggleCompletion_TogglesStateAndIncrementsRowVersion()
    {
        var card = new ActionCard(
            Guid.NewGuid(), _ownerId, _categoryId,
            "Thẻ kiểm tra",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.IMPORTANT);

        Assert.False(card.IsCompleted);
        int initialVersion = card.RowVersion;

        card.ToggleCompletion();
        Assert.True(card.IsCompleted);
        Assert.Equal(initialVersion + 1, card.RowVersion);

        card.ToggleCompletion();
        Assert.False(card.IsCompleted);
        Assert.Equal(initialVersion + 2, card.RowVersion);
    }
}
