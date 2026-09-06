import 'package:dartz/dartz.dart';
import 'package:asseta_mobile/core/errors/failures.dart';
import '../../domain/entities/trusted_person_entity.dart';
import '../../domain/repositories/trusted_people_repository.dart';
import '../datasources/trusted_people_remote_datasource.dart';

class TrustedPeopleRepositoryImpl implements TrustedPeopleRepository {
  final TrustedPeopleRemoteDataSource remoteDataSource;

  TrustedPeopleRepositoryImpl({required this.remoteDataSource});

  @override
  Future<Either<Failure, List<TrustedPersonEntity>>> getTrustedPeople() async {
    try {
      final result = await remoteDataSource.getTrustedPeople();
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, TrustedPersonEntity>> createTrustedPerson({
    required String fullName,
    required String email,
    required String phoneNumber,
    required String relationship,
    required int trustLevel,
    String? roleDescription,
  }) async {
    try {
      final result = await remoteDataSource.createTrustedPerson(
        fullName: fullName,
        email: email,
        phoneNumber: phoneNumber,
        relationship: relationship,
        trustLevel: trustLevel,
        roleDescription: roleDescription,
      );
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ClaimPairingResultEntity>> claimPairingCode(String pairingCode) async {
    try {
      final result = await remoteDataSource.claimPairingCode(pairingCode);
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, bool>> revokeTrustedPerson(String id) async {
    try {
      final result = await remoteDataSource.revokeTrustedPerson(id);
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, TrustedPersonEntity>> regeneratePairingCode(String id) async {
    try {
      final result = await remoteDataSource.regeneratePairingCode(id);
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, List<ScopedPermissionEntity>>> updatePermissions({
    required String id,
    List<Map<String, dynamic>>? categoryPermissions,
    List<Map<String, dynamic>>? actionCardPermissions,
  }) async {
    try {
      final result = await remoteDataSource.updatePermissions(
        id: id,
        categoryPermissions: categoryPermissions,
        actionCardPermissions: actionCardPermissions,
      );
      return Right(result);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
