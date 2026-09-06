using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteKeyContact;

public class DeleteKeyContactCommandValidator : AbstractValidator<DeleteKeyContactCommand>
{
    public DeleteKeyContactCommandValidator()
    {
        RuleFor(x => x.ContactId)
            .NotEmpty().WithMessage("ContactId is required.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");
    }
}
