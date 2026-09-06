import 'package:flutter_test/flutter_test.dart';
import 'package:dartz/dartz.dart';
import 'package:asseta_mobile/core/errors/failures.dart';
import 'package:asseta_mobile/features/trusted_people/domain/entities/trusted_person_entity.dart';
import 'package:asseta_mobile/features/trusted_people/domain/repositories/trusted_people_repository.dart';
import 'package:asseta_mobile/features/trusted_people/presentation/bloc/trusted_people_bloc.dart';

class MockTrustedPeopleRepository implements TrustedPeopleRepository {
  List<TrustedPersonEntity> people = [];

  @override
  Future<Either<Failure, List<TrustedPersonEntity>>> getTrustedPeople() async {
    return Right(people);
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
    final newPerson = TrustedPersonEntity(
      id: 'tp-new',
      ownerId: 'owner-1',
      fullName: fullName,
      email: email,
      phoneNumber: phoneNumber,
      relationship: relationship,
      trustLevel: trustLevel,
      roleDescription: roleDescription,
      status: TrustedPersonStatus.invited,
      activePairingCode: 'AS7K9P',
      pairingExpiresAt: DateTime.now().add(const Duration(hours: 48)),
      rowVersion: 1,
      createdAtUtc: DateTime.now(),
      permissions: const [],
    );
    people.add(newPerson);
    return Right(newPerson);
  }

  @override
  Future<Either<Failure, ClaimPairingResultEntity>> claimPairingCode(String pairingCode) async {
    if (pairingCode == 'AS7K9P') {
      return Right(ClaimPairingResultEntity(
        trustedPersonId: 'tp-1',
        ownerId: 'owner-1',
        ownerDisplayName: 'Chủ tài sản A',
        assignedRole: 'Luật sư',
        status: 'Active',
        pairedAtUtc: DateTime.now(),
      ));
    }
    return const Left(ServerFailure('Mã ghép đôi không hợp lệ hoặc đã hết hạn.'));
  }

  @override
  Future<Either<Failure, bool>> revokeTrustedPerson(String id) async {
    people.removeWhere((p) => p.id == id);
    return const Right(true);
  }

  @override
  Future<Either<Failure, TrustedPersonEntity>> regeneratePairingCode(String id) async {
    final idx = people.indexWhere((p) => p.id == id);
    if (idx != -1) {
      final updated = TrustedPersonEntity(
        id: people[idx].id,
        ownerId: people[idx].ownerId,
        fullName: people[idx].fullName,
        email: people[idx].email,
        phoneNumber: people[idx].phoneNumber,
        relationship: people[idx].relationship,
        trustLevel: people[idx].trustLevel,
        status: people[idx].status,
        activePairingCode: 'NEW999',
        rowVersion: people[idx].rowVersion + 1,
        createdAtUtc: people[idx].createdAtUtc,
        permissions: people[idx].permissions,
      );
      people[idx] = updated;
      return Right(updated);
    }
    return const Left(ServerFailure('Not found'));
  }

  @override
  Future<Either<Failure, List<ScopedPermissionEntity>>> updatePermissions({
    required String id,
    List<Map<String, dynamic>>? categoryPermissions,
    List<Map<String, dynamic>>? actionCardPermissions,
  }) async {
    return const Right([]);
  }
}

void main() {
  late MockTrustedPeopleRepository repository;
  late TrustedPeopleBloc bloc;

  setUp(() {
    repository = MockTrustedPeopleRepository();
    bloc = TrustedPeopleBloc(repository: repository);
  });

  tearDown(() {
    bloc.close();
  });

  test('Initial state should be TrustedPeopleInitialState', () {
    expect(bloc.state, equals(TrustedPeopleInitialState()));
  });

  test('LoadTrustedPeopleEvent emits LoadedState with list of people', () async {
    repository.people = [
      TrustedPersonEntity(
        id: '1',
        ownerId: 'owner-1',
        fullName: 'Nguyễn A',
        email: 'a@example.com',
        phoneNumber: '0901',
        relationship: 'Vợ',
        trustLevel: 3,
        status: TrustedPersonStatus.active,
        rowVersion: 1,
        createdAtUtc: DateTime.now(),
        permissions: const [],
      )
    ];

    expectLater(
      bloc.stream,
      emitsInOrder([
        TrustedPeopleLoadingState(),
        isA<TrustedPeopleLoadedState>().having((s) => s.people.length, 'people.length', 1),
      ]),
    );

    bloc.add(const LoadTrustedPeopleEvent());
  });

  test('ClaimPairingCodeEvent emits PairingClaimSuccessState when code matches', () async {
    expectLater(
      bloc.stream,
      emitsInOrder([
        TrustedPeopleLoadingState(),
        isA<PairingClaimSuccessState>().having((s) => s.result.status, 'status', 'Active'),
      ]),
    );

    bloc.add(const ClaimPairingCodeEvent('AS7K9P'));
  });

  test('ClaimPairingCodeEvent emits TrustedPeopleErrorState when code fails', () async {
    expectLater(
      bloc.stream,
      emitsInOrder([
        TrustedPeopleLoadingState(),
        isA<TrustedPeopleErrorState>(),
      ]),
    );

    bloc.add(const ClaimPairingCodeEvent('INVALID'));
  });
}
