using Asseta.Application.Features.ContinuityPlan.DTOs;
using Asseta.Domain.Enums;
using MediatR;

namespace Asseta.Application.Features.ContinuityPlan.Commands.UpdateActionCardStage;

public record UpdateActionCardStageCommand(
    Guid CardId,
    Guid OwnerId,
    UrgencyStage NewStage,
    int RowVersion
) : IRequest<ContinuityPlanCardItemDto>;
