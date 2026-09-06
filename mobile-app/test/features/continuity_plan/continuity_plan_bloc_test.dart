import 'package:flutter_test/flutter_test.dart';
import 'package:dartz/dartz.dart';
import 'package:asseta_mobile/core/errors/failures.dart';
import 'package:asseta_mobile/features/continuity_plan/domain/entities/continuity_plan_entity.dart';
import 'package:asseta_mobile/features/continuity_plan/domain/repositories/continuity_plan_repository.dart';
import 'package:asseta_mobile/features/continuity_plan/presentation/bloc/continuity_plan_bloc.dart';
import 'package:asseta_mobile/features/continuity_plan/presentation/bloc/continuity_plan_event.dart';
import 'package:asseta_mobile/features/continuity_plan/presentation/bloc/continuity_plan_state.dart';

class MockContinuityPlanRepository implements ContinuityPlanRepository {
  ContinuityPlanEntity? currentPlan;
  ContinuityPlanEntity? delegatedPlan;
  bool shouldFail = false;

  @override
  Future<Either<Failure, ContinuityPlanEntity>> getContinuityPlan() async {
    if (shouldFail) {
      return const Left(ServerFailure('Lỗi máy chủ khi tải kế hoạch'));
    }
    return Right(currentPlan ?? _createDefaultPlan());
  }

  @override
  Future<Either<Failure, ContinuityPlanEntity>> getMyDelegatedPlan() async {
    if (shouldFail) {
      return const Left(ServerFailure('Lỗi máy chủ khi tải kế hoạch ủy quyền'));
    }
    return Right(delegatedPlan ?? _createDefaultPlan());
  }

  @override
  Future<Either<Failure, ContinuityPlanCardItemEntity>> updateCardStage({
    required String cardId,
    required UrgencyStage newStage,
    required int rowVersion,
  }) async {
    if (shouldFail) {
      return const Left(ServerFailure('Dữ liệu đã bị thay đổi bởi phiên làm việc khác'));
    }
    final card = ContinuityPlanCardItemEntity(
      id: cardId,
      categoryId: 'cat-1',
      categoryName: 'Tài chính',
      categoryIcon: 'account_balance',
      title: 'Tài khoản ngân hàng',
      urgency: newStage,
      priority: 'CRITICAL',
      hasDocumentLocation: true,
      hasStageGap: false,
      isCompleted: false,
      stepsCount: 2,
      contactsCount: 1,
      rowVersion: rowVersion + 1,
    );
    return Right(card);
  }

  @override
  Future<Either<Failure, ContinuityPlanCardItemEntity>> toggleCardCompletion({
    required String cardId,
    required int rowVersion,
  }) async {
    if (shouldFail) {
      return const Left(ServerFailure('Lỗi máy chủ'));
    }
    final card = ContinuityPlanCardItemEntity(
      id: cardId,
      categoryId: 'cat-1',
      categoryName: 'Tài chính',
      categoryIcon: 'account_balance',
      title: 'Tài khoản ngân hàng',
      urgency: UrgencyStage.immediate,
      priority: 'CRITICAL',
      hasDocumentLocation: true,
      hasStageGap: false,
      isCompleted: true,
      stepsCount: 2,
      contactsCount: 1,
      rowVersion: rowVersion + 1,
    );
    return Right(card);
  }

  ContinuityPlanEntity _createDefaultPlan() {
    return const ContinuityPlanEntity(
      ownerId: 'owner-test',
      totalCardsCount: 3,
      completedCardsCount: 1,
      gapsCount: 1,
      delegateCoveragePercentage: 66.7,
      documentReadinessPercentage: 66.7,
      overallPlanReadinessScore: 70,
      hasSinglePointOfFailureRisk: false,
      stages: [
        ContinuityPlanStageEntity(
          stage: UrgencyStage.immediate,
          stageName: 'Ngay lập tức (24h)',
          stageDescription: 'Cấp bách',
          totalCardsCount: 2,
          completedCardsCount: 1,
          gapCardsCount: 1,
          cards: [
            ContinuityPlanCardItemEntity(
              id: 'card-1',
              categoryId: 'cat-1',
              categoryName: 'Bất Động Sản',
              categoryIcon: 'home',
              title: 'Sổ đỏ biệt thự',
              urgency: UrgencyStage.immediate,
              priority: 'CRITICAL',
              assignedTrustedPersonId: 'tp-1',
              assignedTrustedPersonName: 'Nguyễn Văn B',
              hasDocumentLocation: false,
              hasStageGap: true,
              isCompleted: false,
              stepsCount: 2,
              contactsCount: 1,
              rowVersion: 1,
            ),
            ContinuityPlanCardItemEntity(
              id: 'card-2',
              categoryId: 'cat-2',
              categoryName: 'Tài chính',
              categoryIcon: 'account_balance',
              title: 'Sổ tiết kiệm',
              urgency: UrgencyStage.immediate,
              priority: 'CRITICAL',
              assignedTrustedPersonId: 'tp-2',
              assignedTrustedPersonName: 'Trần Thị C',
              hasDocumentLocation: true,
              documentLocationHint: 'Két sắt',
              hasStageGap: false,
              isCompleted: true,
              stepsCount: 1,
              contactsCount: 1,
              rowVersion: 1,
            ),
          ],
        ),
        ContinuityPlanStageEntity(
          stage: UrgencyStage.first72Hours,
          stageName: '72 giờ đầu',
          stageDescription: 'Ưu tiên cao',
          totalCardsCount: 1,
          completedCardsCount: 0,
          gapCardsCount: 0,
          cards: [
            ContinuityPlanCardItemEntity(
              id: 'card-3',
              categoryId: 'cat-2',
              categoryName: 'Tài chính',
              categoryIcon: 'account_balance',
              title: 'Cổ phiếu',
              urgency: UrgencyStage.first72Hours,
              priority: 'IMPORTANT',
              assignedTrustedPersonId: 'tp-1',
              assignedTrustedPersonName: 'Nguyễn Văn B',
              hasDocumentLocation: true,
              hasStageGap: false,
              isCompleted: false,
              stepsCount: 1,
              contactsCount: 1,
              rowVersion: 1,
            ),
          ],
        ),
      ],
    );
  }
}

