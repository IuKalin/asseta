import { UrgencyStage, CardPriority } from './actionCard';

export interface ContinuityPlanCardItem {
  id: string;
  categoryId: string;
  categoryName: string;
  categoryIcon: string;
  title: string;
  summary?: string | null;
  urgency: UrgencyStage;
  priority: CardPriority;
  assignedTrustedPersonId?: string | null;
  assignedTrustedPersonName?: string | null;
  assignedTrustedPersonPhone?: string | null;
  documentLocationHint?: string | null;
  hasDocumentLocation: boolean;
  hasStageGap: boolean;
  isCompleted: boolean;
  stepsCount: number;
  contactsCount: number;
  rowVersion: number;
}

export interface ContinuityPlanStage {
  stage: UrgencyStage;
  stageName: string;
  stageDescription: string;
  totalCardsCount: number;
  completedCardsCount: number;
  gapCardsCount: number;
  cards: ContinuityPlanCardItem[];
}

export interface ContinuityPlan {
  ownerId: string;
  totalCardsCount: number;
  completedCardsCount: number;
  gapsCount: number;
  delegateCoveragePercentage: number;
  documentReadinessPercentage: number;
  overallPlanReadinessScore: number;
  hasSinglePointOfFailureRisk: boolean;
  singlePointOfFailureWarning?: string | null;
  stages: ContinuityPlanStage[];
}

export interface EmergencyBriefCard {
  id: string;
  title: string;
  priority: CardPriority;
  delegateName?: string | null;
  delegatePhone?: string | null;
  documentLocationHint?: string | null;
  keySteps: string[];
  keyContacts: string[];
}

export interface EmergencyBriefStage {
  stage: UrgencyStage;
  stageName: string;
  actionItems: EmergencyBriefCard[];
}

export interface EmergencyContactSummary {
  name: string;
  phone?: string | null;
  roleOrRelationship?: string | null;
  relatedCardTitle: string;
}

export interface OfflineEmergencyBrief {
  ownerId: string;
  generatedAtUtc: string;
  stages: EmergencyBriefStage[];
  primaryContacts: EmergencyContactSummary[];
}

export interface StageAuditSummary {
  stage: UrgencyStage;
  stageName: string;
  totalCards: number;
  gapCards: number;
  stageScore: number;
}

export interface PlanGapItem {
  cardId: string;
  cardTitle: string;
  stage: UrgencyStage;
  priority: CardPriority;
  gapReason: string;
  recommendedAction: string;
}

export interface PlanReadinessAudit {
  ownerId: string;
  overallScore: number;
  delegateCoveragePercentage: number;
  documentReadinessPercentage: number;
  totalGapsCount: number;
  hasSinglePointOfFailureRisk: boolean;
  singlePointOfFailureDetails?: string | null;
  stageAudits: StageAuditSummary[];
  identifiedGaps: PlanGapItem[];
  actionableRecommendations: string[];
}
