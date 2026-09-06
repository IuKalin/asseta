import 'package:dio/dio.dart';
import '../../domain/entities/continuity_plan_entity.dart';
import '../models/continuity_plan_model.dart';

abstract class PlanRemoteDataSource {
  Future<ContinuityPlanModel> getContinuityPlan();
  Future<ContinuityPlanModel> getMyDelegatedPlan();
  Future<ContinuityPlanCardItemModel> updateCardStage({
    required String cardId,
    required UrgencyStage newStage,
    required int rowVersion,
  });
  Future<ContinuityPlanCardItemModel> toggleCardCompletion({
    required String cardId,
    required int rowVersion,
  });
}

class PlanRemoteDataSourceImpl implements PlanRemoteDataSource {
  final Dio dio;

  PlanRemoteDataSourceImpl({required this.dio});

  @override
  Future<ContinuityPlanModel> getContinuityPlan() async {
    final response = await dio.get('/api/v1/continuity-plan');
    final data = response.data['data'] as Map<String, dynamic>;
    return ContinuityPlanModel.fromJson(data);
  }

  @override
  Future<ContinuityPlanModel> getMyDelegatedPlan() async {
    final response = await dio.get('/api/v1/continuity-plan/delegated');
    final data = response.data['data'] as Map<String, dynamic>;
    return ContinuityPlanModel.fromJson(data);
  }

  @override
  Future<ContinuityPlanCardItemModel> updateCardStage({
    required String cardId,
    required UrgencyStage newStage,
    required int rowVersion,
  }) async {
    final response = await dio.patch(
      '/api/v1/continuity-plan/cards/$cardId/stage',
      data: {
        'newStage': newStage.toShortString(),
        'rowVersion': rowVersion,
      },
    );
    final data = response.data['data'] as Map<String, dynamic>;
    return ContinuityPlanCardItemModel.fromJson(data);
  }

  @override
  Future<ContinuityPlanCardItemModel> toggleCardCompletion({
    required String cardId,
    required int rowVersion,
  }) async {
    final response = await dio.patch(
      '/api/v1/continuity-plan/cards/$cardId/toggle-completion',
      data: {
        'rowVersion': rowVersion,
      },
    );
    final data = response.data['data'] as Map<String, dynamic>;
    return ContinuityPlanCardItemModel.fromJson(data);
  }
}
