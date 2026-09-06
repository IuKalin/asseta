import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/entities/action_card_entity.dart';
import '../../domain/entities/action_card_enums.dart';
import '../../domain/repositories/action_card_repository.dart';
import 'action_card_event.dart';
import 'action_card_state.dart';

class ActionCardBloc extends Bloc<ActionCardEvent, ActionCardState> {
  final ActionCardRepository repository;

  ActionCardBloc({required this.repository}) : super(ActionCardInitial()) {
    on<LoadActionCardsEvent>(_onLoadActionCards);
    on<FilterUrgencyStageEvent>(_onFilterUrgencyStage);
    on<SearchActionCardsEvent>(_onSearchActionCards);
    on<CreateActionCardEvent>(_onCreateActionCard);
    on<CreateActionCardFromItemEvent>(_onCreateActionCardFromItem);
    on<UpdateActionCardEvent>(_onUpdateActionCard);
    on<DeleteActionCardEvent>(_onDeleteActionCard);
    on<ToggleStepCompletionEvent>(_onToggleStepCompletion);
    on<AddStepEvent>(_onAddStep);
    on<DeleteStepEvent>(_onDeleteStep);
    on<ReorderStepsEvent>(_onReorderSteps);
    on<AddContactEvent>(_onAddContact);
    on<DeleteContactEvent>(_onDeleteContact);
  }

  List<ActionCardEntity> _applyFilters(
    List<ActionCardEntity> cards,
    UrgencyStage? urgency,
    String query,
  ) {
    var result = cards;
    if (urgency != null) {
      result = result.where((c) => c.urgency == urgency).toList();
    }
    if (query.trim().isNotEmpty) {
      final q = query.trim().toLowerCase();
      result = result.where((c) =>
          c.title.toLowerCase().contains(q) ||
          (c.summary != null && c.summary!.toLowerCase().contains(q)) ||
          (c.documentLocationHint != null && c.documentLocationHint!.toLowerCase().contains(q))
      ).toList();
    }
    return result;
  }

