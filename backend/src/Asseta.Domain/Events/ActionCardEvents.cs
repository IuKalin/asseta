using Asseta.Domain.Common;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Events;

public record ActionCardCreatedEvent(
    Guid CardId,
    Guid OwnerId,
    Guid CategoryId,
    Guid? ContinuityItemId,
    string Title,
    UrgencyStage Urgency,
    PriorityLevel Priority,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ActionCardUpdatedEvent(
    Guid CardId,
    Guid OwnerId,
    string Title,
    UrgencyStage Urgency,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ActionCardCompletedEvent(
    Guid CardId,
    Guid? ContinuityItemId,
    Guid CategoryId,
    Guid OwnerId,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ActionCardDeletedEvent(
    Guid CardId,
    Guid OwnerId,
    DateTime OccurredOnUtc) : IDomainEvent;
