namespace Asseta.Domain.ValueObjects;

public record ReadinessScore
{
    public int Value { get; }

    public ReadinessScore(int value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "Readiness score must be between 0 and 100.");

        Value = value;
    }

    public static ReadinessScore Zero => new(0);
    public static ReadinessScore Full => new(100);

    public static implicit operator int(ReadinessScore score) => score.Value;
    public static explicit operator ReadinessScore(int value) => new(value);
}
