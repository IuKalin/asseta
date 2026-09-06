using FluentValidation;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteActionCard;

public class DeleteActionCardCommandValidator : AbstractValidator<DeleteActionCardCommand>
{
    public DeleteActionCardCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Card Id is required.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");
    }
}
