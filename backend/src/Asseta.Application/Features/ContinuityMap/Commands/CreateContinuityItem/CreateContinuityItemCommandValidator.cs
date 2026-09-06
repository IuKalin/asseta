using Asseta.Application.Common.Security;
using FluentValidation;

namespace Asseta.Application.Features.ContinuityMap.Commands.CreateContinuityItem;

public class CreateContinuityItemCommandValidator : AbstractValidator<CreateContinuityItemCommand>
{
    public CreateContinuityItemCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(200).WithMessage("Item name cannot exceed 200 characters.")
            .Custom((name, context) =>
            {
                SensitiveDataInspector.EnsureSafe(name, nameof(name));
            });

        RuleFor(x => x.DocumentLocationHint)
            .MaximumLength(255).WithMessage("Document location hint cannot exceed 255 characters.")
            .Custom((hint, context) =>
            {
                SensitiveDataInspector.EnsureSafe(hint, nameof(hint));
            });

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority level.");
    }
}
