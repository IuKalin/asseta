using Asseta.Domain.Entities;

namespace Asseta.Application.Features.ActionCards.DTOs;

public record ActionCardContactDto(
    Guid Id,
    Guid ActionCardId,
    string ContactName,
    string RelationshipOrRole,
    string? PhoneNumber,
    string? Email,
    string? ContactNotes)
{
    public static ActionCardContactDto FromEntity(ActionCardContact contact)
    {
        return new ActionCardContactDto(
            contact.Id,
            contact.ActionCardId,
            contact.ContactName,
            contact.RelationshipOrRole,
            contact.PhoneNumber,
            contact.Email,
            contact.ContactNotes);
    }
}
