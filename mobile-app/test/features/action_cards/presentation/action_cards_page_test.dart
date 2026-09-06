import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_card_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_card_enums.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_step_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_contact_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_template_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/repositories/action_card_repository.dart';
import 'package:asseta_mobile/features/action_cards/presentation/bloc/action_card_bloc.dart';
import 'package:asseta_mobile/features/action_cards/presentation/pages/action_cards_page.dart';

class MockRepoForActionCardsTest implements ActionCardRepository {
  final List<ActionCardEntity> cards;

  MockRepoForActionCardsTest(this.cards);

  @override
  Future<List<ActionCardEntity>> getActionCards({UrgencyStage? urgency, String? categoryId, String? search}) async {
    return cards;
  }

  @override
  Future<ActionCardEntity> getActionCardById(String id) async => cards.first;

  @override
  Future<List<ActionTemplateEntity>> getActionCardTemplates({String? categoryCode}) async => [];

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
  }) async => cards.first;

  @override
  Future<ActionCardEntity> createActionCardFromItem({
    required String continuityItemId,
    String? templateCode,
    UrgencyStage? urgency,
    String? priority,
    String? summary,
  }) async => cards.first;

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
  }) async => cards.first;

  @override
  Future<void> deleteActionCard(String id) async {}

  @override
  Future<ActionStepEntity> addStep({required String cardId, required String instruction, String? estimatedDuration}) async {
    throw UnimplementedError();
  }

  @override
  Future<ActionStepEntity> updateStep({
    required String cardId,
    required String stepId,
    required String instruction,
    String? estimatedDuration,
    bool? isCompleted,
  }) async {
    throw UnimplementedError();
  }

  @override
  Future<void> deleteStep({required String cardId, required String stepId}) async {}

  @override
  Future<List<ActionStepEntity>> reorderSteps({required String cardId, required List<String> orderedStepIds}) async => [];

  @override
  Future<ActionContactEntity> addContact({
    required String cardId,
    required String contactName,
    required String relationshipOrRole,
    String? phoneNumber,
    String? email,
    String? contactNotes,
  }) async {
    throw UnimplementedError();
  }

  @override
  Future<void> deleteContact({required String cardId, required String contactId}) async {}
}

void main() {
  testWidgets('ActionCardsPage renders header, urgency filters, and cards', (WidgetTester tester) async {
    final sampleCards = [
      ActionCardEntity(
        id: 'card-1',
        ownerId: 'owner-1',
        categoryId: 'cat-1',
        categoryCode: 'FINANCIAL',
        categoryNameVi: 'Tài chính & Ngân hàng',
        title: 'Xử lý khoản vay ngân hàng',
        summary: 'Kiểm tra dư nợ gốc',
        urgency: UrgencyStage.first72Hours,
        priority: 'CRITICAL',
        hasConfidentialInstructions: true,
        isCompleted: false,
        rowVersion: 1,
        createdAtUtc: DateTime.now(),
        steps: const [
          ActionStepEntity(
            id: 's1',
            actionCardId: 'card-1',
            stepOrder: 1,
            instruction: 'Liên hệ cán bộ tín dụng',
            isCompleted: true,
          )
        ],
        contacts: const [],
      ),
    ];

    final repo = MockRepoForActionCardsTest(sampleCards);

    await tester.pumpWidget(
      MaterialApp(
        home: BlocProvider(
          create: (_) => ActionCardBloc(repository: repo),
          child: const ActionCardsPage(),
        ),
      ),
    );

    await tester.pumpAndSettle();

    // Verify Title and Urgency tabs exist
    expect(find.text('Action Cards'), findsOneWidget);
    expect(find.text('Tất cả'), findsOneWidget);
    expect(find.text('⚡ Ngay lập tức'), findsOneWidget);
    expect(find.text('⏳ 72 Giờ đầu'), findsOneWidget);

    // Verify card content
    expect(find.text('Xử lý khoản vay ngân hàng'), findsOneWidget);
    expect(find.text('CRITICAL'), findsOneWidget);
    expect(find.text('Tiến độ: 1/1 bước'), findsOneWidget);
  });
}
