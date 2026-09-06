using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Domain.ValueObjects;

namespace Asseta.Domain.Services;

public static class ReadinessScoreCalculator
{
    private const double WeightCritical = 50.0;
    private const double WeightImportant = 35.0;
    private const double WeightLow = 15.0;

    /// <summary>
    /// Calculates the readiness score for a single category based on its active items.
    /// Returns 0 if there are no items to prevent DivideByZeroException.
    /// </summary>
    public static ReadinessScore CalculateCategoryScore(IEnumerable<ContinuityItem> items)
    {
        var activeItems = items.Where(i => !i.IsDeleted).ToList();
        if (activeItems.Count == 0)
        {
            return ReadinessScore.Zero;
        }

        double totalWeightedCompleteness = 0.0;
        double totalWeights = 0.0;

        foreach (var item in activeItems)
        {
            double weight = item.Priority switch
            {
                PriorityLevel.CRITICAL => WeightCritical,
                PriorityLevel.IMPORTANT => WeightImportant,
                PriorityLevel.LOW => WeightLow,
                _ => WeightLow
            };

            double completeness = CalculateItemCompleteness(item);
            totalWeightedCompleteness += weight * completeness;
            totalWeights += weight;
        }

        if (totalWeights <= 0)
        {
            return ReadinessScore.Zero;
        }

        int score = (int)Math.Round((totalWeightedCompleteness / totalWeights) * 100.0);
        return new ReadinessScore(Math.Clamp(score, 0, 100));
    }

    /// <summary>
    /// Calculates the overall readiness score across all categories.
    /// </summary>
    public static ReadinessScore CalculateOverallScore(IDictionary<Guid, IEnumerable<ContinuityItem>> itemsByCategory)
    {
        if (itemsByCategory == null || itemsByCategory.Count == 0)
        {
            return ReadinessScore.Zero;
        }

        var validCategoryScores = new List<int>();

        foreach (var (_, items) in itemsByCategory)
        {
            var activeItems = items.Where(i => !i.IsDeleted).ToList();
            if (activeItems.Count > 0)
            {
                validCategoryScores.Add(CalculateCategoryScore(activeItems).Value);
            }
        }

        if (validCategoryScores.Count == 0)
        {
            return ReadinessScore.Zero;
        }

        int average = (int)Math.Round(validCategoryScores.Average());
        return new ReadinessScore(Math.Clamp(average, 0, 100));
    }

    /// <summary>
    /// Measures how complete an item is based on the 4 core dimensions:
    /// 1. Name present (25%)
    /// 2. Priority defined (25%)
    /// 3. Document location hint recorded (25%)
    /// 4. Trusted contact assigned (25%)
    /// </summary>
    public static double CalculateItemCompleteness(ContinuityItem item)
    {
        if (item.IsDeleted)
        {
            return 0.0;
        }

        int points = 0;

        if (!string.IsNullOrWhiteSpace(item.Name))
            points += 25;

        // Priority is non-nullable enum, so always present
        points += 25;

        if (!string.IsNullOrWhiteSpace(item.DocumentLocationHint))
            points += 25;

        if (item.AssignedTrustedPersonId.HasValue && item.AssignedTrustedPersonId.Value != Guid.Empty)
            points += 25;

        return points / 100.0;
    }

    /// <summary>
    /// Identifies all items that suffer from continuity gaps (unassigned or undocumented critical/important items).
    /// </summary>
    public static IReadOnlyList<ContinuityItem> DetectGaps(IEnumerable<ContinuityItem> items)
    {
        return items
            .Where(i => !i.IsDeleted && i.HasContinuityGap)
            .OrderByDescending(i => i.Priority)
            .ThenBy(i => i.SortOrder)
            .ToList();
    }
}
