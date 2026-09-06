using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Domain.Services;
using Xunit;

namespace Asseta.UnitTests.Domain;

public class ReadinessScoreCalculatorTests
{
    [Fact]
    public void CalculateCategoryScore_EmptyList_ShouldReturnZeroWithoutException()
    {
        var score = ReadinessScoreCalculator.CalculateCategoryScore(new List<ContinuityItem>());

        Assert.Equal(0, score.Value);
    }

    [Fact]
    public void CalculateCategoryScore_OnlyDeletedItems_ShouldReturnZero()
    {
        var item = new ContinuityItem(Guid.NewGuid(), Guid.NewGuid(), "Deleted item", PriorityLevel.CRITICAL);
        item.SoftDelete();

        var score = ReadinessScoreCalculator.CalculateCategoryScore(new[] { item });

        Assert.Equal(0, score.Value);
    }

    [Fact]
    public void CalculateCategoryScore_FullyCompletedItem_ShouldReturn100()
    {
        var item = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sổ đỏ căn hộ 1204",
            PriorityLevel.CRITICAL,
            documentLocationHint: "Tủ tài liệu số 2",
            assignedTrustedPersonId: Guid.NewGuid());

        var score = ReadinessScoreCalculator.CalculateCategoryScore(new[] { item });

        Assert.Equal(100, score.Value);
    }

    [Fact]
    public void CalculateCategoryScore_HalfCompletedItem_ShouldReturn50()
    {
        // Has Name + Priority (50%), lacks DocumentLocationHint and AssignedTrustedPersonId
        var item = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Hợp đồng bảo hiểm AIA",
            PriorityLevel.IMPORTANT);

        var score = ReadinessScoreCalculator.CalculateCategoryScore(new[] { item });

        Assert.Equal(50, score.Value);
    }

    [Fact]
    public void CalculateCategoryScore_WeightedPriority_CriticalHasHigherImpact()
    {
        // Item 1: Critical, 100% complete (weight 50)
        var criticalItem = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Doanh nghiệp ABC",
            PriorityLevel.CRITICAL,
            documentLocationHint: "Phòng kế toán",
            assignedTrustedPersonId: Guid.NewGuid());

        // Item 2: Low, 50% complete (weight 15)
        var lowItem = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Thẻ thành viên golf",
            PriorityLevel.LOW);

        // Expected: (50 * 1.0 + 15 * 0.5) / (50 + 15) = (50 + 7.5) / 65 = 57.5 / 65 = 88.46% => 88%
        var score = ReadinessScoreCalculator.CalculateCategoryScore(new[] { criticalItem, lowItem });

        Assert.Equal(88, score.Value);
    }

    [Fact]
    public void DetectGaps_ShouldReturnCriticalAndImportantItemsMissingData()
    {
        var gapItem = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Khoản vay thế chấp",
            PriorityLevel.CRITICAL,
            documentLocationHint: null, // missing!
            assignedTrustedPersonId: null);

        var okItem = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Hợp đồng mua xe",
            PriorityLevel.CRITICAL,
            documentLocationHint: "Hộc bàn",
            assignedTrustedPersonId: Guid.NewGuid());

        var gaps = ReadinessScoreCalculator.DetectGaps(new[] { gapItem, okItem });

        Assert.Single(gaps);
        Assert.Equal("Khoản vay thế chấp", gaps[0].Name);
    }
}
