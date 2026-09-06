using FluentValidation;

namespace Asseta.Application.Features.TrustedPeople.Commands.UpdateScopedPermissions;

public class UpdateScopedPermissionsCommandValidator : AbstractValidator<UpdateScopedPermissionsCommand>
{
    public UpdateScopedPermissionsCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Mã người ủy thác là bắt buộc.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Mã chủ tài sản là bắt buộc.");
    }
}
