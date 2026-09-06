import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/entities/safe_activation_entity.dart';
import '../../domain/repositories/safe_activation_repository.dart';
import '../datasources/safe_activation_remote_datasource.dart';
import '../models/safe_activation_model.dart';

class SafeActivationRepositoryImpl implements SafeActivationRepository {
  final SafeActivationRemoteDataSource remoteDataSource;

  SafeActivationRepositoryImpl({required this.remoteDataSource});

  @override
  Future<Either<Failure, ActivationStatusEntity>> getStatus({String? targetOwnerId}) async {
    try {
      final result = await remoteDataSource.getStatus(targetOwnerId: targetOwnerId);
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ActivationStatusEntity>> vitalityCheckIn() async {
    try {
      final result = await remoteDataSource.vitalityCheckIn();
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ActivationConfigEntity>> updateConfig(ActivationConfigEntity config) async {
    try {
      final model = ActivationConfigModel(
        checkInIntervalDays: config.checkInIntervalDays,
        gracePeriodHours: config.gracePeriodHours,
        minConfirmationsRequired: config.minConfirmationsRequired,
      );
      final result = await remoteDataSource.updateConfig(model);
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ActivationRequestEntity>> initiateRequest({
    required String targetOwnerId,
    String? reason,
  }) async {
    try {
      final result = await remoteDataSource.initiateRequest(
        targetOwnerId: targetOwnerId,
        reason: reason,
      );
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, bool>> cancelRequest(String requestId) async {
    try {
      final result = await remoteDataSource.cancelRequest(requestId);
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ActivationRequestEntity>> confirmRequest({
    required String requestId,
    required bool isConfirmed,
    String? note,
  }) async {
    try {
      final result = await remoteDataSource.confirmRequest(
        requestId: requestId,
        isConfirmed: isConfirmed,
        note: note,
      );
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, bool>> deactivateEmergency() async {
    try {
      final result = await remoteDataSource.deactivateEmergency();
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
