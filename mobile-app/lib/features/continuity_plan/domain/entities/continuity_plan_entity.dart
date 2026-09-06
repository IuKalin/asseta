import 'package:equatable/equatable.dart';

enum UrgencyStage { immediate, first72Hours, first7Days, longerTerm }

extension UrgencyStageX on UrgencyStage {
  String toShortString() {
    switch (this) {
      case UrgencyStage.immediate:
        return 'IMMEDIATE';
      case UrgencyStage.first72Hours:
        return 'FIRST_72_HOURS';
      case UrgencyStage.first7Days:
        return 'FIRST_7_DAYS';
      case UrgencyStage.longerTerm:
        return 'LONGER_TERM';
    }
  }

  String get displayNameVi {
    switch (this) {
      case UrgencyStage.immediate:
        return 'Ngay lập tức (24h)';
      case UrgencyStage.first72Hours:
        return '72 giờ đầu';
      case UrgencyStage.first7Days:
        return '7 ngày đầu';
      case UrgencyStage.longerTerm:
        return 'Dài hạn (30 ngày)';
    }
  }

  static UrgencyStage fromString(String val) {
    switch (val.toUpperCase()) {
      case 'FIRST_72_HOURS':
        return UrgencyStage.first72Hours;
      case 'FIRST_7_DAYS':
        return UrgencyStage.first7Days;
      case 'LONGER_TERM':
        return UrgencyStage.longerTerm;
      case 'IMMEDIATE':
      default:
        return UrgencyStage.immediate;
    }
  }
}

class ContinuityPlanCardItemEntity extends Equatable {
  final String id;
  final String categoryId;
  final String categoryName;
  final String categoryIcon;
  final String title;
  final String? summary;
  final UrgencyStage urgency;
  final String priority;
  final String? assignedTrustedPersonId;
  final String? assignedTrustedPersonName;
  final String? assignedTrustedPersonPhone;
  final String? documentLocationHint;
  final bool hasDocumentLocation;
  final bool hasStageGap;
  final bool isCompleted;
  final int stepsCount;
  final int contactsCount;
  final int rowVersion;

  const ContinuityPlanCardItemEntity({
    required this.id,
    required this.categoryId,
    required this.categoryName,
    required this.categoryIcon,
    required this.title,
    this.summary,
    required this.urgency,
    required this.priority,
    this.assignedTrustedPersonId,
    this.assignedTrustedPersonName,
    this.assignedTrustedPersonPhone,
    this.documentLocationHint,
    required this.hasDocumentLocation,
    required this.hasStageGap,
    required this.isCompleted,
    required this.stepsCount,
    required this.contactsCount,
    required this.rowVersion,
  });

  @override
  List<Object?> get props => [
        id,
        categoryId,
        categoryName,
        title,
        urgency,
        priority,
        assignedTrustedPersonId,
        hasStageGap,
        isCompleted,
        rowVersion,
      ];
}

class ContinuityPlanStageEntity extends Equatable {
  final UrgencyStage stage;
  final String stageName;
  final String stageDescription;
  final int totalCardsCount;
  final int completedCardsCount;
  final int gapCardsCount;
  final List<ContinuityPlanCardItemEntity> cards;

  const ContinuityPlanStageEntity({
    required this.stage,
    required this.stageName,
    required this.stageDescription,
    required this.totalCardsCount,
    required this.completedCardsCount,
    required this.gapCardsCount,
    required this.cards,
  });

  @override
  List<Object?> get props => [
        stage,
        stageName,
        totalCardsCount,
        completedCardsCount,
        gapCardsCount,
        cards,
      ];
}

class ContinuityPlanEntity extends Equatable {
  final String ownerId;
  final int totalCardsCount;
  final int completedCardsCount;
  final int gapsCount;
  final double delegateCoveragePercentage;
  final double documentReadinessPercentage;
  final int overallPlanReadinessScore;
  final bool hasSinglePointOfFailureRisk;
  final String? singlePointOfFailureWarning;
  final List<ContinuityPlanStageEntity> stages;

  const ContinuityPlanEntity({
    required this.ownerId,
    required this.totalCardsCount,
    required this.completedCardsCount,
    required this.gapsCount,
    required this.delegateCoveragePercentage,
    required this.documentReadinessPercentage,
    required this.overallPlanReadinessScore,
    required this.hasSinglePointOfFailureRisk,
    this.singlePointOfFailureWarning,
    required this.stages,
  });

  @override
  List<Object?> get props => [
        ownerId,
        totalCardsCount,
        completedCardsCount,
        gapsCount,
        overallPlanReadinessScore,
        hasSinglePointOfFailureRisk,
        stages,
      ];
}
