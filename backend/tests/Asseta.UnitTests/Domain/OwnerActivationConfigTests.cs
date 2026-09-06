using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Xunit;

namespace Asseta.UnitTests.Domain;

public class OwnerActivationConfigTests
{
    [Fact]
    public void CreateDefault_SetsExpectedDefaultValues()
    {
        var ownerId = Guid.NewGuid();
        var config = OwnerActivationConfig.CreateDefault(ownerId);

        Assert.Equal(ownerId, config.OwnerId);
        Assert.Equal(30, config.CheckInIntervalDays);
        Assert.Equal(48, config.GracePeriodHours);
        Assert.Equal(1, config.MinConfirmationsRequired);
        Assert.Equal(HeartbeatStatus.Active, config.Status);
        Assert.Equal(1, config.RowVersion);
        Assert.True(config.NextCheckInDueUtc > DateTime.UtcNow);
    }

    [Fact]
    public void RecordCheckIn_UpdatesTimestampsAndIncrementsRowVersion()
    {
        var config = OwnerActivationConfig.CreateDefault(Guid.NewGuid());
        var initialRowVersion = config.RowVersion;
        var initialCheckIn = config.LastCheckInAtUtc;

        config.RecordCheckIn();

        Assert.True(config.LastCheckInAtUtc >= initialCheckIn);
        Assert.Equal(initialRowVersion + 1, config.RowVersion);
        Assert.Equal(HeartbeatStatus.Active, config.Status);
    }

    [Theory]
    [InlineData(10, 48, 1)] // interval < 15
    [InlineData(100, 48, 1)] // interval > 90
    [InlineData(30, 12, 1)] // grace < 24
    [InlineData(30, 200, 1)] // grace > 168
    [InlineData(30, 48, 0)] // minConfirmations < 1
    [InlineData(30, 48, 6)] // minConfirmations > 5
    public void UpdateConfig_InvalidValues_ThrowsArgumentOutOfRangeException(int interval, int grace, int quorum)
    {
        var config = OwnerActivationConfig.CreateDefault(Guid.NewGuid());

        Assert.Throws<ArgumentOutOfRangeException>(() => config.UpdateConfig(interval, grace, quorum));
    }

    [Fact]
    public void ActivationRequest_Lifecycle_ManagesConfirmationsAndCancellation()
    {
        var ownerId = Guid.NewGuid();
        var delegateId = Guid.NewGuid();

        var request = new ActivationRequest(
            ownerId,
            ActivationTriggerSource.TrustedPersonRequest,
            48,
            delegateId,
            "Chủ tài sản gặp tai nạn");

        Assert.Equal(ownerId, request.OwnerId);
        Assert.Equal(ActivationRequestStatus.PendingGracePeriod, request.Status);
        Assert.Single(request.Confirmations);
        Assert.Equal(1, request.RowVersion);

        // Add second confirmation from another delegate
        var secondDelegateId = Guid.NewGuid();
        request.AddConfirmation(secondDelegateId, true, "Tôi xác nhận thông tin này");
        Assert.Equal(2, request.Confirmations.Count);
        Assert.Equal(2, request.RowVersion);

        // Cancel by owner
        request.CancelByOwner();
        Assert.Equal(ActivationRequestStatus.CancelledByOwner, request.Status);
        Assert.NotNull(request.CancelledAtUtc);
        Assert.Equal(3, request.RowVersion);

        // Attempting to cancel again should throw
        Assert.Throws<InvalidOperationException>(() => request.CancelByOwner());
    }
}
