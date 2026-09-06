using Asseta.Application.Common.Security;
using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.UpdateActionCard;

public class UpdateActionCardCommandValidator : AbstractValidator<UpdateActionCardCommand>
{
    public UpdateActionCardCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Card Id is required.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");

        RuleFor(x => x.RowVersion)
            .GreaterThan(0).WithMessage("Valid RowVersion is required for concurrency control.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.")
            .Custom((title, context) =>
            {
                SensitiveDataInspector.EnsureSafe(title, nameof(title));
            });

        RuleFor(x => x.Summary)
            .MaximumLength(1000).WithMessage("Summary cannot exceed 1000 characters.")
            .Custom((summary, context) =>
            {
                SensitiveDataInspector.EnsureSafe(summary, nameof(summary));
            });

        RuleFor(x => x.DocumentLocationHint)
            .MaximumLength(255).WithMessage("Document location hint cannot exceed 255 characters.")
            .Custom((hint, context) =>
            {
                SensitiveDataInspector.EnsureSafe(hint, nameof(hint));
            });

        RuleFor(x => x.DigitalStorageLink)
            .MaximumLength(500).WithMessage("Digital storage link cannot exceed 500 characters.");

        RuleFor(x => x.Urgency)
            .IsInEnum().WithMessage("Invalid urgency stage.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority level.");
    }
}
