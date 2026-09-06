using Asseta.Application.Common.Security;
using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.CreateActionCardFromItem;

public class CreateActionCardFromItemCommandValidator : AbstractValidator<CreateActionCardFromItemCommand>
{
    public CreateActionCardFromItemCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");

        RuleFor(x => x.ContinuityItemId)
            .NotEmpty().WithMessage("ContinuityItemId is required.");

        RuleFor(x => x.TemplateCode)
            .MaximumLength(50).WithMessage("Template code cannot exceed 50 characters.");

        RuleFor(x => x.Summary)
            .MaximumLength(1000).WithMessage("Summary cannot exceed 1000 characters.")
            .Custom((summary, context) =>
            {
                SensitiveDataInspector.EnsureSafe(summary, nameof(summary));
            });

        When(x => x.Urgency.HasValue, () =>
        {
            RuleFor(x => x.Urgency!.Value)
                .IsInEnum().WithMessage("Invalid urgency stage.");
        });

        When(x => x.Priority.HasValue, () =>
        {
            RuleFor(x => x.Priority!.Value)
                .IsInEnum().WithMessage("Invalid priority level.");
        });
    }
}
