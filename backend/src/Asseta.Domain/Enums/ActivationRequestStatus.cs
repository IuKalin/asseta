namespace Asseta.Domain.Enums;

public enum ActivationRequestStatus
{
    PendingGracePeriod = 0,
    CancelledByOwner = 1,
    Activated = 2,
    Rejected = 3
}
