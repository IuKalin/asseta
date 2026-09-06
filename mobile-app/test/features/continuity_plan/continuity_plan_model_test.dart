import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/features/continuity_plan/domain/entities/continuity_plan_entity.dart';
import 'package:asseta_mobile/features/continuity_plan/data/models/continuity_plan_model.dart';

void main() {
  group('ContinuityPlanModel & JSON serialization', () {
    test('UrgencyStageX should convert string correctly', () {
      expect(UrgencyStageX.fromString('IMMEDIATE'), UrgencyStage.immediate);
      expect(UrgencyStageX.fromString('FIRST_72_HOURS'), UrgencyStage.first72Hours);
      expect(UrgencyStageX.fromString('FIRST_7_DAYS'), UrgencyStage.first7Days);
      expect(UrgencyStageX.fromString('LONGER_TERM'), UrgencyStage.longerTerm);

      expect(UrgencyStage.immediate.toShortString(), 'IMMEDIATE');
      expect(UrgencyStage.first72Hours.toShortString(), 'FIRST_72_HOURS');
    });

    test('ContinuityPlanCardItemModel parses JSON correctly', () {
      final json = {
        'id': 'card-1',
        'categoryId': 'cat-1',
        'categoryName': 'Bất Động Sản',
        'categoryIcon': 'home',
        'title': 'Sổ đỏ nhà đất',
        'summary': 'Chuyển quyền sử dụng',
        'urgency': 'IMMEDIATE',
        'priority': 'CRITICAL',
        'assignedTrustedPersonId': 'tp-1',
        'assignedTrustedPersonName': 'Nguyễn Văn B',
        'assignedTrustedPersonPhone': '0901234567',
        'documentLocationHint': 'Két sắt gia đình',
        'hasDocumentLocation': true,
        'hasStageGap': false,
        'isCompleted': false,
        'stepsCount': 3,
        'contactsCount': 1,
        'rowVersion': 2,
      };

      final model = ContinuityPlanCardItemModel.fromJson(json);

      expect(model.id, 'card-1');
      expect(model.title, 'Sổ đỏ nhà đất');
      expect(model.urgency, UrgencyStage.immediate);
      expect(model.hasDocumentLocation, true);
      expect(model.hasStageGap, false);
      expect(model.rowVersion, 2);
    });

    test('ContinuityPlanModel parses complete plan JSON with SPoF risk', () {
      final json = {
        'ownerId': 'owner-1',
        'totalCardsCount': 4,
        'completedCardsCount': 1,
        'gapsCount': 1,
        'delegateCoveragePercentage': 75.0,
        'documentReadinessPercentage': 50.0,
        'overallPlanReadinessScore': 68,
        'hasSinglePointOfFailureRisk': true,
        'singlePointOfFailureWarning': 'Người ủy thác Nguyễn Văn B phụ trách trên 70% đầu việc khẩn cấp',
        'stages': [
          {
            'stage': 'IMMEDIATE',
            'stageName': 'Ngay lập tức (24h)',
            'stageDescription': 'Đầu việc cấp bách',
            'totalCardsCount': 2,
            'completedCardsCount': 0,
            'gapCardsCount': 1,
            'cards': [
              {
                'id': 'card-1',
                'categoryId': 'cat-1',
                'categoryName': 'Bất động sản',
                'categoryIcon': 'home',
                'title': 'Bàn giao chìa khóa',
                'urgency': 'IMMEDIATE',
                'priority': 'CRITICAL',
                'assignedTrustedPersonId': 'tp-1',
                'assignedTrustedPersonName': 'Nguyễn Văn B',
                'hasDocumentLocation': false,
                'hasStageGap': true,
                'isCompleted': false,
                'stepsCount': 1,
                'contactsCount': 1,
                'rowVersion': 1,
              }
            ],
          }
        ],
      };

      final plan = ContinuityPlanModel.fromJson(json);

      expect(plan.ownerId, 'owner-1');
      expect(plan.totalCardsCount, 4);
      expect(plan.overallPlanReadinessScore, 68);
      expect(plan.hasSinglePointOfFailureRisk, true);
      expect(plan.stages.length, 1);
      expect(plan.stages.first.cards.first.hasStageGap, true);
    });
  });
}
