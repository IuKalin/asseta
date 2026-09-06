using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteActionStep;

public class DeleteActionStepCommandValidator : AbstractValidator<DeleteActionStepCommand>
{
    public DeleteActionStepCommandValidator()
    {
        RuleFor(x => x.StepId)
            .NotEmpty().WithMessage("StepId is required.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");
    }
}
