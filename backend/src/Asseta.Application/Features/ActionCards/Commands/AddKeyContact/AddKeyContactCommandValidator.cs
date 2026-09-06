using Asseta.Application.Common.Security;
using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.AddKeyContact;

public class AddKeyContactCommandValidator : AbstractValidator<AddKeyContactCommand>
{
    public AddKeyContactCommandValidator()
    {
        RuleFor(x => x.ActionCardId)
            .NotEmpty().WithMessage("ActionCardId is required.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("ContactName is required.")
            .MaximumLength(150).WithMessage("ContactName cannot exceed 150 characters.")
            .Custom((name, context) =>
            {
                SensitiveDataInspector.EnsureSafe(name, nameof(name));
            });

        RuleFor(x => x.RelationshipOrRole)
            .NotEmpty().WithMessage("RelationshipOrRole is required.")
            .MaximumLength(100).WithMessage("RelationshipOrRole cannot exceed 100 characters.")
            .Custom((role, context) =>
            {
                SensitiveDataInspector.EnsureSafe(role, nameof(role));
            });

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30).WithMessage("PhoneNumber cannot exceed 30 characters.");

        RuleFor(x => x.Email)
            .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.")
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("Invalid email format.");

        RuleFor(x => x.ContactNotes)
            .MaximumLength(255).WithMessage("ContactNotes cannot exceed 255 characters.")
            .Custom((notes, context) =>
            {
                SensitiveDataInspector.EnsureSafe(notes, nameof(notes));
            });
    }
}
