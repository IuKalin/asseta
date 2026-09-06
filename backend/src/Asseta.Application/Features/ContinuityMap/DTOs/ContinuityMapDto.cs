namespace Asseta.Application.Features.ContinuityMap.DTOs;

public record ContinuityCategoryDto(
    Guid CategoryId,
    string Code,
    string Name,
    string Icon,
    int SortOrder,
    int ReadinessScore,
    IReadOnlyList<ContinuityItemDto> Items);

public record ContinuityGapDto(
    Guid ItemId,
    string ItemName,
    string CategoryCode,
    string Priority,
    IReadOnlyList<string> MissingFields);

public record ContinuityMapDto(
    int OverallReadinessScore,
    int TotalItems,
    int TotalGaps,
    IReadOnlyList<ContinuityCategoryDto> Categories,
    IReadOnlyList<ContinuityGapDto> Gaps);
