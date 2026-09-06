using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class ActivationConfirmation : BaseEntity
{
    public Guid ActivationRequestId { get; private set; }
    public Guid TrustedPersonId { get; private set; }
    public bool IsConfirmed { get; private set; }
    public string? Note { get; private set; }
    public DateTime ConfirmedAtUtc { get; private set; }

    private ActivationConfirmation() { }

    public ActivationConfirmation(
        Guid activationRequestId,
        Guid trustedPersonId,
        bool isConfirmed,
        string? note = null)
    {
        Id = Guid.NewGuid();
        ActivationRequestId = activationRequestId;
        TrustedPersonId = trustedPersonId;
        IsConfirmed = isConfirmed;
        Note = note;
        ConfirmedAtUtc = DateTime.UtcNow;
    }

    public void UpdateVote(bool isConfirmed, string? note = null)
    {
        IsConfirmed = isConfirmed;
        Note = note;
        ConfirmedAtUtc = DateTime.UtcNow;
    }
}
