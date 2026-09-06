import '../../domain/entities/continuity_plan_entity.dart';

class ContinuityPlanCardItemModel extends ContinuityPlanCardItemEntity {
  const ContinuityPlanCardItemModel({
    required super.id,
    required super.categoryId,
    required super.categoryName,
    required super.categoryIcon,
    required super.title,
    super.summary,
    required super.urgency,
    required super.priority,
    super.assignedTrustedPersonId,
    super.assignedTrustedPersonName,
    super.assignedTrustedPersonPhone,
    super.documentLocationHint,
    required super.hasDocumentLocation,
    required super.hasStageGap,
    required super.isCompleted,
    required super.stepsCount,
    required super.contactsCount,
    required super.rowVersion,
  });

  factory ContinuityPlanCardItemModel.fromJson(Map<String, dynamic> json) {
    return ContinuityPlanCardItemModel(
      id: json['id'] as String? ?? '',
      categoryId: json['categoryId'] as String? ?? '',
      categoryName: json['categoryName'] as String? ?? 'Khác',
      categoryIcon: json['categoryIcon'] as String? ?? 'folder',
      title: json['title'] as String? ?? '',
      summary: json['summary'] as String?,
      urgency: UrgencyStageX.fromString(json['urgency']?.toString() ?? 'IMMEDIATE'),
      priority: json['priority']?.toString() ?? 'CRITICAL',
      assignedTrustedPersonId: json['assignedTrustedPersonId'] as String?,
      assignedTrustedPersonName: json['assignedTrustedPersonName'] as String?,
      assignedTrustedPersonPhone: json['assignedTrustedPersonPhone'] as String?,
      documentLocationHint: json['documentLocationHint'] as String?,
      hasDocumentLocation: json['hasDocumentLocation'] as bool? ?? false,
      hasStageGap: json['hasStageGap'] as bool? ?? false,
      isCompleted: json['isCompleted'] as bool? ?? false,
      stepsCount: (json['stepsCount'] as num?)?.toInt() ?? 0,
      contactsCount: (json['contactsCount'] as num?)?.toInt() ?? 0,
      rowVersion: (json['rowVersion'] as num?)?.toInt() ?? 1,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'categoryId': categoryId,
      'categoryName': categoryName,
      'categoryIcon': categoryIcon,
      'title': title,
      'summary': summary,
      'urgency': urgency.toShortString(),
      'priority': priority,
      'assignedTrustedPersonId': assignedTrustedPersonId,
      'assignedTrustedPersonName': assignedTrustedPersonName,
      'assignedTrustedPersonPhone': assignedTrustedPersonPhone,
      'documentLocationHint': documentLocationHint,
      'hasDocumentLocation': hasDocumentLocation,
      'hasStageGap': hasStageGap,
      'isCompleted': isCompleted,
      'stepsCount': stepsCount,
      'contactsCount': contactsCount,
      'rowVersion': rowVersion,
    };
  }
}

class ContinuityPlanStageModel extends ContinuityPlanStageEntity {
  const ContinuityPlanStageModel({
    required super.stage,
    required super.stageName,
    required super.stageDescription,
    required super.totalCardsCount,
    required super.completedCardsCount,
    required super.gapCardsCount,
    required super.cards,
  });

  factory ContinuityPlanStageModel.fromJson(Map<String, dynamic> json) {
    final rawCards = json['cards'] as List<dynamic>? ?? [];
    return ContinuityPlanStageModel(
      stage: UrgencyStageX.fromString(json['stage']?.toString() ?? 'IMMEDIATE'),
      stageName: json['stageName'] as String? ?? '',
      stageDescription: json['stageDescription'] as String? ?? '',
      totalCardsCount: (json['totalCardsCount'] as num?)?.toInt() ?? 0,
      completedCardsCount: (json['completedCardsCount'] as num?)?.toInt() ?? 0,
      gapCardsCount: (json['gapCardsCount'] as num?)?.toInt() ?? 0,
      cards: rawCards
          .map((c) => ContinuityPlanCardItemModel.fromJson(c as Map<String, dynamic>))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'stage': stage.toShortString(),
      'stageName': stageName,
      'stageDescription': stageDescription,
      'totalCardsCount': totalCardsCount,
      'completedCardsCount': completedCardsCount,
      'gapCardsCount': gapCardsCount,
      'cards': cards.map((c) => (c as ContinuityPlanCardItemModel).toJson()).toList(),
    };
  }
}

class ContinuityPlanModel extends ContinuityPlanEntity {
  const ContinuityPlanModel({
    required super.ownerId,
    required super.totalCardsCount,
    required super.completedCardsCount,
    required super.gapsCount,
    required super.delegateCoveragePercentage,
    required super.documentReadinessPercentage,
    required super.overallPlanReadinessScore,
    required super.hasSinglePointOfFailureRisk,
    super.singlePointOfFailureWarning,
    required super.stages,
  });

  factory ContinuityPlanModel.fromJson(Map<String, dynamic> json) {
    final rawStages = json['stages'] as List<dynamic>? ?? [];
    return ContinuityPlanModel(
      ownerId: json['ownerId'] as String? ?? '',
      totalCardsCount: (json['totalCardsCount'] as num?)?.toInt() ?? 0,
      completedCardsCount: (json['completedCardsCount'] as num?)?.toInt() ?? 0,
      gapsCount: (json['gapsCount'] as num?)?.toInt() ?? 0,
      delegateCoveragePercentage: (json['delegateCoveragePercentage'] as num?)?.toDouble() ?? 0.0,
      documentReadinessPercentage: (json['documentReadinessPercentage'] as num?)?.toDouble() ?? 0.0,
      overallPlanReadinessScore: (json['overallPlanReadinessScore'] as num?)?.toInt() ?? 0,
      hasSinglePointOfFailureRisk: json['hasSinglePointOfFailureRisk'] as bool? ?? false,
      singlePointOfFailureWarning: json['singlePointOfFailureWarning'] as String?,
      stages: rawStages
          .map((s) => ContinuityPlanStageModel.fromJson(s as Map<String, dynamic>))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'ownerId': ownerId,
      'totalCardsCount': totalCardsCount,
      'completedCardsCount': completedCardsCount,
      'gapsCount': gapsCount,
      'delegateCoveragePercentage': delegateCoveragePercentage,
      'documentReadinessPercentage': documentReadinessPercentage,
      'overallPlanReadinessScore': overallPlanReadinessScore,
      'hasSinglePointOfFailureRisk': hasSinglePointOfFailureRisk,
      'singlePointOfFailureWarning': singlePointOfFailureWarning,
      'stages': stages.map((s) => (s as ContinuityPlanStageModel).toJson()).toList(),
    };
  }
}
