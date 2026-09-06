import 'package:equatable/equatable.dart';
import '../../domain/entities/action_card_entity.dart';
import '../../domain/entities/action_card_enums.dart';

abstract class ActionCardState extends Equatable {
  const ActionCardState();

  @override
  List<Object?> get props => [];
}

class ActionCardInitial extends ActionCardState {}

class ActionCardLoading extends ActionCardState {}

class ActionCardLoaded extends ActionCardState {
  final List<ActionCardEntity> allCards;
  final List<ActionCardEntity> filteredCards;
  final UrgencyStage? selectedUrgency;
  final String searchQuery;
  final ActionCardEntity? selectedCard;

  const ActionCardLoaded({
    required this.allCards,
    required this.filteredCards,
    this.selectedUrgency,
    this.searchQuery = '',
    this.selectedCard,
  });

  ActionCardLoaded copyWith({
    List<ActionCardEntity>? allCards,
    List<ActionCardEntity>? filteredCards,
    UrgencyStage? selectedUrgency,
    bool clearUrgency = false,
    String? searchQuery,
    ActionCardEntity? selectedCard,
  }) {
    return ActionCardLoaded(
      allCards: allCards ?? this.allCards,
      filteredCards: filteredCards ?? this.filteredCards,
      selectedUrgency: clearUrgency ? null : (selectedUrgency ?? this.selectedUrgency),
      searchQuery: searchQuery ?? this.searchQuery,
      selectedCard: selectedCard ?? this.selectedCard,
    );
  }

  @override
  List<Object?> get props => [
        allCards,
        filteredCards,
        selectedUrgency,
        searchQuery,
        selectedCard,
      ];
}

class ActionCardError extends ActionCardState {
  final String message;

  const ActionCardError(this.message);

  @override
  List<Object?> get props => [message];
}
