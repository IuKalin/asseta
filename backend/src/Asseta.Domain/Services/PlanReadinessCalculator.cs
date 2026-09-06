using Asseta.Domain.Entities;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Services;

public class PlanReadinessReport
{
    public int OverallScore { get; set; }
    public double DelegateCoveragePercentage { get; set; }
    public double DocumentReadinessPercentage { get; set; }
    public int TotalGapsCount { get; set; }
    public bool HasSinglePointOfFailureRisk { get; set; }
    public Guid? DominantDelegateId { get; set; }
    public double DominantDelegateRatio { get; set; }
    public Dictionary<UrgencyStage, StageReadinessMetrics> StageMetrics { get; set; } = new();
}

public class StageReadinessMetrics
{
    public UrgencyStage Stage { get; set; }
    public int TotalCards { get; set; }
    public int CompletedCards { get; set; }
    public int GapCards { get; set; }
    public double DelegateCoveragePercentage { get; set; }
    public double DocumentReadinessPercentage { get; set; }
    public int StageScore { get; set; }
}

public static class PlanReadinessCalculator
{
    public static bool HasStageGap(ActionCard card)
    {
        if (card.Urgency == UrgencyStage.IMMEDIATE || card.Urgency == UrgencyStage.FIRST_72_HOURS)
        {
            // For immediate / 72 hours: Both delegate and document location are critical
            bool hasDelegate = card.AssignedTrustedPersonId.HasValue && card.AssignedTrustedPersonId.Value != Guid.Empty;
            bool hasDocument = !string.IsNullOrWhiteSpace(card.DocumentLocationHint) || !string.IsNullOrWhiteSpace(card.DigitalStorageLink);
            return !hasDelegate || !hasDocument;
        }
        else
        {
            // For longer stages: at least a delegate or document location should be identified
            bool hasDelegate = card.AssignedTrustedPersonId.HasValue && card.AssignedTrustedPersonId.Value != Guid.Empty;
            bool hasDocument = !string.IsNullOrWhiteSpace(card.DocumentLocationHint) || !string.IsNullOrWhiteSpace(card.DigitalStorageLink);
            return !hasDelegate && !hasDocument;
        }
    }

    public static StageReadinessMetrics CalculateStageMetrics(UrgencyStage stage, IReadOnlyCollection<ActionCard> stageCards)
    {
        int total = stageCards.Count;
        if (total == 0)
        {
            return new StageReadinessMetrics
            {
                Stage = stage,
                TotalCards = 0,
                CompletedCards = 0,
                GapCards = 0,
                DelegateCoveragePercentage = 100.0,
                DocumentReadinessPercentage = 100.0,
                StageScore = 100
            };
        }

        int completed = stageCards.Count(c => c.IsCompleted);
        int gaps = stageCards.Count(HasStageGap);
        int withDelegate = stageCards.Count(c => c.AssignedTrustedPersonId.HasValue && c.AssignedTrustedPersonId.Value != Guid.Empty);
        int withDoc = stageCards.Count(c => !string.IsNullOrWhiteSpace(c.DocumentLocationHint) || !string.IsNullOrWhiteSpace(c.DigitalStorageLink));

        double delegatePct = Math.Round((double)withDelegate / total * 100.0, 1);
        double docPct = Math.Round((double)withDoc / total * 100.0, 1);

        int score = (int)Math.Round((delegatePct * 0.5) + (docPct * 0.5));

        return new StageReadinessMetrics
        {
            Stage = stage,
            TotalCards = total,
            CompletedCards = completed,
            GapCards = gaps,
            DelegateCoveragePercentage = delegatePct,
            DocumentReadinessPercentage = docPct,
            StageScore = Math.Clamp(score, 0, 100)
        };
    }

    public static PlanReadinessReport CalculatePlanReadiness(IReadOnlyCollection<ActionCard> cards)
    {
        var report = new PlanReadinessReport();
        int totalCards = cards.Count;

        if (totalCards == 0)
        {
            report.OverallScore = 0;
            report.DelegateCoveragePercentage = 0.0;
            report.DocumentReadinessPercentage = 0.0;
            report.TotalGapsCount = 0;
            report.HasSinglePointOfFailureRisk = false;
            return report;
        }

        int withDelegate = cards.Count(c => c.AssignedTrustedPersonId.HasValue && c.AssignedTrustedPersonId.Value != Guid.Empty);
        int withDoc = cards.Count(c => !string.IsNullOrWhiteSpace(c.DocumentLocationHint) || !string.IsNullOrWhiteSpace(c.DigitalStorageLink));
        int totalGaps = cards.Count(HasStageGap);

        report.DelegateCoveragePercentage = Math.Round((double)withDelegate / totalCards * 100.0, 1);
        report.DocumentReadinessPercentage = Math.Round((double)withDoc / totalCards * 100.0, 1);
        report.TotalGapsCount = totalGaps;

        // Weights: IMMEDIATE (40%), FIRST_72_HOURS (30%), FIRST_7_DAYS (20%), LONGER_TERM (10%)
        var stageWeights = new Dictionary<UrgencyStage, double>
        {
            { UrgencyStage.IMMEDIATE, 0.40 },
            { UrgencyStage.FIRST_72_HOURS, 0.30 },
            { UrgencyStage.FIRST_7_DAYS, 0.20 },
            { UrgencyStage.LONGER_TERM, 0.10 }
        };

        double weightedScore = 0.0;
        foreach (UrgencyStage stage in Enum.GetValues<UrgencyStage>())
        {
            var stageCards = cards.Where(c => c.Urgency == stage).ToList();
            var metrics = CalculateStageMetrics(stage, stageCards);
            report.StageMetrics[stage] = metrics;
            weightedScore += metrics.StageScore * stageWeights[stage];
        }

        report.OverallScore = (int)Math.Round(Math.Clamp(weightedScore, 0, 100));

        // Detect SPoF in Immediate & 72 Hours
        var urgentCards = cards.Where(c => c.Urgency == UrgencyStage.IMMEDIATE || c.Urgency == UrgencyStage.FIRST_72_HOURS).ToList();
        if (urgentCards.Count >= 3)
        {
            var delegateGroups = urgentCards
                .Where(c => c.AssignedTrustedPersonId.HasValue && c.AssignedTrustedPersonId.Value != Guid.Empty)
                .GroupBy(c => c.AssignedTrustedPersonId!.Value)
                .Select(g => new { DelegateId = g.Key, Count = g.Count(), Ratio = (double)g.Count() / urgentCards.Count })
                .OrderByDescending(g => g.Ratio)
                .FirstOrDefault();

            if (delegateGroups != null && delegateGroups.Ratio >= 0.70)
            {
                report.HasSinglePointOfFailureRisk = true;
                report.DominantDelegateId = delegateGroups.DelegateId;
                report.DominantDelegateRatio = Math.Round(delegateGroups.Ratio * 100.0, 1);
            }
        }

        return report;
    }
}
