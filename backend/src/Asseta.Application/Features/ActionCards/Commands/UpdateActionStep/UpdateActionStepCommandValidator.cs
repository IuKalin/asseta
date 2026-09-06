using Asseta.Application.Common.Security;
using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.UpdateActionStep;

public class UpdateActionStepCommandValidator : AbstractValidator<UpdateActionStepCommand>
{
    public UpdateActionStepCommandValidator()
    {
        RuleFor(x => x.StepId)
            .NotEmpty().WithMessage("StepId is required.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");

        RuleFor(x => x.Instruction)
            .NotEmpty().WithMessage("Instruction is required.")
            .MaximumLength(500).WithMessage("Instruction cannot exceed 500 characters.")
            .Custom((instruction, context) =>
            {
                SensitiveDataInspector.EnsureSafe(instruction, nameof(instruction));
            });

        RuleFor(x => x.EstimatedDuration)
            .MaximumLength(50).WithMessage("EstimatedDuration cannot exceed 50 characters.");
    }
}
