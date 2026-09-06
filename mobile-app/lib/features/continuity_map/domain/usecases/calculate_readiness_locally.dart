import '../entities/continuity_item_entity.dart';
import '../entities/continuity_map_entity.dart';

class CalculateReadinessLocally {
  static double _getItemWeight(ItemPriority priority) {
    switch (priority) {
      case ItemPriority.critical:
        return 0.50;
      case ItemPriority.important:
        return 0.35;
      case ItemPriority.low:
        return 0.15;
    }
  }

  static double _calculateItemScore(ContinuityItemEntity item) {
    double score = 0.0;
    if (item.assignedTrustedPersonId != null && item.assignedTrustedPersonId!.isNotEmpty) {
      score += 0.40;
    }
    if (item.documentLocationHint != null && item.documentLocationHint!.trim().isNotEmpty) {
      score += 0.40;
    }
    if (item.isCompleted) {
      score += 0.20;
    }
    return score;
  }

  static int calculateCategoryScore(List<ContinuityItemEntity> items) {
    if (items.isEmpty) return 0;

    double weightedEarned = 0.0;
    double weightedTotal = 0.0;

    for (final item in items) {
      final weight = _getItemWeight(item.priority);
      final itemScore = _calculateItemScore(item);
      weightedEarned += weight * itemScore;
      weightedTotal += weight;
    }

    if (weightedTotal == 0) return 0;
    final score = (weightedEarned / weightedTotal) * 100;
    return score.round().clamp(0, 100);
  }

  static int calculateOverallScore(List<ContinuityCategoryEntity> categories) {
    if (categories.isEmpty) return 0;
    final total = categories.fold<int>(0, (sum, cat) => sum + cat.readinessScore);
    return (total / categories.length).round().clamp(0, 100);
  }

  static List<ContinuityItemEntity> detectGaps(List<ContinuityItemEntity> items) {
    return items.where((i) => i.hasContinuityGap).toList();
  }
}
