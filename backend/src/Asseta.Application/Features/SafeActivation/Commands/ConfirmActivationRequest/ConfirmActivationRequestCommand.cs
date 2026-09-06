using Asseta.Application.Features.SafeActivation.DTOs;
using MediatR;

namespace Asseta.Application.Features.SafeActivation.Commands.ConfirmActivationRequest;

public record ConfirmActivationRequestCommand(
    Guid CurrentUserId,
    Guid RequestId,
    bool IsConfirmed,
    string? Note = null) : IRequest<ActivationRequestDto>;
