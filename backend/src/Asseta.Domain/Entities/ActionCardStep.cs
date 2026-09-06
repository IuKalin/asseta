using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class ActionCardStep : BaseEntity
{
    public Guid ActionCardId { get; private set; }
    public int StepOrder { get; private set; }
    public string Instruction { get; private set; } = string.Empty;
    public string? EstimatedDuration { get; private set; }
    public bool IsCompleted { get; private set; }

    public ActionCard? ActionCard { get; private set; }

    private ActionCardStep() { }

    public ActionCardStep(Guid id, Guid actionCardId, int stepOrder, string instruction, string? estimatedDuration = null)
    {
        if (string.IsNullOrWhiteSpace(instruction))
            throw new ArgumentException("Instruction cannot be empty.", nameof(instruction));
        if (instruction.Length > 500)
            throw new ArgumentException("Instruction cannot exceed 500 characters.", nameof(instruction));
        if (stepOrder < 1)
            throw new ArgumentOutOfRangeException(nameof(stepOrder), "StepOrder must be >= 1.");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        ActionCardId = actionCardId;
        StepOrder = stepOrder;
        Instruction = instruction.Trim();
        EstimatedDuration = estimatedDuration?.Trim();
        IsCompleted = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(string instruction, string? estimatedDuration)
    {
        if (string.IsNullOrWhiteSpace(instruction))
            throw new ArgumentException("Instruction cannot be empty.", nameof(instruction));
        if (instruction.Length > 500)
            throw new ArgumentException("Instruction cannot exceed 500 characters.", nameof(instruction));

        Instruction = instruction.Trim();
        EstimatedDuration = estimatedDuration?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetStepOrder(int newOrder)
    {
        if (newOrder < 1)
            throw new ArgumentOutOfRangeException(nameof(newOrder), "StepOrder must be >= 1.");
        StepOrder = newOrder;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ToggleCompleted(bool isCompleted)
    {
        IsCompleted = isCompleted;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
