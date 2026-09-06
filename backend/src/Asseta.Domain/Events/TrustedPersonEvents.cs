using Asseta.Domain.Common;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Events;

public record TrustedPersonCreatedEvent(
    Guid TrustedPersonId,
    Guid OwnerId,
    string FullName,
    int TrustLevel,
    DateTime OccurredOnUtc) : IDomainEvent;

public record TrustedPersonPairedEvent(
    Guid TrustedPersonId,
    Guid OwnerId,
    Guid DelegateUserId,
    DateTime OccurredOnUtc) : IDomainEvent;

public record TrustedPersonUpdatedEvent(
    Guid TrustedPersonId,
    Guid OwnerId,
    string FullName,
    int TrustLevel,
    DateTime OccurredOnUtc) : IDomainEvent;

public record TrustedPersonRevokedEvent(
    Guid TrustedPersonId,
    Guid OwnerId,
    DateTime OccurredOnUtc) : IDomainEvent;

public record TrustedPersonPermissionsUpdatedEvent(
    Guid TrustedPersonId,
    Guid OwnerId,
    DateTime OccurredOnUtc) : IDomainEvent;
