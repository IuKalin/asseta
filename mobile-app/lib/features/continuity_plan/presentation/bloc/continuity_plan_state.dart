import 'package:equatable/equatable.dart';
import '../../domain/entities/continuity_plan_entity.dart';

abstract class ContinuityPlanState extends Equatable {
  const ContinuityPlanState();

  @override
  List<Object?> get props => [];
}

class ContinuityPlanInitialState extends ContinuityPlanState {}

class ContinuityPlanLoadingState extends ContinuityPlanState {}

class ContinuityPlanLoadedState extends ContinuityPlanState {
  final ContinuityPlanEntity plan;
  final bool isDelegatedView;
  final String? selectedCategory;
  final bool onlyGaps;
  final String? actionMessage;

  const ContinuityPlanLoadedState({
    required this.plan,
    this.isDelegatedView = false,
    this.selectedCategory,
    this.onlyGaps = false,
    this.actionMessage,
  });

  List<ContinuityPlanStageEntity> get filteredStages {
    return plan.stages.map((stage) {
      final filteredCards = stage.cards.where((card) {
        if (selectedCategory != null &&
            card.categoryName != selectedCategory &&
            card.categoryId != selectedCategory) {
          return false;
        }
        if (onlyGaps && !card.hasStageGap) {
          return false;
        }
        return true;
      }).toList();

      return ContinuityPlanStageEntity(
        stage: stage.stage,
        stageName: stage.stageName,
        stageDescription: stage.stageDescription,
        totalCardsCount: stage.totalCardsCount,
        completedCardsCount: stage.completedCardsCount,
        gapCardsCount: stage.gapCardsCount,
        cards: filteredCards,
      );
    }).toList();
  }

  ContinuityPlanLoadedState copyWith({
    ContinuityPlanEntity? plan,
    bool? isDelegatedView,
    String? selectedCategory,
    bool clearCategory = false,
    bool? onlyGaps,
    String? actionMessage,
  }) {
    return ContinuityPlanLoadedState(
      plan: plan ?? this.plan,
      isDelegatedView: isDelegatedView ?? this.isDelegatedView,
      selectedCategory: clearCategory ? null : (selectedCategory ?? this.selectedCategory),
      onlyGaps: onlyGaps ?? this.onlyGaps,
      actionMessage: actionMessage,
    );
  }

  @override
  List<Object?> get props => [
        plan,
        isDelegatedView,
        selectedCategory,
        onlyGaps,
        actionMessage,
      ];
}

class ContinuityPlanErrorState extends ContinuityPlanState {
  final String message;

  const ContinuityPlanErrorState(this.message);

  @override
  List<Object?> get props => [message];
}