  Future<void> _onLoadActionCards(
    LoadActionCardsEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    emit(ActionCardLoading());
    try {
      final cards = await repository.getActionCards(
        urgency: event.urgency,
        search: event.search,
      );
      final filtered = _applyFilters(cards, event.urgency, event.search ?? '');
      emit(ActionCardLoaded(
        allCards: cards,
        filteredCards: filtered,
        selectedUrgency: event.urgency,
        searchQuery: event.search ?? '',
      ));
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  void _onFilterUrgencyStage(
    FilterUrgencyStageEvent event,
    Emitter<ActionCardState> emit,
  ) {
    if (state is ActionCardLoaded) {
      final current = state as ActionCardLoaded;
      final filtered = _applyFilters(current.allCards, event.urgency, current.searchQuery);
      emit(current.copyWith(
        selectedUrgency: event.urgency,
        clearUrgency: event.urgency == null,
        filteredCards: filtered,
      ));
    }
  }

  void _onSearchActionCards(
    SearchActionCardsEvent event,
    Emitter<ActionCardState> emit,
  ) {
    if (state is ActionCardLoaded) {
      final current = state as ActionCardLoaded;
      final filtered = _applyFilters(current.allCards, current.selectedUrgency, event.query);
      emit(current.copyWith(
        searchQuery: event.query,
        filteredCards: filtered,
      ));
    }
  }

  Future<void> _onCreateActionCard(
    CreateActionCardEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      final created = await repository.createActionCard(
        categoryId: event.categoryId,
        title: event.title,
        urgency: event.urgency,
        priority: event.priority,
        summary: event.summary,
        assignedTrustedPersonId: event.assignedTrustedPersonId,
        documentLocationHint: event.documentLocationHint,
        digitalStorageLink: event.digitalStorageLink,
        cipherInstructionsBlob: event.cipherInstructionsBlob,
        cipherNonce: event.cipherNonce,
        cipherAuthTag: event.cipherAuthTag,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = [created, ...current.allCards];
        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(allCards: updatedAll, filteredCards: filtered));
      } else {
        emit(ActionCardLoaded(allCards: [created], filteredCards: [created]));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onCreateActionCardFromItem(
    CreateActionCardFromItemEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      final created = await repository.createActionCardFromItem(
        continuityItemId: event.continuityItemId,
        templateCode: event.templateCode,
        urgency: event.urgency,
        priority: event.priority,
        summary: event.summary,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = [created, ...current.allCards];
        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(allCards: updatedAll, filteredCards: filtered));
      } else {
        emit(ActionCardLoaded(allCards: [created], filteredCards: [created]));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onUpdateActionCard(
    UpdateActionCardEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      final updated = await repository.updateActionCard(
        id: event.id,
        title: event.title,
        urgency: event.urgency,
        priority: event.priority,
        rowVersion: event.rowVersion,
        summary: event.summary,
        assignedTrustedPersonId: event.assignedTrustedPersonId,
        documentLocationHint: event.documentLocationHint,
        digitalStorageLink: event.digitalStorageLink,
        cipherInstructionsBlob: event.cipherInstructionsBlob,
        cipherNonce: event.cipherNonce,
        cipherAuthTag: event.cipherAuthTag,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = current.allCards.map((c) => c.id == event.id ? updated : c).toList();
        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(
          allCards: updatedAll,
          filteredCards: filtered,
          selectedCard: current.selectedCard?.id == event.id ? updated : current.selectedCard,
        ));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onDeleteActionCard(
    DeleteActionCardEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      await repository.deleteActionCard(event.cardId);

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = current.allCards.where((c) => c.id != event.cardId).toList();
        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(allCards: updatedAll, filteredCards: filtered));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onToggleStepCompletion(
    ToggleStepCompletionEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      final updatedStep = await repository.updateStep(
        cardId: event.cardId,
        stepId: event.stepId,
        instruction: event.instruction,
        estimatedDuration: event.estimatedDuration,
        isCompleted: event.isCompleted,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = current.allCards.map((card) {
          if (card.id == event.cardId) {
            final updatedSteps = card.steps.map((s) => s.id == event.stepId ? updatedStep : s).toList();
            final allDone = updatedSteps.isNotEmpty && updatedSteps.every((s) => s.isCompleted);
            return card.copyWith(steps: updatedSteps, isCompleted: allDone);
          }
          return card;
        }).toList();

        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(
          allCards: updatedAll,
          filteredCards: filtered,
          selectedCard: current.selectedCard?.id == event.cardId
              ? current.selectedCard!.copyWith(
                  steps: current.selectedCard!.steps.map((s) => s.id == event.stepId ? updatedStep : s).toList(),
                )
              : current.selectedCard,
        ));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onAddStep(
    AddStepEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      final newStep = await repository.addStep(
        cardId: event.cardId,
        instruction: event.instruction,
        estimatedDuration: event.estimatedDuration,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = current.allCards.map((card) {
          if (card.id == event.cardId) {
            return card.copyWith(steps: [...card.steps, newStep]);
          }
          return card;
        }).toList();

        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(allCards: updatedAll, filteredCards: filtered));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onDeleteStep(
    DeleteStepEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      await repository.deleteStep(
        cardId: event.cardId,
        stepId: event.stepId,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = current.allCards.map((card) {
          if (card.id == event.cardId) {
            final updatedSteps = card.steps.where((s) => s.id != event.stepId).toList();
            return card.copyWith(steps: updatedSteps);
          }
          return card;
        }).toList();

        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(allCards: updatedAll, filteredCards: filtered));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onReorderSteps(
    ReorderStepsEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      final reordered = await repository.reorderSteps(
        cardId: event.cardId,
        orderedStepIds: event.orderedStepIds,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = current.allCards.map((card) {
          if (card.id == event.cardId) {
            return card.copyWith(steps: reordered);
          }
          return card;
        }).toList();

        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(allCards: updatedAll, filteredCards: filtered));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onAddContact(
    AddContactEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      final newContact = await repository.addContact(
        cardId: event.cardId,
        contactName: event.contactName,
        relationshipOrRole: event.relationshipOrRole,
        phoneNumber: event.phoneNumber,
        email: event.email,
        contactNotes: event.contactNotes,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = current.allCards.map((card) {
          if (card.id == event.cardId) {
            return card.copyWith(contacts: [...card.contacts, newContact]);
          }
          return card;
        }).toList();

        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(allCards: updatedAll, filteredCards: filtered));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }

  Future<void> _onDeleteContact(
    DeleteContactEvent event,
    Emitter<ActionCardState> emit,
  ) async {
    try {
      await repository.deleteContact(
        cardId: event.cardId,
        contactId: event.contactId,
      );

      if (state is ActionCardLoaded) {
        final current = state as ActionCardLoaded;
        final updatedAll = current.allCards.map((card) {
          if (card.id == event.cardId) {
            return card.copyWith(
              contacts: card.contacts.where((c) => c.id != event.contactId).toList(),
            );
          }
          return card;
        }).toList();

        final filtered = _applyFilters(updatedAll, current.selectedUrgency, current.searchQuery);
        emit(current.copyWith(allCards: updatedAll, filteredCards: filtered));
      }
    } catch (e) {
      emit(ActionCardError(e.toString()));
    }
  }
}
