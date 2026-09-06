using FluentValidation;

namespace Asseta.Application.Features.TrustedPeople.Commands.UpdateTrustedPerson;

public class UpdateTrustedPersonCommandValidator : AbstractValidator<UpdateTrustedPersonCommand>
{
    public UpdateTrustedPersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Mã người ủy thác là bắt buộc.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Mã chủ tài sản là bắt buộc.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên người ủy thác không được để trống.")
            .MaximumLength(150).WithMessage("Họ và tên không được vượt quá 150 ký tự.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Định dạng email không hợp lệ.")
            .MaximumLength(255).WithMessage("Email không được vượt quá 255 ký tự.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .Matches(@"^[+0-9\s-]{8,20}$").WithMessage("Định dạng số điện thoại không hợp lệ.")
            .MaximumLength(50).WithMessage("Số điện thoại không được vượt quá 50 ký tự.");

        RuleFor(x => x.Relationship)
            .NotEmpty().WithMessage("Mối quan hệ không được để trống.")
            .MaximumLength(100).WithMessage("Mối quan hệ không được vượt quá 100 ký tự.");

        RuleFor(x => x.TrustLevel)
            .InclusiveBetween(1, 3).WithMessage("Cấp bậc tin cậy (TrustLevel) phải từ 1 đến 3.");

        RuleFor(x => x.ExpectedRowVersion)
            .GreaterThanOrEqualTo(1).WithMessage("ExpectedRowVersion không hợp lệ.");
    }
}
