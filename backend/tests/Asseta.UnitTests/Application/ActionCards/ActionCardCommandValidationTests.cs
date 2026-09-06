using Asseta.Application.Common.Exceptions;
using Asseta.Application.Features.ActionCards.Commands.AddActionStep;
using Asseta.Application.Features.ActionCards.Commands.AddKeyContact;
using Asseta.Application.Features.ActionCards.Commands.CreateActionCard;
using Asseta.Application.Features.ActionCards.Commands.CreateActionCardFromItem;
using Asseta.Application.Features.ActionCards.Commands.UpdateActionCard;
using Asseta.Domain.Enums;
using Xunit;

namespace Asseta.UnitTests.Application.ActionCards;

public class ActionCardCommandValidationTests
{
    [Fact]
    public void CreateActionCardCommandValidator_ValidCommand_ShouldPass()
    {
        var validator = new CreateActionCardCommandValidator();
        var command = new CreateActionCardCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Xử lý khoản vay ngân hàng Agribank",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL,
            "Cần thông báo cán bộ tín dụng",
            Guid.NewGuid(),
            "Ngăn kéo phòng ngủ",
            "https://drive.google.com/item");

        var result = validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateActionCardCommandValidator_EmptyTitle_ShouldFail()
    {
        var validator = new CreateActionCardCommandValidator();
        var command = new CreateActionCardCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            UrgencyStage.FIRST_72_HOURS,
            PriorityLevel.CRITICAL);

        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void CreateActionCardCommandValidator_CreditCardNumber_ShouldThrowSensitiveDataDetectedException()
    {
        var validator = new CreateActionCardCommandValidator();
        var command = new CreateActionCardCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Thẻ thanh toán 4532 0150 1234 5678", // Credit card pattern
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL);

        Assert.Throws<SensitiveDataDetectedException>(() => validator.Validate(command));
    }

    [Fact]
    public void CreateActionCardCommandValidator_CryptoPrivateKey_ShouldThrowSensitiveDataDetectedException()
    {
        var validator = new CreateActionCardCommandValidator();
        var command = new CreateActionCardCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Private key ví: 0x4f3edf983ac636a65a842ce7c78d9aa706d3b113bce9c46f30d7d21715b23b1d",
            UrgencyStage.IMMEDIATE,
            PriorityLevel.CRITICAL);

        Assert.Throws<SensitiveDataDetectedException>(() => validator.Validate(command));
    }

    [Fact]
    public void UpdateActionCardCommandValidator_InvalidRowVersion_ShouldFail()
    {
        var validator = new UpdateActionCardCommandValidator();
        var command = new UpdateActionCardCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Cập nhật",
            UrgencyStage.FIRST_7_DAYS,
            PriorityLevel.IMPORTANT,
            0); // Invalid RowVersion

        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RowVersion");
    }

    [Fact]
    public void AddActionStepCommandValidator_EmptyInstruction_ShouldFail()
    {
        var validator = new AddActionStepCommandValidator();
        var command = new AddActionStepCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "");

        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Instruction");
    }

    [Fact]
    public void AddActionStepCommandValidator_InstructionExceeds500Chars_ShouldFail()
    {
        var validator = new AddActionStepCommandValidator();
        var command = new AddActionStepCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new string('A', 501));

        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Instruction");
    }

    [Fact]
    public void AddKeyContactCommandValidator_InvalidEmail_ShouldFail()
    {
        var validator = new AddKeyContactCommandValidator();
        var command = new AddKeyContactCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Nguyễn Văn A",
            "Luật sư",
            "0901234567",
            "not-an-email");

        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }
}
