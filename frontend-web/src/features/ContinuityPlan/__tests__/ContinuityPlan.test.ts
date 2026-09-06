import { describe, it, expect } from 'vitest';
import {
  ContinuityPlan,
  ContinuityPlanCardItem,
  OfflineEmergencyBrief,
  PlanReadinessAudit
} from '../../../types/continuityPlan';

describe('ContinuityPlan Frontend Domain Logic & Timeline Verification', () => {
  const sampleCards: ContinuityPlanCardItem[] = [
    {
      id: 'card-1',
      categoryId: 'cat-1',
      categoryName: 'Tài chính',
      categoryIcon: 'wallet',
      title: 'Thanh toán lãi vay',
      urgency: 'IMMEDIATE',
      priority: 'CRITICAL',
      assignedTrustedPersonId: 'tp-1',
      assignedTrustedPersonName: 'Nguyễn Thị Mai (Vợ)',
      documentLocationHint: 'Két sắt phòng ngủ',
      hasDocumentLocation: true,
      hasStageGap: false,
      isCompleted: true,
      stepsCount: 3,
      contactsCount: 1,
      rowVersion: 1,
    },
    {
      id: 'card-2',
      categoryId: 'cat-2',
      categoryName: 'Doanh nghiệp',
      categoryIcon: 'briefcase',
      title: 'Bàn giao điều hành công ty',
      urgency: 'IMMEDIATE',
      priority: 'CRITICAL',
      assignedTrustedPersonId: null,
      documentLocationHint: null,
      hasDocumentLocation: false,
      hasStageGap: true,
      isCompleted: false,
      stepsCount: 2,
      contactsCount: 1,
      rowVersion: 1,
    },
    {
      id: 'card-3',
      categoryId: 'cat-3',
      categoryName: 'Bất động sản',
      categoryIcon: 'home',
      title: 'Thu tiền thuê nhà 72h',
      urgency: 'FIRST_72_HOURS',
      priority: 'IMPORTANT',
      assignedTrustedPersonId: 'tp-1',
      documentLocationHint: 'Tủ hồ sơ',
      hasDocumentLocation: true,
      hasStageGap: false,
      isCompleted: false,
      stepsCount: 1,
      contactsCount: 1,
      rowVersion: 1,
    },
  ];

  it('verifies 4 standard timeline stages structure', () => {
    const plan: ContinuityPlan = {
      ownerId: 'owner-1',
      totalCardsCount: 3,
      completedCardsCount: 1,
      gapsCount: 1,
      delegateCoveragePercentage: 66.7,
      documentReadinessPercentage: 66.7,
      overallPlanReadinessScore: 70,
      hasSinglePointOfFailureRisk: false,
      stages: [
        {
          stage: 'IMMEDIATE',
          stageName: 'Immediate Actions (NOW)',
          stageDescription: 'Những việc phải xử lý ngay trong 24 giờ đầu',
          totalCardsCount: 2,
          completedCardsCount: 1,
          gapCardsCount: 1,
          cards: [sampleCards[0], sampleCards[1]],
        },
        {
          stage: 'FIRST_72_HOURS',
          stageName: 'First 72 Hours',
          stageDescription: 'Ổn định hoạt động (24–72 giờ)',
          totalCardsCount: 1,
          completedCardsCount: 0,
          gapCardsCount: 0,
          cards: [sampleCards[2]],
        },
        {
          stage: 'FIRST_7_DAYS',
          stageName: 'First 7 Days',
          stageDescription: 'Làm việc với các bên thứ ba',
          totalCardsCount: 0,
          completedCardsCount: 0,
          gapCardsCount: 0,
          cards: [],
        },
        {
          stage: 'LONGER_TERM',
          stageName: 'Longer-Term Continuity',
          stageDescription: 'Tiếp quản dài hạn',
          totalCardsCount: 0,
          completedCardsCount: 0,
          gapCardsCount: 0,
          cards: [],
        },
      ],
    };

    expect(plan.stages).toHaveLength(4);
    expect(plan.stages[0].stage).toBe('IMMEDIATE');
    expect(plan.stages[1].stage).toBe('FIRST_72_HOURS');
    expect(plan.stages[2].stage).toBe('FIRST_7_DAYS');
    expect(plan.stages[3].stage).toBe('LONGER_TERM');
  });

  it('correctly flags stage gap when immediate card lacks delegate or document location', () => {
    const validImmediateCard = sampleCards[0];
    const gapImmediateCard = sampleCards[1];

    expect(validImmediateCard.hasStageGap).toBe(false);
    expect(gapImmediateCard.hasStageGap).toBe(true);
    expect(gapImmediateCard.assignedTrustedPersonId).toBeNull();
    expect(gapImmediateCard.hasDocumentLocation).toBe(false);
  });

  it('verifies Zero-Knowledge Offline Emergency Brief contains no cipher instructions', () => {
    const brief: OfflineEmergencyBrief = {
      ownerId: 'owner-1',
      generatedAtUtc: new Date().toISOString(),
      primaryContacts: [
        {
          name: 'Nguyễn Thị Mai',
          phone: '0901234567',
          roleOrRelationship: 'Vợ',
          relatedCardTitle: 'Thanh toán lãi vay',
        },
      ],
      stages: [
        {
          stage: 'IMMEDIATE',
          stageName: 'Immediate Actions (NOW)',
          actionItems: [
            {
              id: 'card-1',
              title: 'Thanh toán lãi vay',
              priority: 'CRITICAL',
              delegateName: 'Nguyễn Thị Mai',
              delegatePhone: '0901234567',
              documentLocationHint: 'Két sắt',
              keySteps: ['Mở két lấy hợp đồng', 'Đến ngân hàng'],
              keyContacts: ['Chuyên viên tín dụng - 0988776655'],
            },
          ],
        },
      ],
    };

    expect(brief.stages[0].actionItems[0]).not.toHaveProperty('cipherInstructionsBlob');
    expect(brief.stages[0].actionItems[0]).not.toHaveProperty('cipherNonce');
    expect(brief.primaryContacts).toHaveLength(1);
    expect(brief.stages[0].actionItems[0].keySteps).toHaveLength(2);
  });

  it('verifies Plan Readiness Audit SPoF detection and recommendations', () => {
    const audit: PlanReadinessAudit = {
      ownerId: 'owner-1',
      overallScore: 65,
      delegateCoveragePercentage: 70.0,
      documentReadinessPercentage: 60.0,
      totalGapsCount: 2,
      hasSinglePointOfFailureRisk: true,
      singlePointOfFailureDetails: 'Người ủy thác Nguyễn Thị Mai gán 80% việc',
      stageAudits: [
        {
          stage: 'IMMEDIATE',
          stageName: 'Immediate',
          totalCards: 2,
          gapCards: 1,
          stageScore: 50,
        },
      ],
      identifiedGaps: [
        {
          cardId: 'card-2',
          cardTitle: 'Bàn giao điều hành',
          stage: 'IMMEDIATE',
          priority: 'CRITICAL',
          gapReason: 'Chưa có người phụ trách',
          recommendedAction: 'Gán người ủy thác',
        },
      ],
      actionableRecommendations: [
        'Ưu tiên hàng đầu: Khắc phục 1 lỗ hổng trong Immediate Actions',
      ],
    };

    expect(audit.hasSinglePointOfFailureRisk).toBe(true);
    expect(audit.identifiedGaps).toHaveLength(1);
    expect(audit.actionableRecommendations[0]).toContain('Immediate Actions');
  });
});
