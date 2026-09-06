using Asseta.Domain.Entities;
using Asseta.Domain.Enums;

namespace Asseta.Application.Features.ContinuityMap.DTOs;

public record ContinuityItemDto(
    Guid Id,
    Guid OwnerId,
    Guid CategoryId,
    string CategoryCode,
    string Name,
    string Priority,
    string? DocumentLocationHint,
    Guid? AssignedTrustedPersonId,
    Guid? ActionCardId,
    bool HasConfidentialNotes,
    string? CipherNotesBlob,
    string? CipherNonce,
    string? CipherAuthTag,
    bool IsCompleted,
    bool HasContinuityGap,
    int RowVersion,
    int SortOrder,
    DateTime CreatedAtUtc)
{
    public static ContinuityItemDto FromEntity(ContinuityItem item, string categoryCode = "")
    {
        return new ContinuityItemDto(
            item.Id,
            item.OwnerId,
            item.CategoryId,
            string.IsNullOrEmpty(categoryCode) ? (item.Category?.Code ?? "") : categoryCode,
            item.Name,
            item.Priority.ToString(),
            item.DocumentLocationHint,
            item.AssignedTrustedPersonId,
            item.ActionCardId,
            item.HasConfidentialNotes,
            item.CipherNotesBlob,
            item.CipherNonce,
            item.CipherAuthTag,
            item.IsCompleted,
            item.HasContinuityGap,
            item.RowVersion,
            item.SortOrder,
            item.CreatedAtUtc
        );
    }
}
