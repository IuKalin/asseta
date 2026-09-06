using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Enums;
using MediatR;

namespace Asseta.Application.Features.ContinuityMap.Commands.UpdateContinuityItem;

public record UpdateContinuityItemCommand(
    Guid Id,
    Guid OwnerId,
    string Name,
    PriorityLevel Priority,
    string? DocumentLocationHint,
    Guid? AssignedTrustedPersonId,
    string? CipherNotesBlob,
    string? CipherNonce,
    string? CipherAuthTag,
    int RowVersion) : IRequest<ContinuityItemDto>;
