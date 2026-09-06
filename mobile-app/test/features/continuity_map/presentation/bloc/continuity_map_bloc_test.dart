import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/core/crypto/crypto_service.dart';
import 'package:asseta_mobile/core/errors/failures.dart';
import 'package:asseta_mobile/features/continuity_map/domain/entities/continuity_item_entity.dart';
import 'package:asseta_mobile/features/continuity_map/domain/entities/continuity_map_entity.dart';
import 'package:asseta_mobile/features/continuity_map/domain/repositories/continuity_repository.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/bloc/continuity_map_bloc.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/bloc/continuity_map_event.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/bloc/continuity_map_state.dart';

class MockContinuityRepository implements ContinuityRepository {
  bool shouldFail = false;
  ContinuityMapEntity testMap = const ContinuityMapEntity(
    overallReadinessScore: 75,
    totalItems: 5,
    totalGaps: 1,
    categories: [],
    gaps: [],
  );

  @override
  Future<Either<Failure, ContinuityMapEntity>> getContinuityMap([String? ownerId]) async {
    if (shouldFail) {
      return const Left(ServerFailure('Connection refused'));
    }
    return Right(testMap);
  }

  @override
  Future<Either<Failure, ContinuityItemEntity>> createContinuityItem({
    required String categoryId,
    required String name,
    required ItemPriority priority,
    String? documentLocationHint,
    String? assignedTrustedPersonId,
    String? cipherNotesBlob,
    String? cipherNonce,
    String? cipherAuthTag,
  }) async {
    return Right(ContinuityItemEntity(
      id: 'item-new',
      ownerId: 'owner-1',
      categoryId: categoryId,
      categoryCode: 'FINANCIAL',
      name: name,
      priority: priority,
      documentLocationHint: documentLocationHint,
      assignedTrustedPersonId: assignedTrustedPersonId,
      hasConfidentialNotes: cipherNotesBlob != null,
      isCompleted: false,
      rowVersion: 1,
      sortOrder: 1,
      createdAtUtc: DateTime.now(),
    ));
  }

  @override
  Future<Either<Failure, ContinuityItemEntity>> updateContinuityItem({
    required String id,
    required String name,
    required ItemPriority priority,
    String? documentLocationHint,
    String? assignedTrustedPersonId,
    String? cipherNotesBlob,
    String? cipherNonce,
    String? cipherAuthTag,
    required int rowVersion,
  }) async {
    return Right(ContinuityItemEntity(
      id: id,
      ownerId: 'owner-1',
      categoryId: 'cat-1',
      categoryCode: 'FINANCIAL',
      name: name,
      priority: priority,
      documentLocationHint: documentLocationHint,
      assignedTrustedPersonId: assignedTrustedPersonId,
      hasConfidentialNotes: cipherNotesBlob != null,
      isCompleted: false,
      rowVersion: rowVersion + 1,
      sortOrder: 1,
      createdAtUtc: DateTime.now(),
    ));
  }

  @override
  Future<Either<Failure, void>> deleteContinuityItem(String id) async {
    return const Right(null);
  }
}

void main() {
  group('ContinuityMapBloc Tests', () {
    late ContinuityMapBloc bloc;
    late MockContinuityRepository mockRepo;
    late MobileCryptoService cryptoService;

    setUp(() {
      mockRepo = MockContinuityRepository();
      cryptoService = MobileCryptoService();
      bloc = ContinuityMapBloc(repository: mockRepo, cryptoService: cryptoService);
    });

    tearDown(() {
      bloc.close();
    });

    test('initial state is ContinuityMapInitial', () {
      expect(bloc.state, equals(const ContinuityMapInitial()));
    });

    test('emits [ContinuityMapLoading, ContinuityMapLoaded] when LoadContinuityMapEvent succeeds', () async {
      final expectedStates = [
        const ContinuityMapLoading(),
        ContinuityMapLoaded(map: mockRepo.testMap),
      ];

      expectLater(bloc.stream, emitsInOrder(expectedStates));

      bloc.add(const LoadContinuityMapEvent());
    });

    test('emits [ContinuityMapLoading, ContinuityMapError] when LoadContinuityMapEvent fails', () async {
      mockRepo.shouldFail = true;
      final expectedStates = [
        const ContinuityMapLoading(),
        const ContinuityMapError('Connection refused'),
      ];

      expectLater(bloc.stream, emitsInOrder(expectedStates));

      bloc.add(const LoadContinuityMapEvent());
    });

    test('emits state with isKeyUnlocked = true when UnlockMasterKeyEvent succeeds', () async {
      // Setup loaded state first
      bloc.emit(ContinuityMapLoaded(map: mockRepo.testMap));

      bloc.add(const UnlockMasterKeyEvent('SecurePassphrase123!'));

      await expectLater(
        bloc.stream,
        emits(isA<ContinuityMapLoaded>().having((s) => s.isKeyUnlocked, 'isKeyUnlocked', isTrue)),
      );
    });
  });
}
