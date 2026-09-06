using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class ContinuityAuditLog : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public Guid? ItemId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string PayloadSnapshot { get; private set; } = string.Empty;
    public string? IpAddress { get; private set; }
    public Guid CorrelationId { get; private set; }

    private ContinuityAuditLog() { }

    public ContinuityAuditLog(
        Guid ownerId,
        Guid? itemId,
        string action,
        string payloadSnapshot,
        string? ipAddress,
        Guid correlationId)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("OwnerId cannot be empty.", nameof(ownerId));
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty.", nameof(action));

        OwnerId = ownerId;
        ItemId = itemId;
        Action = action.Trim().ToUpperInvariant();
        PayloadSnapshot = payloadSnapshot ?? "{}";
        IpAddress = ipAddress;
        CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
