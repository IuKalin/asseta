using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.ReorderActionSteps;

public class ReorderActionStepsCommandValidator : AbstractValidator<ReorderActionStepsCommand>
{
    public ReorderActionStepsCommandValidator()
    {
        RuleFor(x => x.ActionCardId)
            .NotEmpty().WithMessage("ActionCardId is required.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");

        RuleFor(x => x.OrderedStepIds)
            .NotNull().WithMessage("OrderedStepIds list cannot be null.");
    }
}
