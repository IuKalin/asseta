import 'package:equatable/equatable.dart';
import 'continuity_item_entity.dart';

class ContinuityCategoryEntity extends Equatable {
  final String categoryId;
  final String code;
  final String name;
  final String icon;
  final int readinessScore;
  final List<ContinuityItemEntity> items;

  const ContinuityCategoryEntity({
    required this.categoryId,
    required this.code,
    required this.name,
    required this.icon,
    required this.readinessScore,
    required this.items,
  });

  @override
  List<Object?> get props => [categoryId, code, name, icon, readinessScore, items];
}

class ContinuityGapEntity extends Equatable {
  final String itemId;
  final String itemName;
  final String categoryCode;
  final String priority;
  final List<String> missingFields;

  const ContinuityGapEntity({
    required this.itemId,
    required this.itemName,
    required this.categoryCode,
    required this.priority,
    required this.missingFields,
  });

  @override
  List<Object?> get props => [itemId, itemName, categoryCode, priority, missingFields];
}

class ContinuityMapEntity extends Equatable {
  final int overallReadinessScore;
  final int totalItems;
  final int totalGaps;
  final List<ContinuityCategoryEntity> categories;
  final List<ContinuityGapEntity> gaps;

  const ContinuityMapEntity({
    required this.overallReadinessScore,
    required this.totalItems,
    required this.totalGaps,
    required this.categories,
    required this.gaps,
  });

  @override
  List<Object?> get props => [overallReadinessScore, totalItems, totalGaps, categories, gaps];
}
