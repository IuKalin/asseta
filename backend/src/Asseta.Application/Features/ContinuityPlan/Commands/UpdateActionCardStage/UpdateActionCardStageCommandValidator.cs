using FluentValidation;

namespace Asseta.Application.Features.ContinuityPlan.Commands.UpdateActionCardStage;

public class UpdateActionCardStageCommandValidator : AbstractValidator<UpdateActionCardStageCommand>
{
    public UpdateActionCardStageCommandValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty().WithMessage("CardId cannot be empty.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId cannot be empty.");

        RuleFor(x => x.NewStage)
            .IsInEnum().WithMessage("Invalid urgency stage.");

        RuleFor(x => x.RowVersion)
            .GreaterThan(0).WithMessage("RowVersion must be greater than 0.");
    }
}
