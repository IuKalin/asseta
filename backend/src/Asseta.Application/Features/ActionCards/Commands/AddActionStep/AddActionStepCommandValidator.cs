using Asseta.Application.Common.Security;
using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.AddActionStep;

public class AddActionStepCommandValidator : AbstractValidator<AddActionStepCommand>
{
    public AddActionStepCommandValidator()
    {
        RuleFor(x => x.ActionCardId)
            .NotEmpty().WithMessage("ActionCardId is required.");

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
