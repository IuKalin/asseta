using Asseta.Domain.Common;
using Asseta.Domain.Enums;
using Asseta.Domain.ValueObjects;

namespace Asseta.Domain.Entities;

public class ContinuityItem : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public PriorityLevel Priority { get; private set; } = PriorityLevel.IMPORTANT;
    public string? DocumentLocationHint { get; private set; }
    public Guid? AssignedTrustedPersonId { get; private set; }
    public Guid? ActionCardId { get; private set; }

    // Zero-Knowledge Client-Side Encrypted fields
    public string? CipherNotesBlob { get; private set; }
    public string? CipherNonce { get; private set; }
    public string? CipherAuthTag { get; private set; }

    public int SortOrder { get; private set; }
    public bool IsCompleted { get; private set; }
    public int RowVersion { get; private set; } = 1;

    // Navigation
    public virtual ContinuityCategory? Category { get; private set; }

    public bool HasContinuityGap =>
        (Priority == PriorityLevel.CRITICAL || Priority == PriorityLevel.IMPORTANT) &&
        (!AssignedTrustedPersonId.HasValue || string.IsNullOrWhiteSpace(DocumentLocationHint));

    public bool HasConfidentialNotes => !string.IsNullOrWhiteSpace(CipherNotesBlob);

    private ContinuityItem() { }

    public ContinuityItem(
        Guid ownerId,
        Guid categoryId,
        string name,
        PriorityLevel priority,
        string? documentLocationHint = null,
        Guid? assignedTrustedPersonId = null,
        CipherBlobPayload? encryptedNotes = null,
        int sortOrder = 0)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("OwnerId cannot be empty.", nameof(ownerId));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Item name cannot be empty.", nameof(name));

        OwnerId = ownerId;
        CategoryId = categoryId;
        Name = name.Trim();
        Priority = priority;
        DocumentLocationHint = documentLocationHint?.Trim();
        AssignedTrustedPersonId = assignedTrustedPersonId;
        SortOrder = sortOrder;
        CreatedAtUtc = DateTime.UtcNow;
        RowVersion = 1;

        if (encryptedNotes != null)
        {
            SetEncryptedNotes(encryptedNotes);
        }
    }

    public void UpdateDetails(
        string name,
        PriorityLevel priority,
        string? documentLocationHint,
        Guid? assignedTrustedPersonId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Item name cannot be empty.", nameof(name));

        Name = name.Trim();
        Priority = priority;
        DocumentLocationHint = documentLocationHint?.Trim();
        AssignedTrustedPersonId = assignedTrustedPersonId;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    public void UnassignTrustedPerson()
    {
        AssignedTrustedPersonId = null;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    public void SetEncryptedNotes(CipherBlobPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        CipherNotesBlob = payload.CipherBlob;
        CipherNonce = payload.Nonce;
        CipherAuthTag = payload.AuthTag;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ClearEncryptedNotes()
    {
        CipherNotesBlob = null;
        CipherNonce = null;
        CipherAuthTag = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void LinkActionCard(Guid actionCardId)
    {
        if (actionCardId == Guid.Empty)
            throw new ArgumentException("ActionCardId cannot be empty.", nameof(actionCardId));

        ActionCardId = actionCardId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UnlinkActionCard()
    {
        ActionCardId = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetCompletionStatus(bool isCompleted)
    {
        IsCompleted = isCompleted;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        MarkDeleted();
        DeletedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }
}
