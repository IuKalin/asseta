import 'package:dartz/dartz.dart';
import 'package:asseta_mobile/core/errors/failures.dart';
import '../entities/trusted_person_entity.dart';

abstract class TrustedPeopleRepository {
  Future<Either<Failure, List<TrustedPersonEntity>>> getTrustedPeople();

  Future<Either<Failure, TrustedPersonEntity>> createTrustedPerson({
    required String fullName,
    required String email,
    required String phoneNumber,
    required String relationship,
    required int trustLevel,
    String? roleDescription,
  });

  Future<Either<Failure, ClaimPairingResultEntity>> claimPairingCode(String pairingCode);

  Future<Either<Failure, bool>> revokeTrustedPerson(String id);

  Future<Either<Failure, TrustedPersonEntity>> regeneratePairingCode(String id);

  Future<Either<Failure, List<ScopedPermissionEntity>>> updatePermissions({
    required String id,
    List<Map<String, dynamic>>? categoryPermissions,
    List<Map<String, dynamic>>? actionCardPermissions,
  });
}