void main() {
  late MockContinuityPlanRepository repository;
  late ContinuityPlanBloc bloc;

  setUp(() {
    repository = MockContinuityPlanRepository();
    bloc = ContinuityPlanBloc(repository: repository);
  });

  tearDown(() {
    bloc.close();
  });

  test('Initial state should be ContinuityPlanInitialState', () {
    expect(bloc.state, equals(ContinuityPlanInitialState()));
  });

  test('LoadContinuityPlanEvent emits Loading then Loaded state', () async {
    final expectedStates = [
      ContinuityPlanLoadingState(),
      isA<ContinuityPlanLoadedState>()
          .having((s) => s.plan.ownerId, 'ownerId', 'owner-test')
          .having((s) => s.isDelegatedView, 'isDelegatedView', false),
    ];

    expectLater(bloc.stream, emitsInOrder(expectedStates));

    bloc.add(const LoadContinuityPlanEvent());
  });

  test('LoadContinuityPlanEvent emits Error state on failure', () async {
    repository.shouldFail = true;

    final expectedStates = [
      ContinuityPlanLoadingState(),
      const ContinuityPlanErrorState('Lỗi máy chủ khi tải kế hoạch'),
    ];

    expectLater(bloc.stream, emitsInOrder(expectedStates));

    bloc.add(const LoadContinuityPlanEvent());
  });

  test('LoadMyDelegatedPlanEvent emits Loaded state with isDelegatedView = true', () async {
    final expectedStates = [
      ContinuityPlanLoadingState(),
      isA<ContinuityPlanLoadedState>()
          .having((s) => s.isDelegatedView, 'isDelegatedView', true),
    ];

    expectLater(bloc.stream, emitsInOrder(expectedStates));

    bloc.add(const LoadMyDelegatedPlanEvent());
  });

  test('FilterContinuityPlanEvent filters cards by category and onlyGaps', () async {
    bloc.add(const LoadContinuityPlanEvent());
    await Future.delayed(const Duration(milliseconds: 50));

    expect(bloc.state, isA<ContinuityPlanLoadedState>());
    final loadedState = bloc.state as ContinuityPlanLoadedState;
    expect(loadedState.filteredStages.first.cards.length, 2);

    // Filter by category: Bất Động Sản
    bloc.add(const FilterContinuityPlanEvent(category: 'Bất Động Sản'));
    await Future.delayed(const Duration(milliseconds: 20));

    final catFiltered = bloc.state as ContinuityPlanLoadedState;
    expect(catFiltered.filteredStages.first.cards.length, 1);
    expect(catFiltered.filteredStages.first.cards.first.categoryName, 'Bất Động Sản');

    // Filter by onlyGaps: true
    bloc.add(const FilterContinuityPlanEvent(category: null, onlyGaps: true));
    await Future.delayed(const Duration(milliseconds: 20));

    final gapsFiltered = bloc.state as ContinuityPlanLoadedState;
    expect(gapsFiltered.filteredStages.first.cards.length, 1);
    expect(gapsFiltered.filteredStages.first.cards.first.hasStageGap, true);
  });

  test('UpdateCardStageEvent triggers repository update and refreshes plan', () async {
    bloc.add(const LoadContinuityPlanEvent());
    await Future.delayed(const Duration(milliseconds: 50));

    bloc.add(const UpdateCardStageEvent(
      cardId: 'card-1',
      newStage: UrgencyStage.first72Hours,
      rowVersion: 1,
    ));

    await Future.delayed(const Duration(milliseconds: 50));

    final state = bloc.state as ContinuityPlanLoadedState;
    expect(state.actionMessage, 'Đã cập nhật giai đoạn thẻ thành công');
  });

  test('ToggleCardCompletionEvent triggers toggle and refreshes plan', () async {
    bloc.add(const LoadContinuityPlanEvent());
    await Future.delayed(const Duration(milliseconds: 50));

    bloc.add(const ToggleCardCompletionEvent(
      cardId: 'card-1',
      rowVersion: 1,
    ));

    await Future.delayed(const Duration(milliseconds: 50));

    final state = bloc.state as ContinuityPlanLoadedState;
    expect(state.actionMessage, 'Đã cập nhật trạng thái thẻ');
  });
}
