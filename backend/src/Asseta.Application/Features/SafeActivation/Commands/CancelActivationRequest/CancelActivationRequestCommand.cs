using MediatR;

namespace Asseta.Application.Features.SafeActivation.Commands.CancelActivationRequest;

public record CancelActivationRequestCommand(
    Guid CurrentUserId,
    Guid RequestId) : IRequest<bool>;
