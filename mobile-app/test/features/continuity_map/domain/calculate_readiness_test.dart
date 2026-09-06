import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/features/continuity_map/domain/entities/continuity_item_entity.dart';
import 'package:asseta_mobile/features/continuity_map/domain/entities/continuity_map_entity.dart';
import 'package:asseta_mobile/features/continuity_map/domain/usecases/calculate_readiness_locally.dart';

void main() {
  group('CalculateReadinessLocally (Offline-First Calculations)', () {
    test('calculateCategoryScore returns 0 when items are empty', () {
      final score = CalculateReadinessLocally.calculateCategoryScore([]);
      expect(score, equals(0));
    });

    test('calculateCategoryScore returns 100 when item has all fields and action completed', () {
      final item = ContinuityItemEntity(
        id: 'item-1',
        ownerId: 'owner-1',
        categoryId: 'cat-1',
        categoryCode: 'FINANCIAL',
        name: 'Sổ tiết kiệm Agribank',
        priority: ItemPriority.critical,
        documentLocationHint: 'Két sắt',
        assignedTrustedPersonId: 'trusted-1',
        hasConfidentialNotes: false,
        isCompleted: true,
        rowVersion: 1,
        sortOrder: 1,
        createdAtUtc: DateTime.now(),
      );

      final score = CalculateReadinessLocally.calculateCategoryScore([item]);
      expect(score, equals(100));
    });

    test('calculateCategoryScore returns 80 when item has person and doc hint but action not completed', () {
      final item = ContinuityItemEntity(
        id: 'item-1',
        ownerId: 'owner-1',
        categoryId: 'cat-1',
        categoryCode: 'FINANCIAL',
        name: 'Sổ tiết kiệm Agribank',
        priority: ItemPriority.critical,
        documentLocationHint: 'Két sắt',
        assignedTrustedPersonId: 'trusted-1',
        hasConfidentialNotes: false,
        isCompleted: false, // 40% + 40% = 80%
        rowVersion: 1,
        sortOrder: 1,
        createdAtUtc: DateTime.now(),
      );

      final score = CalculateReadinessLocally.calculateCategoryScore([item]);
      expect(score, equals(80));
    });

    test('detectGaps detects item with missing person or missing hint on critical/important priority', () {
      final itemWithGap = ContinuityItemEntity(
        id: 'item-gap',
        ownerId: 'owner-1',
        categoryId: 'cat-1',
        categoryCode: 'FINANCIAL',
        name: 'Khoản vay Vietcombank',
        priority: ItemPriority.critical,
        documentLocationHint: null, // Gap!
        assignedTrustedPersonId: 'trusted-1',
        hasConfidentialNotes: false,
        isCompleted: false,
        rowVersion: 1,
        sortOrder: 1,
        createdAtUtc: DateTime.now(),
      );

      final itemWithoutGap = ContinuityItemEntity(
        id: 'item-ok',
        ownerId: 'owner-1',
        categoryId: 'cat-1',
        categoryCode: 'FINANCIAL',
        name: 'Thẻ tín dụng',
        priority: ItemPriority.low, // Low priority doesn't trigger gap
        documentLocationHint: null,
        assignedTrustedPersonId: null,
        hasConfidentialNotes: false,
        isCompleted: false,
        rowVersion: 1,
        sortOrder: 2,
        createdAtUtc: DateTime.now(),
      );

      final gaps = CalculateReadinessLocally.detectGaps([itemWithGap, itemWithoutGap]);
      expect(gaps.length, equals(1));
      expect(gaps.first.id, equals('item-gap'));
    });

    test('calculateOverallScore averages across all categories', () {
      final cat1 = ContinuityCategoryEntity(
        categoryId: '1',
        code: 'FINANCIAL',
        name: 'Tài chính',
        icon: 'wallet',
        readinessScore: 80,
        items: const [],
      );
      final cat2 = ContinuityCategoryEntity(
        categoryId: '2',
        code: 'PROPERTY',
        name: 'Bất động sản',
        icon: 'home',
        readinessScore: 60,
        items: const [],
      );

      final overall = CalculateReadinessLocally.calculateOverallScore([cat1, cat2]);
      expect(overall, equals(70)); // (80 + 60) / 2 = 70
    });
  });
}
