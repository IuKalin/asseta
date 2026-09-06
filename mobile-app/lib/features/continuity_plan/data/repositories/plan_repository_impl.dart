import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/entities/continuity_plan_entity.dart';
import '../../domain/repositories/continuity_plan_repository.dart';
import '../datasources/plan_remote_datasource.dart';

class PlanRepositoryImpl implements ContinuityPlanRepository {
  final PlanRemoteDataSource remoteDataSource;

  PlanRepositoryImpl({required this.remoteDataSource});

  @override
  Future<Either<Failure, ContinuityPlanEntity>> getContinuityPlan() async {
    try {
      final result = await remoteDataSource.getContinuityPlan();
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ContinuityPlanEntity>> getMyDelegatedPlan() async {
    try {
      final result = await remoteDataSource.getMyDelegatedPlan();
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ContinuityPlanCardItemEntity>> updateCardStage({
    required String cardId,
    required UrgencyStage newStage,
    required int rowVersion,
  }) async {
    try {
      final result = await remoteDataSource.updateCardStage(
        cardId: cardId,
        newStage: newStage,
        rowVersion: rowVersion,
      );
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ContinuityPlanCardItemEntity>> toggleCardCompletion({
    required String cardId,
    required int rowVersion,
  }) async {
    try {
      final result = await remoteDataSource.toggleCardCompletion(
        cardId: cardId,
        rowVersion: rowVersion,
      );
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
