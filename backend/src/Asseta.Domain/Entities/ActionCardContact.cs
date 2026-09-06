using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class ActionCardContact : BaseEntity
{
    public Guid ActionCardId { get; private set; }
    public string ContactName { get; private set; } = string.Empty;
    public string RelationshipOrRole { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? ContactNotes { get; private set; }

    public ActionCard? ActionCard { get; private set; }

    private ActionCardContact() { }

    public ActionCardContact(
        Guid id,
        Guid actionCardId,
        string contactName,
        string relationshipOrRole,
        string? phoneNumber = null,
        string? email = null,
        string? contactNotes = null)
    {
        if (string.IsNullOrWhiteSpace(contactName))
            throw new ArgumentException("Contact name cannot be empty.", nameof(contactName));
        if (string.IsNullOrWhiteSpace(relationshipOrRole))
            throw new ArgumentException("Relationship or role cannot be empty.", nameof(relationshipOrRole));

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        ActionCardId = actionCardId;
        ContactName = contactName.Trim();
        RelationshipOrRole = relationshipOrRole.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Email = email?.Trim();
        ContactNotes = contactNotes?.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
        string contactName,
        string relationshipOrRole,
        string? phoneNumber,
        string? email,
        string? contactNotes)
    {
        if (string.IsNullOrWhiteSpace(contactName))
            throw new ArgumentException("Contact name cannot be empty.", nameof(contactName));
        if (string.IsNullOrWhiteSpace(relationshipOrRole))
            throw new ArgumentException("Relationship or role cannot be empty.", nameof(relationshipOrRole));

        ContactName = contactName.Trim();
        RelationshipOrRole = relationshipOrRole.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Email = email?.Trim();
        ContactNotes = contactNotes?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
