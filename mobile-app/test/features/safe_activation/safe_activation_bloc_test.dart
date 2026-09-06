import 'package:flutter_test/flutter_test.dart';
import 'package:dartz/dartz.dart';
import 'package:asseta_mobile/core/errors/failures.dart';
import 'package:asseta_mobile/features/safe_activation/domain/entities/safe_activation_entity.dart';
import 'package:asseta_mobile/features/safe_activation/domain/repositories/safe_activation_repository.dart';
import 'package:asseta_mobile/features/safe_activation/presentation/bloc/safe_activation_bloc.dart';
import 'package:asseta_mobile/features/safe_activation/presentation/bloc/safe_activation_event.dart';
import 'package:asseta_mobile/features/safe_activation/presentation/bloc/safe_activation_state.dart';

class MockSafeActivationRepository implements SafeActivationRepository {
  ActivationStatusEntity? currentStatus;
  bool shouldFail = false;
  String errorMessage = 'Lỗi kết nối máy chủ';

  @override
  Future<Either<Failure, ActivationStatusEntity>> getStatus({String? targetOwnerId}) async {
    if (shouldFail) return Left(ServerFailure(errorMessage));
    return Right(currentStatus ?? _createDefaultStatus());
  }

  @override
  Future<Either<Failure, ActivationStatusEntity>> vitalityCheckIn() async {
    if (shouldFail) return Left(ServerFailure(errorMessage));
    final updated = ActivationStatusEntity(
      ownerId: 'owner-1',
      checkInIntervalDays: 30,
      gracePeriodHours: 48,
      minConfirmationsRequired: 2,
      lastCheckInAtUtc: DateTime.now(),
      nextCheckInDueUtc: DateTime.now().add(const Duration(days: 30)),
      isDueSoon: false,
      isOverdue: false,
      heartbeatStatus: HeartbeatStatus.active,
      isEmergencyActive: false,
      activeRequest: null,
    );
    currentStatus = updated;
    return Right(updated);
  }

  @override
  Future<Either<Failure, ActivationConfigEntity>> updateConfig(ActivationConfigEntity config) async {
    if (shouldFail) return Left(ServerFailure(errorMessage));
    return Right(config);
  }

  @override
  Future<Either<Failure, ActivationRequestEntity>> initiateRequest({
    required String targetOwnerId,
    String? reason,
  }) async {
    if (shouldFail) return Left(ServerFailure(errorMessage));
    final req = ActivationRequestEntity(
      id: 'req-1',
      ownerId: targetOwnerId,
      triggerSource: 'TrustedPersonRequest',
      reason: reason,
      status: 'PendingGracePeriod',
      gracePeriodExpiresAtUtc: DateTime.now().add(const Duration(hours: 48)),
      remainingSeconds: 48 * 3600,
      confirmationsCount: 1,
      minConfirmationsRequired: 2,
      rowVersion: 1,
      confirmations: const [],
    );
    return Right(req);
  }

  @override
  Future<Either<Failure, bool>> cancelRequest(String requestId) async {
    if (shouldFail) return Left(ServerFailure(errorMessage));
    currentStatus = _createDefaultStatus();
    return const Right(true);
  }

  @override
  Future<Either<Failure, ActivationRequestEntity>> confirmRequest({
    required String requestId,
    required bool isConfirmed,
    String? note,
  }) async {
    if (shouldFail) return Left(ServerFailure(errorMessage));
    final req = ActivationRequestEntity(
      id: requestId,
      ownerId: 'owner-1',
      triggerSource: 'TrustedPersonRequest',
      status: 'PendingGracePeriod',
      gracePeriodExpiresAtUtc: DateTime.now().add(const Duration(hours: 48)),
      remainingSeconds: 48 * 3600,
      confirmationsCount: 2,
      minConfirmationsRequired: 2,
      rowVersion: 2,
      confirmations: const [],
    );
    return Right(req);
  }

  @override
  Future<Either<Failure, bool>> deactivateEmergency() async {
    if (shouldFail) return Left(ServerFailure(errorMessage));
    currentStatus = _createDefaultStatus();
    return const Right(true);
  }

