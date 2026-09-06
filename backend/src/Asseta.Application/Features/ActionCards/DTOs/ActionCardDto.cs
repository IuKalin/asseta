using Asseta.Domain.Entities;
using Asseta.Domain.Enums;

namespace Asseta.Application.Features.ActionCards.DTOs;

public record ActionCardDto(
    Guid Id,
    Guid OwnerId,
    Guid CategoryId,
    string CategoryCode,
    string CategoryNameVi,
    Guid? ContinuityItemId,
    string? ContinuityItemName,
    string Title,
    string? Summary,
    string Urgency,
    string Priority,
    Guid? AssignedTrustedPersonId,
    string? DocumentLocationHint,
    string? DigitalStorageLink,
    bool HasConfidentialInstructions,
    string? CipherInstructionsBlob,
    string? CipherNonce,
    string? CipherAuthTag,
    bool IsCompleted,
    int RowVersion,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyList<ActionCardStepDto> Steps,
    IReadOnlyList<ActionCardContactDto> Contacts)
{
    public static ActionCardDto FromEntity(
        ActionCard card,
        string categoryCode = "",
        string categoryNameVi = "",
        string? continuityItemName = null)
    {
        return new ActionCardDto(
            card.Id,
            card.OwnerId,
            card.CategoryId,
            string.IsNullOrEmpty(categoryCode) ? (card.Category?.Code ?? "") : categoryCode,
            string.IsNullOrEmpty(categoryNameVi) ? (card.Category?.NameVi ?? "") : categoryNameVi,
            card.ContinuityItemId,
            continuityItemName ?? card.ContinuityItem?.Name,
            card.Title,
            card.Summary,
            card.Urgency.ToString(),
            card.Priority.ToString(),
            card.AssignedTrustedPersonId,
            card.DocumentLocationHint,
            card.DigitalStorageLink,
            card.CipherInstructions != null,
            card.CipherInstructions?.CipherBlob,
            card.CipherInstructions?.Nonce,
            card.CipherInstructions?.AuthTag,
            card.IsCompleted,
            card.RowVersion,
            card.CreatedAtUtc,
            card.UpdatedAtUtc,
            card.Steps.Where(s => !s.IsDeleted).OrderBy(s => s.StepOrder).Select(ActionCardStepDto.FromEntity).ToList(),
            card.Contacts.Where(c => !c.IsDeleted).Select(ActionCardContactDto.FromEntity).ToList()
        );
    }
}
