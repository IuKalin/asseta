using FluentValidation;

namespace Asseta.Application.Features.SafeActivation.Commands.UpdateActivationConfig;

public class UpdateActivationConfigCommandValidator : AbstractValidator<UpdateActivationConfigCommand>
{
    public UpdateActivationConfigCommandValidator()
    {
        RuleFor(x => x.CheckInIntervalDays)
            .InclusiveBetween(15, 90)
            .WithMessage("Chu kỳ điểm danh phải từ 15 đến 90 ngày.");

        RuleFor(x => x.GracePeriodHours)
            .InclusiveBetween(24, 168)
            .WithMessage("Thời gian đệm an toàn phải từ 24 đến 168 giờ (1 đến 7 ngày).");

        RuleFor(x => x.MinConfirmationsRequired)
            .InclusiveBetween(1, 5)
            .WithMessage("Số lượng xác nhận tối thiểu phải từ 1 đến 5 người.");
    }
}
