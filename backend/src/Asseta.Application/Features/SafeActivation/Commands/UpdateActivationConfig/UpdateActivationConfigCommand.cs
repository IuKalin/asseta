using Asseta.Application.Features.SafeActivation.DTOs;
using MediatR;

namespace Asseta.Application.Features.SafeActivation.Commands.UpdateActivationConfig;

public record UpdateActivationConfigCommand(
    Guid OwnerId,
    int CheckInIntervalDays,
    int GracePeriodHours,
    int MinConfirmationsRequired) : IRequest<ActivationConfigDto>;
