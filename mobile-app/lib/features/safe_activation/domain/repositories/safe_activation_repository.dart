import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../entities/safe_activation_entity.dart';

abstract class SafeActivationRepository {
  Future<Either<Failure, ActivationStatusEntity>> getStatus({String? targetOwnerId});
  Future<Either<Failure, ActivationStatusEntity>> vitalityCheckIn();
  Future<Either<Failure, ActivationConfigEntity>> updateConfig(ActivationConfigEntity config);
  Future<Either<Failure, ActivationRequestEntity>> initiateRequest({
    required String targetOwnerId,
    String? reason,
  });
  Future<Either<Failure, bool>> cancelRequest(String requestId);
  Future<Either<Failure, ActivationRequestEntity>> confirmRequest({
    required String requestId,
    required bool isConfirmed,
    String? note,
  });
  Future<Either<Failure, bool>> deactivateEmergency();
}
