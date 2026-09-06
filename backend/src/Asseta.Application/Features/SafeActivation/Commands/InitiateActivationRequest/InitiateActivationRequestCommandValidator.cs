using FluentValidation;

namespace Asseta.Application.Features.SafeActivation.Commands.InitiateActivationRequest;

public class InitiateActivationRequestCommandValidator : AbstractValidator<InitiateActivationRequestCommand>
{
    public InitiateActivationRequestCommandValidator()
    {
        RuleFor(x => x.TargetOwnerId)
            .NotEmpty()
            .WithMessage("Mã chủ tài sản không được để trống.");

        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .WithMessage("Lý do kích hoạt không được vượt quá 1000 ký tự.");
    }
}
