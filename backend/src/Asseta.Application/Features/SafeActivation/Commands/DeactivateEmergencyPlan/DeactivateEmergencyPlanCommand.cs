using MediatR;

namespace Asseta.Application.Features.SafeActivation.Commands.DeactivateEmergencyPlan;

public record DeactivateEmergencyPlanCommand(Guid OwnerId) : IRequest<bool>;
