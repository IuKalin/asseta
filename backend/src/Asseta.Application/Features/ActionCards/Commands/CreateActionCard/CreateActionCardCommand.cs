using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Enums;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.CreateActionCard;

public record CreateActionCardCommand(
    Guid OwnerId,
    Guid CategoryId,
    string Title,
    UrgencyStage Urgency,
    PriorityLevel Priority,
    string? Summary = null,
    Guid? AssignedTrustedPersonId = null,
    string? DocumentLocationHint = null,
    string? DigitalStorageLink = null,
    string? CipherInstructionsBlob = null,
    string? CipherNonce = null,
    string? CipherAuthTag = null) : IRequest<ActionCardDto>;
