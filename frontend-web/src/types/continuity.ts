export type PriorityLevel = 'CRITICAL' | 'IMPORTANT' | 'LOW';

export interface ContinuityItem {
  id: string;
  ownerId: string;
  categoryId: string;
  categoryCode: string;
  name: string;
  priority: PriorityLevel;
  documentLocationHint?: string | null;
  assignedTrustedPersonId?: string | null;
  actionCardId?: string | null;
  hasConfidentialNotes: boolean;
  cipherNotesBlob?: string | null;
  cipherNonce?: string | null;
  cipherAuthTag?: string | null;
  isCompleted: boolean;
  hasContinuityGap: boolean;
  rowVersion: number;
  sortOrder: number;
  createdAtUtc: string;
}

export interface ContinuityCategory {
  categoryId: string;
  code: string;
  name: string;
  icon: string;
  readinessScore: number;
  items: ContinuityItem[];
}

export interface ContinuityGap {
  itemId: string;
  itemName: string;
  categoryCode: string;
  priority: string;
  missingFields: string[];
}

export interface ContinuityMapData {
  overallReadinessScore: number;
  totalItems: number;
  totalGaps: number;
  categories: ContinuityCategory[];
  gaps: ContinuityGap[];
}

export interface AssessmentAnswer {
  questionId: string;
  categoryCode: string;
  itemName: string;
  hasItem: boolean;
  priority?: PriorityLevel;
  documentLocationHint?: string;
}

export interface AssessmentResult {
  historyId: string;
  itemsGeneratedCount: number;
  initialReadinessScore: number;
  continuityMap: ContinuityMapData;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  error?: {
    code: string;
    message: string;
    details?: unknown;
  } | null;
  meta: {
    timestamp: string;
    correlationId: string;
  };
}

export interface CreateContinuityItemPayload {
  categoryId: string;
  name: string;
  priority: PriorityLevel;
  documentLocationHint?: string;
  assignedTrustedPersonId?: string;
  cipherNotesBlob?: string;
  cipherNonce?: string;
  cipherAuthTag?: string;
}

export interface UpdateContinuityItemPayload {
  name: string;
  priority: PriorityLevel;
  documentLocationHint?: string;
  assignedTrustedPersonId?: string;
  cipherNotesBlob?: string;
  cipherNonce?: string;
  cipherAuthTag?: string;
  rowVersion: number;
}
