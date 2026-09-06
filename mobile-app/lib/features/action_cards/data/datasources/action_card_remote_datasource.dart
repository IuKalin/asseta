import 'package:dio/dio.dart';
import '../../domain/entities/action_card_entity.dart';
import '../../domain/entities/action_card_enums.dart';
import '../../domain/entities/action_step_entity.dart';
import '../../domain/entities/action_contact_entity.dart';
import '../../domain/entities/action_template_entity.dart';

abstract class ActionCardRemoteDataSource {
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

class ActionCardRemoteDataSourceImpl implements ActionCardRemoteDataSource {
  final Dio dio;

  ActionCardRemoteDataSourceImpl({required this.dio});

  @override
  Future<List<ActionCardEntity>> getActionCards({
    UrgencyStage? urgency,
    String? categoryId,
    String? search,
  }) async {
    final queryParams = <String, dynamic>{};
    if (urgency != null) queryParams['urgency'] = urgency.toShortString();
    if (categoryId != null) queryParams['categoryId'] = categoryId;
    if (search != null) queryParams['search'] = search;

    final response = await dio.get(
      '/v1/action-cards',
      queryParameters: queryParams,
    );

    final list = response.data['data'] as List<dynamic>;
    return list.map((item) => ActionCardEntity.fromJson(item as Map<String, dynamic>)).toList();
  }

  @override
  Future<ActionCardEntity> getActionCardById(String id) async {
    final response = await dio.get('/v1/action-cards/$id');
    final data = response.data['data'] as Map<String, dynamic>;
    return ActionCardEntity.fromJson(data);
  }

  @override
  Future<List<ActionTemplateEntity>> getActionCardTemplates({String? categoryCode}) async {
    final queryParams = <String, dynamic>{};
    if (categoryCode != null) queryParams['categoryCode'] = categoryCode;

    final response = await dio.get(
      '/v1/action-cards/templates',
      queryParameters: queryParams,
    );

    final list = response.data['data'] as List<dynamic>;
    return list.map((t) => ActionTemplateEntity.fromJson(t as Map<String, dynamic>)).toList();
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
    final response = await dio.post(
      '/v1/action-cards',
      data: {
        'categoryId': categoryId,
        'title': title,
        'urgency': urgency.toShortString(),
        'priority': priority,
        'summary': summary,
        'assignedTrustedPersonId': assignedTrustedPersonId,
        'documentLocationHint': documentLocationHint,
        'digitalStorageLink': digitalStorageLink,
        'cipherInstructionsBlob': cipherInstructionsBlob,
        'cipherNonce': cipherNonce,
        'cipherAuthTag': cipherAuthTag,
      },
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return ActionCardEntity.fromJson(data);
  }

  @override
  Future<ActionCardEntity> createActionCardFromItem({
    required String continuityItemId,
    String? templateCode,
    UrgencyStage? urgency,
    String? priority,
    String? summary,
  }) async {
    final response = await dio.post(
      '/v1/action-cards/from-item',
      data: {
        'continuityItemId': continuityItemId,
        'templateCode': templateCode,
        'urgency': urgency?.toShortString(),
        'priority': priority,
        'summary': summary,
      },
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return ActionCardEntity.fromJson(data);
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
    final response = await dio.put(
      '/v1/action-cards/$id',
      data: {
        'title': title,
        'urgency': urgency.toShortString(),
        'priority': priority,
        'rowVersion': rowVersion,
        'summary': summary,
        'assignedTrustedPersonId': assignedTrustedPersonId,
        'documentLocationHint': documentLocationHint,
        'digitalStorageLink': digitalStorageLink,
        'cipherInstructionsBlob': cipherInstructionsBlob,
        'cipherNonce': cipherNonce,
        'cipherAuthTag': cipherAuthTag,
      },
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return ActionCardEntity.fromJson(data);
  }

  @override
  Future<void> deleteActionCard(String id) async {
    await dio.delete('/v1/action-cards/$id');
  }

  @override
  Future<ActionStepEntity> addStep({
    required String cardId,
    required String instruction,
    String? estimatedDuration,
  }) async {
    final response = await dio.post(
      '/v1/action-cards/$cardId/steps',
      data: {
        'instruction': instruction,
        'estimatedDuration': estimatedDuration,
      },
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return ActionStepEntity.fromJson(data);
  }

  @override
  Future<ActionStepEntity> updateStep({
    required String cardId,
    required String stepId,
    required String instruction,
    String? estimatedDuration,
    bool? isCompleted,
  }) async {
    final response = await dio.put(
      '/v1/action-cards/$cardId/steps/$stepId',
      data: {
        'instruction': instruction,
        'estimatedDuration': estimatedDuration,
        'isCompleted': isCompleted,
      },
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return ActionStepEntity.fromJson(data);
  }

  @override
  Future<void> deleteStep({
    required String cardId,
    required String stepId,
  }) async {
    await dio.delete('/v1/action-cards/$cardId/steps/$stepId');
  }

  @override
  Future<List<ActionStepEntity>> reorderSteps({
    required String cardId,
    required List<String> orderedStepIds,
  }) async {
    final response = await dio.post(
      '/v1/action-cards/$cardId/steps/reorder',
      data: {
        'orderedStepIds': orderedStepIds,
      },
    );

    final list = response.data['data'] as List<dynamic>;
    return list.map((s) => ActionStepEntity.fromJson(s as Map<String, dynamic>)).toList();
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
    final response = await dio.post(
      '/v1/action-cards/$cardId/contacts',
      data: {
        'contactName': contactName,
        'relationshipOrRole': relationshipOrRole,
        'phoneNumber': phoneNumber,
        'email': email,
        'contactNotes': contactNotes,
      },
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return ActionContactEntity.fromJson(data);
  }

  @override
  Future<void> deleteContact({
    required String cardId,
    required String contactId,
  }) async {
    await dio.delete('/v1/action-cards/$cardId/contacts/$contactId');
  }
}
