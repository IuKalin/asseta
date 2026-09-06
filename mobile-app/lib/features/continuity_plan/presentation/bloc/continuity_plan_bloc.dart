import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/repositories/continuity_plan_repository.dart';
import 'continuity_plan_event.dart';
import 'continuity_plan_state.dart';

class ContinuityPlanBloc extends Bloc<ContinuityPlanEvent, ContinuityPlanState> {
  final ContinuityPlanRepository repository;

  ContinuityPlanBloc({required this.repository}) : super(ContinuityPlanInitialState()) {
    on<LoadContinuityPlanEvent>(_onLoadContinuityPlan);
    on<LoadMyDelegatedPlanEvent>(_onLoadMyDelegatedPlan);
    on<UpdateCardStageEvent>(_onUpdateCardStage);
    on<ToggleCardCompletionEvent>(_onToggleCardCompletion);
    on<FilterContinuityPlanEvent>(_onFilterContinuityPlan);
  }

  Future<void> _onLoadContinuityPlan(
    LoadContinuityPlanEvent event,
    Emitter<ContinuityPlanState> emit,
  ) async {
    emit(ContinuityPlanLoadingState());
    final result = await repository.getContinuityPlan();
    result.fold(
      (failure) => emit(ContinuityPlanErrorState(failure.message)),
      (plan) => emit(ContinuityPlanLoadedState(plan: plan, isDelegatedView: false)),
    );
  }

  Future<void> _onLoadMyDelegatedPlan(
    LoadMyDelegatedPlanEvent event,
    Emitter<ContinuityPlanState> emit,
  ) async {
    emit(ContinuityPlanLoadingState());
    final result = await repository.getMyDelegatedPlan();
    result.fold(
      (failure) => emit(ContinuityPlanErrorState(failure.message)),
      (plan) => emit(ContinuityPlanLoadedState(plan: plan, isDelegatedView: true)),
    );
  }

  Future<void> _onUpdateCardStage(
    UpdateCardStageEvent event,
    Emitter<ContinuityPlanState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ContinuityPlanLoadedState) return;

    final updateResult = await repository.updateCardStage(
      cardId: event.cardId,
      newStage: event.newStage,
      rowVersion: event.rowVersion,
    );

    await updateResult.fold(
      (failure) async {
        emit(currentState.copyWith(actionMessage: failure.message));
      },
      (updatedCard) async {
        final refreshResult = currentState.isDelegatedView
            ? await repository.getMyDelegatedPlan()
            : await repository.getContinuityPlan();

        refreshResult.fold(
          (failure) => emit(currentState.copyWith(actionMessage: failure.message)),
          (refreshedPlan) => emit(currentState.copyWith(
            plan: refreshedPlan,
            actionMessage: 'Đã cập nhật giai đoạn thẻ thành công',
          )),
        );
      },
    );
  }

  Future<void> _onToggleCardCompletion(
    ToggleCardCompletionEvent event,
    Emitter<ContinuityPlanState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ContinuityPlanLoadedState) return;

    final toggleResult = await repository.toggleCardCompletion(
      cardId: event.cardId,
      rowVersion: event.rowVersion,
    );

    await toggleResult.fold(
      (failure) async {
        emit(currentState.copyWith(actionMessage: failure.message));
      },
      (updatedCard) async {
        final refreshResult = currentState.isDelegatedView
            ? await repository.getMyDelegatedPlan()
            : await repository.getContinuityPlan();

        refreshResult.fold(
          (failure) => emit(currentState.copyWith(actionMessage: failure.message)),
          (refreshedPlan) => emit(currentState.copyWith(
            plan: refreshedPlan,
            actionMessage: 'Đã cập nhật trạng thái thẻ',
          )),
        );
      },
    );
  }

  void _onFilterContinuityPlan(
    FilterContinuityPlanEvent event,
    Emitter<ContinuityPlanState> emit,
  ) {
    final currentState = state;
    if (currentState is! ContinuityPlanLoadedState) return;

    emit(currentState.copyWith(
      selectedCategory: event.category,
      clearCategory: event.category == null,
      onlyGaps: event.onlyGaps ?? currentState.onlyGaps,
    ));
  }
}
