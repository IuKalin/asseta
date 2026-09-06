using Asseta.Domain.Common;
using Asseta.Domain.Enums;
using Asseta.Domain.Events;

namespace Asseta.Domain.Entities;

public class OwnerActivationConfig : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public int CheckInIntervalDays { get; private set; } = 30;
    public int GracePeriodHours { get; private set; } = 48;
    public int MinConfirmationsRequired { get; private set; } = 1;
    public DateTime LastCheckInAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime NextCheckInDueUtc { get; private set; }
    public HeartbeatStatus Status { get; private set; } = HeartbeatStatus.Active;
    public int RowVersion { get; private set; } = 1;

    private OwnerActivationConfig() { }

    public OwnerActivationConfig(
        Guid ownerId,
        int checkInIntervalDays = 30,
        int gracePeriodHours = 48,
        int minConfirmationsRequired = 1)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        CheckInIntervalDays = checkInIntervalDays > 0 ? checkInIntervalDays : 30;
        GracePeriodHours = gracePeriodHours > 0 ? gracePeriodHours : 48;
        MinConfirmationsRequired = minConfirmationsRequired > 0 ? minConfirmationsRequired : 1;
        LastCheckInAtUtc = DateTime.UtcNow;
        NextCheckInDueUtc = LastCheckInAtUtc.AddDays(CheckInIntervalDays);
        Status = HeartbeatStatus.Active;
        RowVersion = 1;
    }

    public static OwnerActivationConfig CreateDefault(Guid ownerId)
    {
        return new OwnerActivationConfig(ownerId, 30, 48, 1);
    }

    public void RecordCheckIn()
    {
        LastCheckInAtUtc = DateTime.UtcNow;
        NextCheckInDueUtc = LastCheckInAtUtc.AddDays(CheckInIntervalDays);
        Status = HeartbeatStatus.Active;
        RowVersion++;

        AddDomainEvent(new VitalityCheckInRecordedEvent(
            OwnerId,
            LastCheckInAtUtc,
            NextCheckInDueUtc,
            DateTime.UtcNow));
    }

    public void UpdateConfig(int checkInIntervalDays, int gracePeriodHours, int minConfirmationsRequired)
    {
        if (checkInIntervalDays < 15 || checkInIntervalDays > 90)
            throw new ArgumentOutOfRangeException(nameof(checkInIntervalDays), "Check-in interval must be between 15 and 90 days.");

        if (gracePeriodHours < 24 || gracePeriodHours > 168)
            throw new ArgumentOutOfRangeException(nameof(gracePeriodHours), "Grace period must be between 24 and 168 hours.");

        if (minConfirmationsRequired < 1 || minConfirmationsRequired > 5)
            throw new ArgumentOutOfRangeException(nameof(minConfirmationsRequired), "Minimum confirmations must be between 1 and 5.");

        CheckInIntervalDays = checkInIntervalDays;
        GracePeriodHours = gracePeriodHours;
        MinConfirmationsRequired = minConfirmationsRequired;
        NextCheckInDueUtc = LastCheckInAtUtc.AddDays(CheckInIntervalDays);
        RowVersion++;

        AddDomainEvent(new ActivationConfigUpdatedEvent(
            OwnerId,
            CheckInIntervalDays,
            GracePeriodHours,
            MinConfirmationsRequired,
            DateTime.UtcNow));
    }

    public void SetStatus(HeartbeatStatus newStatus)
    {
        Status = newStatus;
        RowVersion++;
    }
}
