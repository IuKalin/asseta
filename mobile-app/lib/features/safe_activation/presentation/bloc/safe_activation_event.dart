import 'package:equatable/equatable.dart';
import '../../domain/entities/safe_activation_entity.dart';

abstract class SafeActivationEvent extends Equatable {
  const SafeActivationEvent();

  @override
  List<Object?> get props => [];
}

class FetchSafeActivationStatusEvent extends SafeActivationEvent {
  final String? targetOwnerId;

  const FetchSafeActivationStatusEvent({this.targetOwnerId});

  @override
  List<Object?> get props => [targetOwnerId];
}

class VitalityCheckInEvent extends SafeActivationEvent {
  const VitalityCheckInEvent();
}

class UpdateSafeActivationConfigEvent extends SafeActivationEvent {
  final ActivationConfigEntity config;

  const UpdateSafeActivationConfigEvent(this.config);

  @override
  List<Object?> get props => [config];
}

class InitiateSafeActivationRequestEvent extends SafeActivationEvent {
  final String targetOwnerId;
  final String? reason;

  const InitiateSafeActivationRequestEvent({
    required this.targetOwnerId,
    this.reason,
  });

  @override
  List<Object?> get props => [targetOwnerId, reason];
}

class CancelSafeActivationRequestEvent extends SafeActivationEvent {
  final String requestId;

  const CancelSafeActivationRequestEvent(this.requestId);

  @override
  List<Object?> get props => [requestId];
}

class ConfirmSafeActivationRequestEvent extends SafeActivationEvent {
  final String requestId;
  final bool isConfirmed;
  final String? note;

  const ConfirmSafeActivationRequestEvent({
    required this.requestId,
    required this.isConfirmed,
    this.note,
  });

  @override
  List<Object?> get props => [requestId, isConfirmed, note];
}

class DeactivateEmergencyPlanEvent extends SafeActivationEvent {
  const DeactivateEmergencyPlanEvent();
}

class CountdownTickEvent extends SafeActivationEvent {
  final int remainingSeconds;

  const CountdownTickEvent(this.remainingSeconds);

  @override
  List<Object?> get props => [remainingSeconds];
}
