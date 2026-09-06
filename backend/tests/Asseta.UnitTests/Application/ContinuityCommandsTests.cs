using Asseta.Application.Common.Exceptions;
using Asseta.Application.Features.ContinuityMap.Commands.CreateContinuityItem;
using Asseta.Application.Features.ContinuityMap.Commands.UpdateContinuityItem;
using Asseta.Domain.Enums;
using Xunit;

namespace Asseta.UnitTests.Application;

public class ContinuityCommandsTests
{
    [Fact]
    public void CreateCommandValidator_ValidCommand_ShouldPass()
    {
        var validator = new CreateContinuityItemCommandValidator();
        var command = new CreateContinuityItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Khoản vay thế chấp Vietcombank",
            PriorityLevel.CRITICAL,
            "Tủ hồ sơ tầng 2",
            Guid.NewGuid(),
            null,
            null,
            null);

        var result = validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateCommandValidator_EmptyName_ShouldFail()
    {
        var validator = new CreateContinuityItemCommandValidator();
        var command = new CreateContinuityItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            PriorityLevel.CRITICAL,
            null,
            null,
            null,
            null,
            null);

        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void CreateCommandValidator_CreditCardInPlaintext_ShouldThrowSensitiveDataDetectedException()
    {
        var validator = new CreateContinuityItemCommandValidator();
        var command = new CreateContinuityItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Thẻ tín dụng số 4111 2222 3333 4444", // Credit Card number
            PriorityLevel.IMPORTANT,
            null,
            null,
            null,
            null,
            null);

        Assert.Throws<SensitiveDataDetectedException>(() => validator.Validate(command));
    }

    [Fact]
    public void UpdateCommandValidator_ValidCommand_ShouldPass()
    {
        var validator = new UpdateContinuityItemCommandValidator();
        var command = new UpdateContinuityItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Hợp đồng bảo hiểm AIA",
            PriorityLevel.CRITICAL,
            "Hộc bàn",
            Guid.NewGuid(),
            null,
            null,
            null,
            1);

        var result = validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateCommandValidator_InvalidRowVersion_ShouldFail()
    {
        var validator = new UpdateContinuityItemCommandValidator();
        var command = new UpdateContinuityItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Hợp đồng",
            PriorityLevel.CRITICAL,
            null,
            null,
            null,
            null,
            null,
            0); // Invalid RowVersion

        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RowVersion");
    }
}
