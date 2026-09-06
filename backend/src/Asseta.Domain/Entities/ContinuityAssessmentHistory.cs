using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class ContinuityAssessmentHistory : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public string AssessmentVersion { get; private set; } = "v1";
    public string RawResponses { get; private set; } = string.Empty;
    public int ItemsGeneratedCount { get; private set; }
    public int InitialReadinessScore { get; private set; }

    private ContinuityAssessmentHistory() { }

    public ContinuityAssessmentHistory(
        Guid ownerId,
        string rawResponses,
        int itemsGeneratedCount,
        int initialReadinessScore,
        string assessmentVersion = "v1")
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("OwnerId cannot be empty.", nameof(ownerId));
        if (string.IsNullOrWhiteSpace(rawResponses))
            throw new ArgumentException("RawResponses cannot be empty.", nameof(rawResponses));

        OwnerId = ownerId;
        RawResponses = rawResponses;
        ItemsGeneratedCount = itemsGeneratedCount;
        InitialReadinessScore = initialReadinessScore;
        AssessmentVersion = string.IsNullOrWhiteSpace(assessmentVersion) ? "v1" : assessmentVersion.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }
}
