using Asseta.Domain.Entities;

namespace Asseta.Application.Features.ActionCards.DTOs;

public record ActionCardStepDto(
    Guid Id,
    Guid ActionCardId,
    int StepOrder,
    string Instruction,
    string? EstimatedDuration,
    bool IsCompleted)
{
    public static ActionCardStepDto FromEntity(ActionCardStep step)
    {
        return new ActionCardStepDto(
            step.Id,
            step.ActionCardId,
            step.StepOrder,
            step.Instruction,
            step.EstimatedDuration,
            step.IsCompleted);
    }
}
