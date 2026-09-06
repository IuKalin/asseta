using FluentValidation;

namespace Asseta.Application.Features.TrustedPeople.Commands.ClaimPairingCode;

public class ClaimPairingCodeCommandValidator : AbstractValidator<ClaimPairingCodeCommand>
{
    public ClaimPairingCodeCommandValidator()
    {
        RuleFor(x => x.DelegateUserId)
            .NotEmpty().WithMessage("Mã định danh người ủy thác là bắt buộc.");

        RuleFor(x => x.PairingCode)
            .NotEmpty().WithMessage("Mã ghép đôi không được để trống.")
            .Length(6).WithMessage("Mã ghép đôi phải có độ dài chính xác 6 ký tự.")
            .Matches(@"^[A-Za-z0-9]{6}$").WithMessage("Mã ghép đôi chỉ chứa ký tự chữ và số.");
    }
}
