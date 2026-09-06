using Asseta.Domain.Common;
using Asseta.Domain.Enums;
using Asseta.Domain.ValueObjects;

namespace Asseta.Domain.Entities;

public class ActionCard : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? ContinuityItemId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Summary { get; private set; }
    public UrgencyStage Urgency { get; private set; }
    public PriorityLevel Priority { get; private set; }
    public Guid? AssignedTrustedPersonId { get; private set; }
    public string? DocumentLocationHint { get; private set; }
    public string? DigitalStorageLink { get; private set; }
    public CipherBlobPayload? CipherInstructions { get; private set; }
    public bool IsCompleted { get; private set; }
    public int RowVersion { get; private set; } = 1;

    public ContinuityCategory? Category { get; private set; }
    public ContinuityItem? ContinuityItem { get; private set; }

    private readonly List<ActionCardStep> _steps = new();
    public IReadOnlyCollection<ActionCardStep> Steps => _steps.AsReadOnly();

    private readonly List<ActionCardContact> _contacts = new();
    public IReadOnlyCollection<ActionCardContact> Contacts => _contacts.AsReadOnly();

    private ActionCard() { }

    public ActionCard(
        Guid id,
        Guid ownerId,
        Guid categoryId,
        string title,
        UrgencyStage urgency,
        PriorityLevel priority,
        Guid? continuityItemId = null,
        string? summary = null,
        Guid? assignedTrustedPersonId = null,
        string? documentLocationHint = null,
        string? digitalStorageLink = null,
        CipherBlobPayload? cipherInstructions = null)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("OwnerId cannot be empty.", nameof(ownerId));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        OwnerId = ownerId;
        CategoryId = categoryId;
        ContinuityItemId = continuityItemId;
        Title = title.Trim();
        Summary = summary?.Trim();
        Urgency = urgency;
        Priority = priority;
        AssignedTrustedPersonId = assignedTrustedPersonId;
        DocumentLocationHint = documentLocationHint?.Trim();
        DigitalStorageLink = digitalStorageLink?.Trim();
        CipherInstructions = cipherInstructions;
        IsCompleted = false;
        RowVersion = 1;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
        string title,
        UrgencyStage urgency,
        PriorityLevel priority,
        string? summary,
        Guid? assignedTrustedPersonId,
        string? documentLocationHint,
        string? digitalStorageLink,
        CipherBlobPayload? cipherInstructions)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

        Title = title.Trim();
        Urgency = urgency;
        Priority = priority;
        Summary = summary?.Trim();
        AssignedTrustedPersonId = assignedTrustedPersonId;
        DocumentLocationHint = documentLocationHint?.Trim();
        DigitalStorageLink = digitalStorageLink?.Trim();
        CipherInstructions = cipherInstructions;

        EvaluateCompletion();
        RowVersion++;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UnassignTrustedPerson()
    {
        AssignedTrustedPersonId = null;
        EvaluateCompletion();
        RowVersion++;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public ActionCardStep AddStep(string instruction, string? estimatedDuration = null)
    {
        if (_steps.Count(s => !s.IsDeleted) >= 20)
            throw new InvalidOperationException("An Action Card cannot exceed 20 active steps.");

        int nextOrder = _steps.Count(s => !s.IsDeleted) + 1;
        var step = new ActionCardStep(Guid.NewGuid(), Id, nextOrder, instruction, estimatedDuration);
        _steps.Add(step);

        EvaluateCompletion();
        UpdatedAtUtc = DateTime.UtcNow;
        return step;
    }

    public void RemoveStep(Guid stepId)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId && !s.IsDeleted);
        if (step != null)
        {
            step.MarkDeleted();
            RenumberActiveSteps();
            EvaluateCompletion();
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    public void ReorderSteps(IReadOnlyList<Guid> orderedStepIds)
    {
        var activeSteps = _steps.Where(s => !s.IsDeleted).ToList();
        int newOrder = 1;
        foreach (var id in orderedStepIds)
        {
            var step = activeSteps.FirstOrDefault(s => s.Id == id);
            if (step != null)
            {
                step.SetStepOrder(newOrder++);
            }
        }
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public ActionCardContact AddContact(
        string contactName,
        string relationshipOrRole,
        string? phoneNumber = null,
        string? email = null,
        string? contactNotes = null)
    {
        if (_contacts.Count(c => !c.IsDeleted) >= 5)
            throw new InvalidOperationException("An Action Card cannot exceed 5 active key contacts.");

        var contact = new ActionCardContact(
            Guid.NewGuid(),
            Id,
            contactName,
            relationshipOrRole,
            phoneNumber,
            email,
            contactNotes);
        _contacts.Add(contact);

        UpdatedAtUtc = DateTime.UtcNow;
        return contact;
    }

    public void RemoveContact(Guid contactId)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId && !c.IsDeleted);
        if (contact != null)
        {
            contact.MarkDeleted();
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    public void LinkContinuityItem(Guid continuityItemId)
    {
        ContinuityItemId = continuityItemId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void EvaluateCompletion()
    {
        bool hasPerson = AssignedTrustedPersonId.HasValue && AssignedTrustedPersonId.Value != Guid.Empty;
        bool hasLocation = !string.IsNullOrWhiteSpace(DocumentLocationHint);
        bool hasSteps = _steps.Any(s => !s.IsDeleted);

        IsCompleted = hasPerson && hasLocation && hasSteps;
    }

    public void UpdateUrgencyStage(UrgencyStage newStage)
    {
        if (!Enum.IsDefined(typeof(UrgencyStage), newStage))
            throw new ArgumentException("Invalid urgency stage.", nameof(newStage));

        if (Urgency != newStage)
        {
            Urgency = newStage;
            RowVersion++;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    public void ToggleCompletion()
    {
        IsCompleted = !IsCompleted;
        RowVersion++;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private void RenumberActiveSteps()
    {
        int order = 1;
        foreach (var s in _steps.Where(s => !s.IsDeleted).OrderBy(s => s.StepOrder))
        {
            s.SetStepOrder(order++);
        }
    }
}
