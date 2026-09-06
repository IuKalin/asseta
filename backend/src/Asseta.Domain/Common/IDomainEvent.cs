namespace Asseta.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
