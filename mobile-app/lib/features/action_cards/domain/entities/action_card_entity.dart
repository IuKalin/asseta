import 'package:equatable/equatable.dart';
import 'action_card_enums.dart';
import 'action_step_entity.dart';
import 'action_contact_entity.dart';

class ActionCardEntity extends Equatable {
  final String id;
  final String ownerId;
  final String categoryId;
  final String categoryCode;
  final String categoryNameVi;
  final String? continuityItemId;
  final String? continuityItemName;
  final String title;
  final String? summary;
  final UrgencyStage urgency;
  final String priority;
  final String? assignedTrustedPersonId;
  final String? documentLocationHint;
  final String? digitalStorageLink;
  final bool hasConfidentialInstructions;
  final String? cipherInstructionsBlob;
  final String? cipherNonce;
  final String? cipherAuthTag;
  final bool isCompleted;
  final int rowVersion;
  final DateTime createdAtUtc;
  final DateTime? updatedAtUtc;
  final List<ActionStepEntity> steps;
  final List<ActionContactEntity> contacts;

  const ActionCardEntity({
    required this.id,
    required this.ownerId,
    required this.categoryId,
    required this.categoryCode,
    required this.categoryNameVi,
    this.continuityItemId,
    this.continuityItemName,
    required this.title,
    this.summary,
    required this.urgency,
    required this.priority,
    this.assignedTrustedPersonId,
    this.documentLocationHint,
    this.digitalStorageLink,
    required this.hasConfidentialInstructions,
    this.cipherInstructionsBlob,
    this.cipherNonce,
    this.cipherAuthTag,
    required this.isCompleted,
    required this.rowVersion,
    required this.createdAtUtc,
    this.updatedAtUtc,
    required this.steps,
    required this.contacts,
  });

  ActionCardEntity copyWith({
    String? id,
    String? ownerId,
    String? categoryId,
    String? categoryCode,
    String? categoryNameVi,
    String? continuityItemId,
    String? continuityItemName,
    String? title,
    String? summary,
    UrgencyStage? urgency,
    String? priority,
    String? assignedTrustedPersonId,
    String? documentLocationHint,
    String? digitalStorageLink,
    bool? hasConfidentialInstructions,
    String? cipherInstructionsBlob,
    String? cipherNonce,
    String? cipherAuthTag,
    bool? isCompleted,
    int? rowVersion,
    DateTime? createdAtUtc,
    DateTime? updatedAtUtc,
    List<ActionStepEntity>? steps,
    List<ActionContactEntity>? contacts,
  }) {
    return ActionCardEntity(
      id: id ?? this.id,
      ownerId: ownerId ?? this.ownerId,
      categoryId: categoryId ?? this.categoryId,
      categoryCode: categoryCode ?? this.categoryCode,
      categoryNameVi: categoryNameVi ?? this.categoryNameVi,
      continuityItemId: continuityItemId ?? this.continuityItemId,
      continuityItemName: continuityItemName ?? this.continuityItemName,
      title: title ?? this.title,
      summary: summary ?? this.summary,
      urgency: urgency ?? this.urgency,
      priority: priority ?? this.priority,
      assignedTrustedPersonId: assignedTrustedPersonId ?? this.assignedTrustedPersonId,
      documentLocationHint: documentLocationHint ?? this.documentLocationHint,
      digitalStorageLink: digitalStorageLink ?? this.digitalStorageLink,
      hasConfidentialInstructions: hasConfidentialInstructions ?? this.hasConfidentialInstructions,
      cipherInstructionsBlob: cipherInstructionsBlob ?? this.cipherInstructionsBlob,
      cipherNonce: cipherNonce ?? this.cipherNonce,
      cipherAuthTag: cipherAuthTag ?? this.cipherAuthTag,
      isCompleted: isCompleted ?? this.isCompleted,
      rowVersion: rowVersion ?? this.rowVersion,
      createdAtUtc: createdAtUtc ?? this.createdAtUtc,
      updatedAtUtc: updatedAtUtc ?? this.updatedAtUtc,
      steps: steps ?? this.steps,
      contacts: contacts ?? this.contacts,
    );
  }

