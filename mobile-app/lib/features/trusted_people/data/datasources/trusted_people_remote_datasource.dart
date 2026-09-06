import 'package:dio/dio.dart';
import '../models/trusted_person_model.dart';

abstract class TrustedPeopleRemoteDataSource {
  Future<List<TrustedPersonModel>> getTrustedPeople();
  Future<TrustedPersonModel> createTrustedPerson({
    required String fullName,
    required String email,
    required String phoneNumber,
    required String relationship,
    required int trustLevel,
    String? roleDescription,
  });
  Future<ClaimPairingResultModel> claimPairingCode(String pairingCode);
  Future<bool> revokeTrustedPerson(String id);
  Future<TrustedPersonModel> regeneratePairingCode(String id);
  Future<List<ScopedPermissionModel>> updatePermissions({
    required String id,
    List<Map<String, dynamic>>? categoryPermissions,
    List<Map<String, dynamic>>? actionCardPermissions,
  });
}

class TrustedPeopleRemoteDataSourceImpl implements TrustedPeopleRemoteDataSource {
  final Dio dio;

  TrustedPeopleRemoteDataSourceImpl({required this.dio});

  @override
  Future<List<TrustedPersonModel>> getTrustedPeople() async {
    final response = await dio.get('/api/v1/trusted-people');
    final data = response.data['data'] as List<dynamic>;
    return data.map((json) => TrustedPersonModel.fromJson(json as Map<String, dynamic>)).toList();
  }

  @override
  Future<TrustedPersonModel> createTrustedPerson({
    required String fullName,
    required String email,
    required String phoneNumber,
    required String relationship,
    required int trustLevel,
    String? roleDescription,
  }) async {
    final response = await dio.post(
      '/api/v1/trusted-people',
      data: {
        'fullName': fullName,
        'email': email,
        'phoneNumber': phoneNumber,
        'relationship': relationship,
        'trustLevel': trustLevel,
        'roleDescription': roleDescription,
      },
    );
    return TrustedPersonModel.fromJson(response.data['data'] as Map<String, dynamic>);
  }

  @override
  Future<ClaimPairingResultModel> claimPairingCode(String pairingCode) async {
    final response = await dio.post(
      '/api/v1/trusted-people/pairing/claim',
      data: {'pairingCode': pairingCode},
    );
    return ClaimPairingResultModel.fromJson(response.data['data'] as Map<String, dynamic>);
  }

  @override
  Future<bool> revokeTrustedPerson(String id) async {
    final response = await dio.delete('/api/v1/trusted-people/$id');
    return response.data['data'] as bool? ?? true;
  }

  @override
  Future<TrustedPersonModel> regeneratePairingCode(String id) async {
    final response = await dio.post('/api/v1/trusted-people/$id/pairing-code/regenerate');
    return TrustedPersonModel.fromJson(response.data['data'] as Map<String, dynamic>);
  }

  @override
  Future<List<ScopedPermissionModel>> updatePermissions({
    required String id,
    List<Map<String, dynamic>>? categoryPermissions,
    List<Map<String, dynamic>>? actionCardPermissions,
  }) async {
    final response = await dio.put(
      '/api/v1/trusted-people/$id/permissions',
      data: {
        'categoryPermissions': categoryPermissions,
        'actionCardPermissions': actionCardPermissions,
      },
    );
    final data = response.data['data'] as List<dynamic>;
    return data.map((json) => ScopedPermissionModel.fromJson(json as Map<String, dynamic>)).toList();
  }
}
