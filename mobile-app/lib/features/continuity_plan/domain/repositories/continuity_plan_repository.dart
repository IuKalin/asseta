import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../entities/continuity_plan_entity.dart';

abstract class ContinuityPlanRepository {
  Future<Either<Failure, ContinuityPlanEntity>> getContinuityPlan();
  Future<Either<Failure, ContinuityPlanEntity>> getMyDelegatedPlan();
  Future<Either<Failure, ContinuityPlanCardItemEntity>> updateCardStage({
    required String cardId,
    required UrgencyStage newStage,
    required int rowVersion,
  });
  Future<Either<Failure, ContinuityPlanCardItemEntity>> toggleCardCompletion({
    required String cardId,
    required int rowVersion,
  });
}