  factory ActionCardEntity.fromJson(Map<String, dynamic> json) {
    final stepsRaw = json['steps'] as List<dynamic>? ?? [];
    final contactsRaw = json['contacts'] as List<dynamic>? ?? [];

    return ActionCardEntity(
      id: json['id'] as String,
      ownerId: json['ownerId'] as String,
      categoryId: json['categoryId'] as String,
      categoryCode: json['categoryCode'] as String? ?? '',
      categoryNameVi: json['categoryNameVi'] as String? ?? '',
      continuityItemId: json['continuityItemId'] as String?,
      continuityItemName: json['continuityItemName'] as String?,
      title: json['title'] as String,
      summary: json['summary'] as String?,
      urgency: UrgencyStageX.fromString(json['urgency'] as String? ?? 'FIRST_72_HOURS'),
      priority: json['priority'] as String? ?? 'CRITICAL',
      assignedTrustedPersonId: json['assignedTrustedPersonId'] as String?,
      documentLocationHint: json['documentLocationHint'] as String?,
      digitalStorageLink: json['digitalStorageLink'] as String?,
      hasConfidentialInstructions: json['hasConfidentialInstructions'] as bool? ?? false,
      cipherInstructionsBlob: json['cipherInstructionsBlob'] as String?,
      cipherNonce: json['cipherNonce'] as String?,
      cipherAuthTag: json['cipherAuthTag'] as String?,
      isCompleted: json['isCompleted'] as bool? ?? false,
      rowVersion: json['rowVersion'] as int? ?? 1,
      createdAtUtc: DateTime.parse(json['createdAtUtc'] as String),
      updatedAtUtc: json['updatedAtUtc'] != null ? DateTime.parse(json['updatedAtUtc'] as String) : null,
      steps: stepsRaw.map((s) => ActionStepEntity.fromJson(s as Map<String, dynamic>)).toList(),
      contacts: contactsRaw.map((c) => ActionContactEntity.fromJson(c as Map<String, dynamic>)).toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'ownerId': ownerId,
      'categoryId': categoryId,
      'categoryCode': categoryCode,
      'categoryNameVi': categoryNameVi,
      'continuityItemId': continuityItemId,
      'continuityItemName': continuityItemName,
      'title': title,
      'summary': summary,
      'urgency': urgency.toShortString(),
      'priority': priority,
      'assignedTrustedPersonId': assignedTrustedPersonId,
      'documentLocationHint': documentLocationHint,
      'digitalStorageLink': digitalStorageLink,
      'hasConfidentialInstructions': hasConfidentialInstructions,
      'cipherInstructionsBlob': cipherInstructionsBlob,
      'cipherNonce': cipherNonce,
      'cipherAuthTag': cipherAuthTag,
      'isCompleted': isCompleted,
      'rowVersion': rowVersion,
      'createdAtUtc': createdAtUtc.toIso8601String(),
      'updatedAtUtc': updatedAtUtc?.toIso8601String(),
      'steps': steps.map((s) => s.toJson()).toList(),
      'contacts': contacts.map((c) => c.toJson()).toList(),
    };
  }

  @override
  List<Object?> get props => [
        id,
        ownerId,
        categoryId,
        categoryCode,
        categoryNameVi,
        continuityItemId,
        continuityItemName,
        title,
        summary,
        urgency,
        priority,
        assignedTrustedPersonId,
        documentLocationHint,
        digitalStorageLink,
        hasConfidentialInstructions,
        cipherInstructionsBlob,
        cipherNonce,
        cipherAuthTag,
        isCompleted,
        rowVersion,
        createdAtUtc,
        updatedAtUtc,
        steps,
        contacts,
      ];
}
