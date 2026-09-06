import 'dart:async';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/repositories/safe_activation_repository.dart';
import 'safe_activation_event.dart';
import 'safe_activation_state.dart';

class SafeActivationBloc extends Bloc<SafeActivationEvent, SafeActivationState> {
  final SafeActivationRepository repository;
  Timer? _countdownTimer;

  SafeActivationBloc({required this.repository}) : super(SafeActivationInitial()) {
    on<FetchSafeActivationStatusEvent>(_onFetchStatus);
    on<VitalityCheckInEvent>(_onVitalityCheckIn);
    on<UpdateSafeActivationConfigEvent>(_onUpdateConfig);
    on<InitiateSafeActivationRequestEvent>(_onInitiateRequest);
    on<CancelSafeActivationRequestEvent>(_onCancelRequest);
    on<ConfirmSafeActivationRequestEvent>(_onConfirmRequest);
    on<DeactivateEmergencyPlanEvent>(_onDeactivateEmergency);
    on<CountdownTickEvent>(_onCountdownTick);
  }

  Future<void> _onFetchStatus(
    FetchSafeActivationStatusEvent event,
    Emitter<SafeActivationState> emit,
  ) async {
    emit(SafeActivationLoading());
    final result = await repository.getStatus(targetOwnerId: event.targetOwnerId);
    result.fold(
      (failure) => emit(SafeActivationError(failure.message)),
      (status) {
        _startTimerIfNeeded(status.activeRequest?.remainingSeconds ?? 0);
        emit(SafeActivationLoaded(
          status: status,
          countdownSeconds: status.activeRequest?.remainingSeconds ?? 0,
        ));
      },
    );
  }

  Future<void> _onVitalityCheckIn(
    VitalityCheckInEvent event,
    Emitter<SafeActivationState> emit,
  ) async {
    final currentState = state;
    if (currentState is SafeActivationLoaded) {
      emit(currentState.copyWith(isActionInProgress: true));
    }

    final result = await repository.vitalityCheckIn();
    result.fold(
      (failure) => emit(SafeActivationError(failure.message)),
      (status) {
        _stopTimer();
        emit(SafeActivationLoaded(
          status: status,
          countdownSeconds: 0,
          successMessage: 'Điểm danh thành công! Chu kỳ an toàn đã được gia hạn.',
        ));
      },
    );
  }

  Future<void> _onUpdateConfig(
    UpdateSafeActivationConfigEvent event,
    Emitter<SafeActivationState> emit,
  ) async {
    final currentState = state;
    if (currentState is SafeActivationLoaded) {
      emit(currentState.copyWith(isActionInProgress: true));
    }

    final result = await repository.updateConfig(event.config);
    await result.fold(
      (failure) async => emit(SafeActivationError(failure.message)),
      (_) async {
        add(const FetchSafeActivationStatusEvent());
      },
    );
  }

  Future<void> _onInitiateRequest(
    InitiateSafeActivationRequestEvent event,
    Emitter<SafeActivationState> emit,
  ) async {
    final currentState = state;
    if (currentState is SafeActivationLoaded) {
      emit(currentState.copyWith(isActionInProgress: true));
    }

    final result = await repository.initiateRequest(
      targetOwnerId: event.targetOwnerId,
      reason: event.reason,
    );
    await result.fold(
      (failure) async => emit(SafeActivationError(failure.message)),
      (_) async {
        add(const FetchSafeActivationStatusEvent());
      },
    );
  }

  Future<void> _onCancelRequest(
    CancelSafeActivationRequestEvent event,
    Emitter<SafeActivationState> emit,
  ) async {
    final currentState = state;
    if (currentState is SafeActivationLoaded) {
      emit(currentState.copyWith(isActionInProgress: true));
    }

    final result = await repository.cancelRequest(event.requestId);
    await result.fold(
      (failure) async => emit(SafeActivationError(failure.message)),
      (_) async {
        _stopTimer();
        add(const FetchSafeActivationStatusEvent());
      },
    );
  }

  Future<void> _onConfirmRequest(
    ConfirmSafeActivationRequestEvent event,
    Emitter<SafeActivationState> emit,
  ) async {
    final currentState = state;
    if (currentState is SafeActivationLoaded) {
      emit(currentState.copyWith(isActionInProgress: true));
    }

    final result = await repository.confirmRequest(
      requestId: event.requestId,
      isConfirmed: event.isConfirmed,
      note: event.note,
    );
    await result.fold(
      (failure) async => emit(SafeActivationError(failure.message)),
      (_) async {
        add(const FetchSafeActivationStatusEvent());
      },
    );
  }

  Future<void> _onDeactivateEmergency(
    DeactivateEmergencyPlanEvent event,
    Emitter<SafeActivationState> emit,
  ) async {
    final currentState = state;
    if (currentState is SafeActivationLoaded) {
      emit(currentState.copyWith(isActionInProgress: true));
    }

    final result = await repository.deactivateEmergency();
    await result.fold(
      (failure) async => emit(SafeActivationError(failure.message)),
      (_) async {
        add(const FetchSafeActivationStatusEvent());
      },
    );
  }

  void _onCountdownTick(
    CountdownTickEvent event,
    Emitter<SafeActivationState> emit,
  ) {
    if (state is SafeActivationLoaded) {
      final loaded = state as SafeActivationLoaded;
      emit(loaded.copyWith(countdownSeconds: event.remainingSeconds));
    }
  }

  void _startTimerIfNeeded(int seconds) {
    _stopTimer();
    if (seconds <= 0) return;

    var remaining = seconds;
    _countdownTimer = Timer.periodic(const Duration(seconds: 1), (timer) {
      remaining--;
      if (remaining <= 0) {
        timer.cancel();
        add(const CountdownTickEvent(0));
        add(const FetchSafeActivationStatusEvent());
      } else {
        add(CountdownTickEvent(remaining));
      }
    });
  }

  void _stopTimer() {
    _countdownTimer?.cancel();
    _countdownTimer = null;
  }

  @override
  Future<void> close() {
    _stopTimer();
    return super.close();
  }
}