  ActivationStatusEntity _createDefaultStatus() {
    return ActivationStatusEntity(
      ownerId: 'owner-1',
      checkInIntervalDays: 30,
      gracePeriodHours: 48,
      minConfirmationsRequired: 2,
      lastCheckInAtUtc: DateTime.now().subtract(const Duration(days: 5)),
      nextCheckInDueUtc: DateTime.now().add(const Duration(days: 25)),
      isDueSoon: false,
      isOverdue: false,
      heartbeatStatus: HeartbeatStatus.active,
      isEmergencyActive: false,
      activeRequest: null,
    );
  }
}

void main() {
  late MockSafeActivationRepository repository;
  late SafeActivationBloc bloc;

  setUp(() {
    repository = MockSafeActivationRepository();
    bloc = SafeActivationBloc(repository: repository);
  });

  tearDown(() {
    bloc.close();
  });

  group('SafeActivationBloc', () {
    test('initial state is SafeActivationInitial', () {
      expect(bloc.state, isA<SafeActivationInitial>());
    });

    test('FetchSafeActivationStatusEvent emits [Loading, Loaded] on success', () async {
      final expectedStates = [
        isA<SafeActivationLoading>(),
        isA<SafeActivationLoaded>()
            .having((s) => s.status.heartbeatStatus, 'heartbeatStatus', HeartbeatStatus.active)
            .having((s) => s.status.ownerId, 'ownerId', 'owner-1'),
      ];

      expectLater(bloc.stream, emitsInOrder(expectedStates));

      bloc.add(const FetchSafeActivationStatusEvent());
    });

    test('FetchSafeActivationStatusEvent emits [Loading, Error] on failure', () async {
      repository.shouldFail = true;
      repository.errorMessage = 'Network connection failed';

      final expectedStates = [
        isA<SafeActivationLoading>(),
        isA<SafeActivationError>().having((s) => s.message, 'message', 'Network connection failed'),
      ];

      expectLater(bloc.stream, emitsInOrder(expectedStates));

      bloc.add(const FetchSafeActivationStatusEvent());
    });

    test('VitalityCheckInEvent extends status and emits successMessage', () async {
      bloc.add(const FetchSafeActivationStatusEvent());
      await pumpEventQueue();

      final expectedStates = [
        isA<SafeActivationLoaded>().having((s) => s.isActionInProgress, 'isActionInProgress', true),
        isA<SafeActivationLoaded>()
            .having((s) => s.status.heartbeatStatus, 'heartbeatStatus', HeartbeatStatus.active)
            .having((s) => s.successMessage, 'successMessage', isNotNull),
      ];

      expectLater(bloc.stream, emitsInOrder(expectedStates));

      bloc.add(const VitalityCheckInEvent());
    });

    test('CancelSafeActivationRequestEvent resets pending activation', () async {
      final pendingReq = ActivationRequestEntity(
        id: 'req-pending-1',
        ownerId: 'owner-1',
        triggerSource: 'SystemTimeout',
        status: 'PendingGracePeriod',
        gracePeriodExpiresAtUtc: DateTime.now().add(const Duration(hours: 48)),
        remainingSeconds: 48 * 3600,
        confirmationsCount: 0,
        minConfirmationsRequired: 2,
        rowVersion: 1,
        confirmations: const [],
      );

      repository.currentStatus = ActivationStatusEntity(
        ownerId: 'owner-1',
        checkInIntervalDays: 30,
        gracePeriodHours: 48,
        minConfirmationsRequired: 2,
        isDueSoon: false,
        isOverdue: false,
        heartbeatStatus: HeartbeatStatus.pendingGracePeriod,
        isEmergencyActive: false,
        activeRequest: pendingReq,
      );

      bloc.add(const FetchSafeActivationStatusEvent());
      await pumpEventQueue();

      bloc.add(const CancelSafeActivationRequestEvent('req-pending-1'));
      await pumpEventQueue();

      expect(bloc.state, isA<SafeActivationLoaded>());
      final loaded = bloc.state as SafeActivationLoaded;
      expect(loaded.status.activeRequest, isNull);
    });
  });
}
