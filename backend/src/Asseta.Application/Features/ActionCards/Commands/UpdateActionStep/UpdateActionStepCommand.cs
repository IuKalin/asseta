using Asseta.Application.Features.ActionCards.DTOs;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.UpdateActionStep;

public record UpdateActionStepCommand(
    Guid StepId,
    Guid OwnerId,
    string Instruction,
    string? EstimatedDuration = null,
    bool? IsCompleted = null) : IRequest<ActionCardStepDto>;
