namespace Asseta.Application.Features.SafeActivation.DTOs;

public class ActivationStatusDto
{
    public Guid OwnerId { get; set; }
    public int CheckInIntervalDays { get; set; }
    public int GracePeriodHours { get; set; }
    public int MinConfirmationsRequired { get; set; }
    public DateTime LastCheckInAtUtc { get; set; }
    public DateTime NextCheckInDueUtc { get; set; }
    public bool IsDueSoon { get; set; }
    public bool IsOverdue { get; set; }
    public string HeartbeatStatus { get; set; } = "ACTIVE";
    public bool IsEmergencyActive { get; set; }
    public ActivationRequestDto? ActiveRequest { get; set; }
}

public class ActivationConfigDto
{
    public int CheckInIntervalDays { get; set; }
    public int GracePeriodHours { get; set; }
    public int MinConfirmationsRequired { get; set; }
}

public class ActivationRequestDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string TriggerSource { get; set; } = string.Empty;
    public Guid? InitiatedByTrustedPersonId { get; set; }
    public string? InitiatedByTrustedPersonName { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime GracePeriodExpiresAtUtc { get; set; }
    public int RemainingSeconds { get; set; }
    public int ConfirmationsCount { get; set; }
    public int MinConfirmationsRequired { get; set; }
    public int RowVersion { get; set; }
    public List<ActivationConfirmationDto> Confirmations { get; set; } = new();
}

public class ActivationConfirmationDto
{
    public Guid Id { get; set; }
    public Guid TrustedPersonId { get; set; }
    public string? TrustedPersonName { get; set; }
    public bool IsConfirmed { get; set; }
    public string? Note { get; set; }
    public DateTime ConfirmedAtUtc { get; set; }
}
