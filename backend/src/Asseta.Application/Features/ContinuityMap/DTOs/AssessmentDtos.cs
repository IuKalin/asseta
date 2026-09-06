using Asseta.Domain.Enums;

namespace Asseta.Application.Features.ContinuityMap.DTOs;

public record AssessmentAnswerDto(
    string QuestionId,
    string CategoryCode,
    string ItemName,
    bool HasItem,
    PriorityLevel Priority = PriorityLevel.IMPORTANT,
    string? DocumentLocationHint = null);

public record AssessmentResultDto(
    Guid HistoryId,
    int ItemsGeneratedCount,
    int InitialReadinessScore,
    ContinuityMapDto ContinuityMap);
