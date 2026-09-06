using Asseta.Application.Features.SafeActivation.DTOs;
using MediatR;

namespace Asseta.Application.Features.SafeActivation.Commands.InitiateActivationRequest;

public record InitiateActivationRequestCommand(
    Guid CurrentUserId,
    Guid TargetOwnerId,
    string? Reason = null) : IRequest<ActivationRequestDto>;
