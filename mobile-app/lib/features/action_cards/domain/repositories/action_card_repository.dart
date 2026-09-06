import '../entities/action_card_entity.dart';
import '../entities/action_card_enums.dart';
import '../entities/action_step_entity.dart';
import '../entities/action_contact_entity.dart';
import '../entities/action_template_entity.dart';

abstract class ActionCardRepository {
  Future<List<ActionCardEntity>> getActionCards({
    UrgencyStage? urgency,
    String? categoryId,
    String? search,
  });

  Future<ActionCardEntity> getActionCardById(String id);

  Future<List<ActionTemplateEntity>> getActionCardTemplates({String? categoryCode});

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
  });

  Future<ActionCardEntity> createActionCardFromItem({
    required String continuityItemId,
    String? templateCode,
    UrgencyStage? urgency,
    String? priority,
    String? summary,
  });

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
  });

  Future<void> deleteActionCard(String id);

  Future<ActionStepEntity> addStep({
    required String cardId,
    required String instruction,
    String? estimatedDuration,
  });

  Future<ActionStepEntity> updateStep({
    required String cardId,
    required String stepId,
    required String instruction,
    String? estimatedDuration,
    bool? isCompleted,
  });

  Future<void> deleteStep({
    required String cardId,
    required String stepId,
  });

  Future<List<ActionStepEntity>> reorderSteps({
    required String cardId,
    required List<String> orderedStepIds,
  });

  Future<ActionContactEntity> addContact({
    required String cardId,
    required String contactName,
    required String relationshipOrRole,
    String? phoneNumber,
    String? email,
    String? contactNotes,
  });

  Future<void> deleteContact({
    required String cardId,
    required String contactId,
  });
}
