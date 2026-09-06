using Asseta.Application.Features.ActionCards.DTOs;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.AddKeyContact;

public record AddKeyContactCommand(
    Guid ActionCardId,
    Guid OwnerId,
    string ContactName,
    string RelationshipOrRole,
    string? PhoneNumber = null,
    string? Email = null,
    string? ContactNotes = null) : IRequest<ActionCardContactDto>;
