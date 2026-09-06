import 'package:dio/dio.dart';
import '../models/safe_activation_model.dart';

abstract class SafeActivationRemoteDataSource {
  Future<ActivationStatusModel> getStatus({String? targetOwnerId});
  Future<ActivationStatusModel> vitalityCheckIn();
  Future<ActivationConfigModel> updateConfig(ActivationConfigModel config);
  Future<ActivationRequestModel> initiateRequest({
    required String targetOwnerId,
    String? reason,
  });
  Future<bool> cancelRequest(String requestId);
  Future<ActivationRequestModel> confirmRequest({
    required String requestId,
    required bool isConfirmed,
    String? note,
  });
  Future<bool> deactivateEmergency();
}

class SafeActivationRemoteDataSourceImpl implements SafeActivationRemoteDataSource {
  final Dio dio;

  SafeActivationRemoteDataSourceImpl({required this.dio});

  @override
  Future<ActivationStatusModel> getStatus({String? targetOwnerId}) async {
    final queryParams = targetOwnerId != null ? {'targetOwnerId': targetOwnerId} : null;
    final response = await dio.get(
      '/api/v1/safe-activation/status',
      queryParameters: queryParams,
    );
    final data = response.data['data'] as Map<String, dynamic>;
    return ActivationStatusModel.fromJson(data);
  }

  @override
  Future<ActivationStatusModel> vitalityCheckIn() async {
    final response = await dio.post('/api/v1/safe-activation/check-in');
    final data = response.data['data'] as Map<String, dynamic>;
    return ActivationStatusModel.fromJson(data);
  }

  @override
  Future<ActivationConfigModel> updateConfig(ActivationConfigModel config) async {
    final response = await dio.put(
      '/api/v1/safe-activation/config',
      data: config.toJson(),
    );
    final data = response.data['data'] as Map<String, dynamic>;
    return ActivationConfigModel.fromJson(data);
  }

  @override
  Future<ActivationRequestModel> initiateRequest({
    required String targetOwnerId,
    String? reason,
  }) async {
    final response = await dio.post(
      '/api/v1/safe-activation/requests',
      data: {
        'targetOwnerId': targetOwnerId,
        'reason': reason,
      },
    );
    final data = response.data['data'] as Map<String, dynamic>;
    return ActivationRequestModel.fromJson(data);
  }

  @override
  Future<bool> cancelRequest(String requestId) async {
    final response = await dio.post('/api/v1/safe-activation/requests/$requestId/cancel');
    return response.data['data'] as bool? ?? true;
  }

  @override
  Future<ActivationRequestModel> confirmRequest({
    required String requestId,
    required bool isConfirmed,
    String? note,
  }) async {
    final response = await dio.post(
      '/api/v1/safe-activation/requests/$requestId/confirm',
      data: {
        'isConfirmed': isConfirmed,
        'note': note,
      },
    );
    final data = response.data['data'] as Map<String, dynamic>;
    return ActivationRequestModel.fromJson(data);
  }

  @override
  Future<bool> deactivateEmergency() async {
    final response = await dio.post('/api/v1/safe-activation/deactivate');
    return response.data['data'] as bool? ?? true;
  }
}
