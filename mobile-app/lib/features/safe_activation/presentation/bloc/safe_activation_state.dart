import 'package:equatable/equatable.dart';
import '../../domain/entities/safe_activation_entity.dart';

abstract class SafeActivationState extends Equatable {
  const SafeActivationState();

  @override
  List<Object?> get props => [];
}

class SafeActivationInitial extends SafeActivationState {}

class SafeActivationLoading extends SafeActivationState {}

class SafeActivationLoaded extends SafeActivationState {
  final ActivationStatusEntity status;
  final int countdownSeconds;
  final String? successMessage;
  final bool isActionInProgress;

  const SafeActivationLoaded({
    required this.status,
    this.countdownSeconds = 0,
    this.successMessage,
    this.isActionInProgress = false,
  });

  SafeActivationLoaded copyWith({
    ActivationStatusEntity? status,
    int? countdownSeconds,
    String? successMessage,
    bool? isActionInProgress,
  }) {
    return SafeActivationLoaded(
      status: status ?? this.status,
      countdownSeconds: countdownSeconds ?? this.countdownSeconds,
      successMessage: successMessage,
      isActionInProgress: isActionInProgress ?? this.isActionInProgress,
    );
  }

  @override
  List<Object?> get props => [status, countdownSeconds, successMessage, isActionInProgress];
}

class SafeActivationError extends SafeActivationState {
  final String message;

  const SafeActivationError(this.message);

  @override
  List<Object?> get props => [message];
}
