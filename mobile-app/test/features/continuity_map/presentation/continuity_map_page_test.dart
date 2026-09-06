import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/core/crypto/crypto_service.dart';
import 'package:asseta_mobile/core/errors/failures.dart';
import 'package:asseta_mobile/features/continuity_map/domain/entities/continuity_item_entity.dart';
import 'package:asseta_mobile/features/continuity_map/domain/entities/continuity_map_entity.dart';
import 'package:asseta_mobile/features/continuity_map/domain/repositories/continuity_repository.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/bloc/continuity_map_bloc.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/bloc/continuity_map_state.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/pages/continuity_map_page.dart';
import 'package:dartz/dartz.dart';

class MockRepoForWidgetTest implements ContinuityRepository {
  final ContinuityMapEntity testMap;

  MockRepoForWidgetTest(this.testMap);

  @override
  Future<Either<Failure, ContinuityMapEntity>> getContinuityMap([String? ownerId]) async {
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
    return Left(const ServerFailure('Not needed'));
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
    return Left(const ServerFailure('Not needed'));
  }

  @override
  Future<Either<Failure, void>> deleteContinuityItem(String id) async {
    return const Right(null);
  }
}

void main() {
  testWidgets('ContinuityMapPage renders categories and overall readiness score', (tester) async {
    final testMap = ContinuityMapEntity(
      overallReadinessScore: 82,
      totalItems: 3,
      totalGaps: 0,
      categories: [
        ContinuityCategoryEntity(
          categoryId: 'cat-fin',
          code: 'FINANCIAL',
          name: 'Tài chính & Tiền tệ',
          icon: 'wallet',
          readinessScore: 82,
          items: [
            ContinuityItemEntity(
              id: 'item-1',
              ownerId: 'owner-1',
              categoryId: 'cat-fin',
              categoryCode: 'FINANCIAL',
              name: 'Sổ tiết kiệm Vietcombank',
              priority: ItemPriority.critical,
              documentLocationHint: 'Két sắt',
              assignedTrustedPersonId: 'person-1',
              hasConfidentialNotes: false,
              isCompleted: false,
              rowVersion: 1,
              sortOrder: 1,
              createdAtUtc: DateTime(2026, 9, 5),
            ),
          ],
        ),
      ],
      gaps: [],
    );

    final mockRepo = MockRepoForWidgetTest(testMap);
    final cryptoService = MobileCryptoService();
    final bloc = ContinuityMapBloc(repository: mockRepo, cryptoService: cryptoService);

    // Seed state directly
    bloc.emit(ContinuityMapLoaded(map: testMap));

    await tester.pumpWidget(
      MaterialApp(
        home: BlocProvider<ContinuityMapBloc>.value(
          value: bloc,
          child: const ContinuityMapPage(),
        ),
      ),
    );

    await tester.pump();

    // Verify Title & Readiness Score
    expect(find.text('Continuity Map'), findsOneWidget);
    expect(find.text('82%'), findsNWidgets(2)); // Header + Category badge
    expect(find.text('Tài chính & Tiền tệ'), findsOneWidget);
    expect(find.text('Sổ tiết kiệm Vietcombank'), findsOneWidget);
  });
}
