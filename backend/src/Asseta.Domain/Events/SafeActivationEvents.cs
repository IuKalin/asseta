using Asseta.Domain.Common;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Events;

public record VitalityCheckInRecordedEvent(
    Guid OwnerId,
    DateTime CheckedInAtUtc,
    DateTime NextCheckInDueUtc,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ActivationConfigUpdatedEvent(
    Guid OwnerId,
    int CheckInIntervalDays,
    int GracePeriodHours,
    int MinConfirmationsRequired,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ActivationRequestInitiatedEvent(
    Guid ActivationRequestId,
    Guid OwnerId,
    ActivationTriggerSource TriggerSource,
    Guid? InitiatedByTrustedPersonId,
    DateTime GracePeriodExpiresAtUtc,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ActivationRequestCancelledEvent(
    Guid ActivationRequestId,
    Guid OwnerId,
    DateTime OccurredOnUtc) : IDomainEvent;

public record ActivationConfirmationRecordedEvent(
    Guid ActivationRequestId,
    Guid TrustedPersonId,
    bool IsConfirmed,
    DateTime OccurredOnUtc) : IDomainEvent;

public record PlanActivatedEmergencyEvent(
    Guid ActivationRequestId,
    Guid OwnerId,
    DateTime OccurredOnUtc) : IDomainEvent;

public record PlanEmergencyDeactivatedEvent(
    Guid OwnerId,
    DateTime OccurredOnUtc) : IDomainEvent;
