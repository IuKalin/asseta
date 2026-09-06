using Asseta.Domain.Common;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Events;

public record ContinuityItemCreatedEvent(
    Guid ItemId,
    Guid OwnerId,
    Guid CategoryId,
    string ItemName,
    PriorityLevel Priority,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ContinuityItemUpdatedEvent(
    Guid ItemId,
    Guid OwnerId,
    Guid CategoryId,
    string ItemName,
    PriorityLevel Priority,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ContinuityItemDeletedEvent(
    Guid ItemId,
    Guid OwnerId,
    Guid CategoryId,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ReadinessScoreRecalculatedEvent(
    Guid OwnerId,
    Guid? CategoryId,
    int NewScore,
    DateTime OccurredOnUtc) : IDomainEvent;
