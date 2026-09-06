using Asseta.Domain.Common;
using Asseta.Domain.Enums;
using Asseta.Domain.Events;

namespace Asseta.Domain.Entities;

public class ActivationRequest : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public ActivationTriggerSource TriggerSource { get; private set; }
    public Guid? InitiatedByTrustedPersonId { get; private set; }
    public string? Reason { get; private set; }
    public ActivationRequestStatus Status { get; private set; }
    public DateTime GracePeriodExpiresAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public DateTime? ActivatedAtUtc { get; private set; }
    public int RowVersion { get; private set; } = 1;

    private readonly List<ActivationConfirmation> _confirmations = new();
    public IReadOnlyCollection<ActivationConfirmation> Confirmations => _confirmations.AsReadOnly();

    private ActivationRequest() { }

    public ActivationRequest(
        Guid ownerId,
        ActivationTriggerSource triggerSource,
        int gracePeriodHours,
        Guid? initiatedByTrustedPersonId = null,
        string? reason = null)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        TriggerSource = triggerSource;
        InitiatedByTrustedPersonId = initiatedByTrustedPersonId;
        Reason = reason;
        Status = ActivationRequestStatus.PendingGracePeriod;
        GracePeriodExpiresAtUtc = DateTime.UtcNow.AddHours(gracePeriodHours);
        RowVersion = 1;

        if (initiatedByTrustedPersonId.HasValue)
        {
            _confirmations.Add(new ActivationConfirmation(Id, initiatedByTrustedPersonId.Value, true, "Yêu cầu kích hoạt ban đầu"));
        }

        AddDomainEvent(new ActivationRequestInitiatedEvent(
            Id,
            OwnerId,
            TriggerSource,
            InitiatedByTrustedPersonId,
            GracePeriodExpiresAtUtc,
            DateTime.UtcNow));
    }

    public ActivationConfirmation AddConfirmation(Guid trustedPersonId, bool isConfirmed, string? note = null)
    {
        if (Status != ActivationRequestStatus.PendingGracePeriod)
            throw new InvalidOperationException("Không thể bỏ phiếu xác nhận khi yêu cầu không ở trạng thái đệm an toàn.");

        var existing = _confirmations.FirstOrDefault(c => c.TrustedPersonId == trustedPersonId);
        if (existing != null)
        {
            existing.UpdateVote(isConfirmed, note);
            RowVersion++;
            AddDomainEvent(new ActivationConfirmationRecordedEvent(Id, trustedPersonId, isConfirmed, DateTime.UtcNow));
            return existing;
        }

        var confirmation = new ActivationConfirmation(Id, trustedPersonId, isConfirmed, note);
        _confirmations.Add(confirmation);
        RowVersion++;

        AddDomainEvent(new ActivationConfirmationRecordedEvent(
            Id,
            trustedPersonId,
            isConfirmed,
            DateTime.UtcNow));

        return confirmation;
    }

    public void CancelByOwner()
    {
        if (Status != ActivationRequestStatus.PendingGracePeriod && Status != ActivationRequestStatus.Activated)
            throw new InvalidOperationException("Chỉ có thể hủy yêu cầu kích hoạt đang trong thời gian đệm an toàn hoặc đang mở.");

        Status = ActivationRequestStatus.CancelledByOwner;
        CancelledAtUtc = DateTime.UtcNow;
        RowVersion++;

        AddDomainEvent(new ActivationRequestCancelledEvent(Id, OwnerId, DateTime.UtcNow));
    }

    public void ActivateEmergency()
    {
        if (Status != ActivationRequestStatus.PendingGracePeriod)
            throw new InvalidOperationException("Chỉ có thể kích hoạt khi yêu cầu đang trong thời gian đệm.");

        Status = ActivationRequestStatus.Activated;
        ActivatedAtUtc = DateTime.UtcNow;
        RowVersion++;

        AddDomainEvent(new PlanActivatedEmergencyEvent(Id, OwnerId, DateTime.UtcNow));
    }

    public void Reject()
    {
        Status = ActivationRequestStatus.Rejected;
        RowVersion++;
    }
}
