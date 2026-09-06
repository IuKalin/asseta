using Asseta.Application.Features.ContinuityPlan.DTOs;
using MediatR;

namespace Asseta.Application.Features.ContinuityPlan.Commands.ToggleActionCardCompletion;

public record ToggleActionCardCompletionCommand(
    Guid CardId,
    Guid OwnerId,
    int RowVersion
) : IRequest<ContinuityPlanCardItemDto>;
