import '../../domain/entities/action_card_entity.dart';
import '../../domain/entities/action_card_enums.dart';
import '../../domain/entities/action_step_entity.dart';
import '../../domain/entities/action_contact_entity.dart';
import '../../domain/entities/action_template_entity.dart';
import '../../domain/repositories/action_card_repository.dart';
import '../datasources/action_card_local_datasource.dart';
import '../datasources/action_card_remote_datasource.dart';

class ActionCardRepositoryImpl implements ActionCardRepository {
  final ActionCardRemoteDataSource remoteDataSource;
  final ActionCardLocalDataSource localDataSource;

  ActionCardRepositoryImpl({
    required this.remoteDataSource,
    required this.localDataSource,
  });

  @override
  Future<List<ActionCardEntity>> getActionCards({
    UrgencyStage? urgency,
    String? categoryId,
    String? search,
  }) async {
    try {
      final remoteCards = await remoteDataSource.getActionCards(
        urgency: urgency,
        categoryId: categoryId,
        search: search,
      );
      await localDataSource.cacheCards(remoteCards);
      return remoteCards;
    } catch (_) {
      final cached = await localDataSource.getCachedCards();
      if (cached != null) {
        var filtered = cached;
        if (urgency != null) {
          filtered = filtered.where((c) => c.urgency == urgency).toList();
        }
        if (categoryId != null) {
          filtered = filtered.where((c) => c.categoryId == categoryId).toList();
        }
        if (search != null && search.isNotEmpty) {
          final s = search.toLowerCase();
          filtered = filtered.where((c) => c.title.toLowerCase().contains(s)).toList();
        }
        return filtered;
      }
      rethrow;
    }
  }

  @override
  Future<ActionCardEntity> getActionCardById(String id) async {
    return await remoteDataSource.getActionCardById(id);
  }

  @override
  Future<List<ActionTemplateEntity>> getActionCardTemplates({String? categoryCode}) async {
    return await remoteDataSource.getActionCardTemplates(categoryCode: categoryCode);
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
    return await remoteDataSource.createActionCard(
      categoryId: categoryId,
      title: title,
      urgency: urgency,
      priority: priority,
      summary: summary,
      assignedTrustedPersonId: assignedTrustedPersonId,
      documentLocationHint: documentLocationHint,
      digitalStorageLink: digitalStorageLink,
      cipherInstructionsBlob: cipherInstructionsBlob,
      cipherNonce: cipherNonce,
      cipherAuthTag: cipherAuthTag,
    );
  }

  @override
  Future<ActionCardEntity> createActionCardFromItem({
    required String continuityItemId,
    String? templateCode,
    UrgencyStage? urgency,
    String? priority,
    String? summary,
  }) async {
    return await remoteDataSource.createActionCardFromItem(
      continuityItemId: continuityItemId,
      templateCode: templateCode,
      urgency: urgency,
      priority: priority,
      summary: summary,
    );
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
    return await remoteDataSource.updateActionCard(
      id: id,
      title: title,
      urgency: urgency,
      priority: priority,
      rowVersion: rowVersion,
      summary: summary,
      assignedTrustedPersonId: assignedTrustedPersonId,
      documentLocationHint: documentLocationHint,
      digitalStorageLink: digitalStorageLink,
      cipherInstructionsBlob: cipherInstructionsBlob,
      cipherNonce: cipherNonce,
      cipherAuthTag: cipherAuthTag,
    );
  }

  @override
  Future<void> deleteActionCard(String id) async {
    await remoteDataSource.deleteActionCard(id);
  }

  @override
  Future<ActionStepEntity> addStep({
    required String cardId,
    required String instruction,
    String? estimatedDuration,
  }) async {
    return await remoteDataSource.addStep(
      cardId: cardId,
      instruction: instruction,
      estimatedDuration: estimatedDuration,
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
    return await remoteDataSource.updateStep(
      cardId: cardId,
      stepId: stepId,
      instruction: instruction,
      estimatedDuration: estimatedDuration,
      isCompleted: isCompleted,
    );
  }

  @override
  Future<void> deleteStep({
    required String cardId,
    required String stepId,
  }) async {
    await remoteDataSource.deleteStep(cardId: cardId, stepId: stepId);
  }

  @override
  Future<List<ActionStepEntity>> reorderSteps({
    required String cardId,
    required List<String> orderedStepIds,
  }) async {
    return await remoteDataSource.reorderSteps(
      cardId: cardId,
      orderedStepIds: orderedStepIds,
    );
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
    return await remoteDataSource.addContact(
      cardId: cardId,
      contactName: contactName,
      relationshipOrRole: relationshipOrRole,
      phoneNumber: phoneNumber,
      email: email,
      contactNotes: contactNotes,
    );
  }

  @override
  Future<void> deleteContact({
    required String cardId,
    required String contactId,
  }) async {
    await remoteDataSource.deleteContact(cardId: cardId, contactId: contactId);
  }
}
