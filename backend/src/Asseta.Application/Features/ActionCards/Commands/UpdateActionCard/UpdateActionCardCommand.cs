using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Enums;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.UpdateActionCard;

public record UpdateActionCardCommand(
    Guid Id,
    Guid OwnerId,
    string Title,
    UrgencyStage Urgency,
    PriorityLevel Priority,
    int RowVersion,
    string? Summary = null,
    Guid? AssignedTrustedPersonId = null,
    string? DocumentLocationHint = null,
    string? DigitalStorageLink = null,
    string? CipherInstructionsBlob = null,
    string? CipherNonce = null,
    string? CipherAuthTag = null) : IRequest<ActionCardDto>;
