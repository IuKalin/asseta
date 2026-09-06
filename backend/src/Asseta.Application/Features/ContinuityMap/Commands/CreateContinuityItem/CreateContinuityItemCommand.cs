using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Enums;
using MediatR;

namespace Asseta.Application.Features.ContinuityMap.Commands.CreateContinuityItem;

public record CreateContinuityItemCommand(
    Guid OwnerId,
    Guid CategoryId,
    string Name,
    PriorityLevel Priority,
    string? DocumentLocationHint,
    Guid? AssignedTrustedPersonId,
    string? CipherNotesBlob,
    string? CipherNonce,
    string? CipherAuthTag) : IRequest<ContinuityItemDto>;
