using Asseta.Domain.Enums;

namespace Asseta.Application.Features.ContinuityPlan.DTOs;

public class ContinuityPlanDto
{
    public Guid OwnerId { get; set; }
    public int TotalCardsCount { get; set; }
    public int CompletedCardsCount { get; set; }
    public int GapsCount { get; set; }
    public double DelegateCoveragePercentage { get; set; }
    public double DocumentReadinessPercentage { get; set; }
    public int OverallPlanReadinessScore { get; set; }
    public bool HasSinglePointOfFailureRisk { get; set; }
    public string? SinglePointOfFailureWarning { get; set; }
    public List<ContinuityPlanStageDto> Stages { get; set; } = new();
}

public class ContinuityPlanStageDto
{
    public UrgencyStage Stage { get; set; }
    public string StageName { get; set; } = string.Empty;
    public string StageDescription { get; set; } = string.Empty;
    public int TotalCardsCount { get; set; }
    public int CompletedCardsCount { get; set; }
    public int GapCardsCount { get; set; }
    public List<ContinuityPlanCardItemDto> Cards { get; set; } = new();
}

public class ContinuityPlanCardItemDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public UrgencyStage Urgency { get; set; }
    public PriorityLevel Priority { get; set; }
    public Guid? AssignedTrustedPersonId { get; set; }
    public string? AssignedTrustedPersonName { get; set; }
    public string? AssignedTrustedPersonPhone { get; set; }
    public string? DocumentLocationHint { get; set; }
    public bool HasDocumentLocation { get; set; }
    public bool HasStageGap { get; set; }
    public bool IsCompleted { get; set; }
    public int StepsCount { get; set; }
    public int ContactsCount { get; set; }
    public int RowVersion { get; set; }
}

public class OfflineEmergencyBriefDto
{
    public Guid OwnerId { get; set; }
    public DateTime GeneratedAtUtc { get; set; }
    public List<EmergencyBriefStageDto> Stages { get; set; } = new();
    public List<EmergencyContactSummaryDto> PrimaryContacts { get; set; } = new();
}

public class EmergencyBriefStageDto
{
    public UrgencyStage Stage { get; set; }
    public string StageName { get; set; } = string.Empty;
    public List<EmergencyBriefCardDto> ActionItems { get; set; } = new();
}

public class EmergencyBriefCardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public string? DelegateName { get; set; }
    public string? DelegatePhone { get; set; }
    public string? DocumentLocationHint { get; set; }
    public List<string> KeySteps { get; set; } = new();
    public List<string> KeyContacts { get; set; } = new();
}

public class EmergencyContactSummaryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? RoleOrRelationship { get; set; }
    public string RelatedCardTitle { get; set; } = string.Empty;
}

public class PlanReadinessAuditDto
{
    public Guid OwnerId { get; set; }
    public int OverallScore { get; set; }
    public double DelegateCoveragePercentage { get; set; }
    public double DocumentReadinessPercentage { get; set; }
    public int TotalGapsCount { get; set; }
    public bool HasSinglePointOfFailureRisk { get; set; }
    public string? SinglePointOfFailureDetails { get; set; }
    public List<StageAuditSummaryDto> StageAudits { get; set; } = new();
    public List<PlanGapItemDto> IdentifiedGaps { get; set; } = new();
    public List<string> ActionableRecommendations { get; set; } = new();
}

public class StageAuditSummaryDto
{
    public UrgencyStage Stage { get; set; }
    public string StageName { get; set; } = string.Empty;
    public int TotalCards { get; set; }
    public int GapCards { get; set; }
    public int StageScore { get; set; }
}

public class PlanGapItemDto
{
    public Guid CardId { get; set; }
    public string CardTitle { get; set; } = string.Empty;
    public UrgencyStage Stage { get; set; }
    public PriorityLevel Priority { get; set; }
    public string GapReason { get; set; } = string.Empty;
    public string RecommendedAction { get; set; } = string.Empty;
}
