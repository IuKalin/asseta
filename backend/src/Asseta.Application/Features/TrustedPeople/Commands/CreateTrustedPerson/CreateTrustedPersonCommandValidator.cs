using FluentValidation;

namespace Asseta.Application.Features.TrustedPeople.Commands.CreateTrustedPerson;

public class CreateTrustedPersonCommandValidator : AbstractValidator<CreateTrustedPersonCommand>
{
    public CreateTrustedPersonCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Mã chủ tài sản (OwnerId) là bắt buộc.");

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

        RuleFor(x => x.RoleDescription)
            .MaximumLength(255).WithMessage("Mô tả vai trò không được vượt quá 255 ký tự.");
    }
}
