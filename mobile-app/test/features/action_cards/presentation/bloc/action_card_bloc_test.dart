import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_card_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_card_enums.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_step_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_contact_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_template_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/repositories/action_card_repository.dart';
import 'package:asseta_mobile/features/action_cards/presentation/bloc/action_card_bloc.dart';
import 'package:asseta_mobile/features/action_cards/presentation/bloc/action_card_event.dart';
import 'package:asseta_mobile/features/action_cards/presentation/bloc/action_card_state.dart';

class MockActionCardRepository implements ActionCardRepository {
  final List<ActionCardEntity> _cards;

  MockActionCardRepository(this._cards);

  @override
  Future<List<ActionCardEntity>> getActionCards({
    UrgencyStage? urgency,
    String? categoryId,
    String? search,
  }) async {
    return _cards;
  }

  @override
  Future<ActionCardEntity> getActionCardById(String id) async {
    return _cards.firstWhere((c) => c.id == id);
  }

  @override
  Future<List<ActionTemplateEntity>> getActionCardTemplates({String? categoryCode}) async {
    return [];
  }

  @override
  Future<ActionCardEntity> createActionCard({
    required String categoryId,
    required String title,
    required UrgencyStage urgency,
    required String priority,
    String? summary,
    String? assignedTrustedPersonId,
    String? documentLocationHint,
    String? digitalStorageLink,
    String? cipherInstructionsBlob,
    String? cipherNonce,
    String? cipherAuthTag,
  }) async {
    final created = ActionCardEntity(
      id: 'new-id',
      ownerId: 'owner-1',
      categoryId: categoryId,
      categoryCode: 'FINANCIAL',
      categoryNameVi: 'Tài chính',
      title: title,
      urgency: urgency,
      priority: priority,
      hasConfidentialInstructions: false,
      isCompleted: false,
      rowVersion: 1,
      createdAtUtc: DateTime.now(),
      steps: const [],
      contacts: const [],
    );
    return created;
  }

  @override
  Future<ActionCardEntity> createActionCardFromItem({
    required String continuityItemId,
    String? templateCode,
    UrgencyStage? urgency,
    String? priority,
    String? summary,
  }) async {
    throw UnimplementedError();
  }

  @override
  Future<ActionCardEntity> updateActionCard({
    required String id,
    required String title,
    required UrgencyStage urgency,
    required String priority,
    required int rowVersion,
    String? summary,
    String? assignedTrustedPersonId,
    String? documentLocationHint,
    String? digitalStorageLink,
    String? cipherInstructionsBlob,
    String? cipherNonce,
    String? cipherAuthTag,
  }) async {
    throw UnimplementedError();
  }

  @override
  Future<void> deleteActionCard(String id) async {}

  @override
  Future<ActionStepEntity> addStep({
    required String cardId,
    required String instruction,
    String? estimatedDuration,
  }) async {
    return ActionStepEntity(
      id: 'new-step',
      actionCardId: cardId,
      stepOrder: 1,
      instruction: instruction,
      isCompleted: false,
    );
  }

  @override
  Future<ActionStepEntity> updateStep({
    required String cardId,
    required String stepId,
    required String instruction,
    String? estimatedDuration,
    bool? isCompleted,
  }) async {
    return ActionStepEntity(
      id: stepId,
      actionCardId: cardId,
      stepOrder: 1,
      instruction: instruction,
      estimatedDuration: estimatedDuration,
      isCompleted: isCompleted ?? false,
    );
  }

  @override
  Future<void> deleteStep({required String cardId, required String stepId}) async {}

  @override
  Future<List<ActionStepEntity>> reorderSteps({
    required String cardId,
    required List<String> orderedStepIds,
  }) async {
    return [];
  }

  @override
  Future<ActionContactEntity> addContact({
    required String cardId,
    required String contactName,
    required String relationshipOrRole,
    String? phoneNumber,
    String? email,
    String? contactNotes,
  }) async {
    return ActionContactEntity(
      id: 'contact-id',
      actionCardId: cardId,
      contactName: contactName,
      relationshipOrRole: relationshipOrRole,
    );
  }

  @override
  Future<void> deleteContact({required String cardId, required String contactId}) async {}
}

void main() {
  group('ActionCardBloc Tests', () {
    final sampleCards = [
      ActionCardEntity(
        id: 'card-1',
        ownerId: 'owner-1',
        categoryId: 'cat-1',
        categoryCode: 'FINANCIAL',
        categoryNameVi: 'Tài chính',
        title: 'Nợ ngân hàng',
        urgency: UrgencyStage.immediate,
        priority: 'CRITICAL',
        hasConfidentialInstructions: false,
        isCompleted: false,
        rowVersion: 1,
        createdAtUtc: DateTime.now(),
        steps: const [],
        contacts: const [],
      ),
      ActionCardEntity(
        id: 'card-2',
        ownerId: 'owner-1',
        categoryId: 'cat-2',
        categoryCode: 'PROPERTY',
        categoryNameVi: 'Bất động sản',
        title: 'Nhà cho thuê',
        urgency: UrgencyStage.first72Hours,
        priority: 'IMPORTANT',
        hasConfidentialInstructions: false,
        isCompleted: false,
        rowVersion: 1,
        createdAtUtc: DateTime.now(),
        steps: const [],
        contacts: const [],
      ),
    ];

    test('Initial state should be ActionCardInitial', () {
      final bloc = ActionCardBloc(repository: MockActionCardRepository(sampleCards));
      expect(bloc.state, isA<ActionCardInitial>());
    });

    test('LoadActionCardsEvent emits ActionCardLoading then ActionCardLoaded', () async {
      final bloc = ActionCardBloc(repository: MockActionCardRepository(sampleCards));

      final expectedStates = [
        isA<ActionCardLoading>(),
        isA<ActionCardLoaded>().having((s) => s.allCards.length, 'allCards length', 2),
      ];

      expectLater(bloc.stream, emitsInOrder(expectedStates));

      bloc.add(const LoadActionCardsEvent());
    });

    test('FilterUrgencyStageEvent filters cards by stage', () async {
      final bloc = ActionCardBloc(repository: MockActionCardRepository(sampleCards));

      bloc.add(const LoadActionCardsEvent());
      await Future.delayed(const Duration(milliseconds: 50));

      bloc.add(const FilterUrgencyStageEvent(UrgencyStage.immediate));
      await Future.delayed(const Duration(milliseconds: 50));

      expect(bloc.state, isA<ActionCardLoaded>());
      final loaded = bloc.state as ActionCardLoaded;
      expect(loaded.filteredCards.length, 1);
      expect(loaded.filteredCards.first.urgency, UrgencyStage.immediate);
    });

    test('SearchActionCardsEvent filters cards by text query', () async {
      final bloc = ActionCardBloc(repository: MockActionCardRepository(sampleCards));

      bloc.add(const LoadActionCardsEvent());
      await Future.delayed(const Duration(milliseconds: 50));

      bloc.add(const SearchActionCardsEvent('thuê'));
      await Future.delayed(const Duration(milliseconds: 50));

      expect(bloc.state, isA<ActionCardLoaded>());
      final loaded = bloc.state as ActionCardLoaded;
      expect(loaded.filteredCards.length, 1);
      expect(loaded.filteredCards.first.title, contains('thuê'));
    });
  });
}
