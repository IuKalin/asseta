using Asseta.Application.Features.SafeActivation.DTOs;
using MediatR;

namespace Asseta.Application.Features.SafeActivation.Commands.VitalityCheckIn;

public record VitalityCheckInCommand(Guid OwnerId) : IRequest<ActivationStatusDto>;
