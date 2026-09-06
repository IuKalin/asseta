import 'package:equatable/equatable.dart';
import '../../domain/entities/continuity_plan_entity.dart';

abstract class ContinuityPlanEvent extends Equatable {
  const ContinuityPlanEvent();

  @override
  List<Object?> get props => [];
}

class LoadContinuityPlanEvent extends ContinuityPlanEvent {
  const LoadContinuityPlanEvent();
}

class LoadMyDelegatedPlanEvent extends ContinuityPlanEvent {
  const LoadMyDelegatedPlanEvent();
}

class UpdateCardStageEvent extends ContinuityPlanEvent {
  final String cardId;
  final UrgencyStage newStage;
  final int rowVersion;

  const UpdateCardStageEvent({
    required this.cardId,
    required this.newStage,
    required this.rowVersion,
  });

  @override
  List<Object?> get props => [cardId, newStage, rowVersion];
}

class ToggleCardCompletionEvent extends ContinuityPlanEvent {
  final String cardId;
  final int rowVersion;

  const ToggleCardCompletionEvent({
    required this.cardId,
    required this.rowVersion,
  });

  @override
  List<Object?> get props => [cardId, rowVersion];
}

class FilterContinuityPlanEvent extends ContinuityPlanEvent {
  final String? category;
  final bool? onlyGaps;

  const FilterContinuityPlanEvent({
    this.category,
    this.onlyGaps,
  });

  @override
  List<Object?> get props => [category, onlyGaps];
}
