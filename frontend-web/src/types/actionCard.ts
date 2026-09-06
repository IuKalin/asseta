export type UrgencyStage = 'IMMEDIATE' | 'FIRST_72_HOURS' | 'FIRST_7_DAYS' | 'LONGER_TERM';
export type CardPriority = 'CRITICAL' | 'IMPORTANT' | 'LOW';

export interface ActionCardStep {
  id: string;
  actionCardId: string;
  stepOrder: number;
  instruction: string;
  estimatedDuration?: string | null;
  isCompleted: boolean;
}

export interface ActionCardContact {
  id: string;
  actionCardId: string;
  contactName: string;
  relationshipOrRole: string;
  phoneNumber?: string | null;
  email?: string | null;
  contactNotes?: string | null;
}

export interface ActionCard {
  id: string;
  ownerId: string;
  categoryId: string;
  categoryCode: string;
  categoryNameVi: string;
  continuityItemId?: string | null;
  continuityItemName?: string | null;
  title: string;
  summary?: string | null;
  urgency: UrgencyStage;
  priority: CardPriority;
  assignedTrustedPersonId?: string | null;
  documentLocationHint?: string | null;
  digitalStorageLink?: string | null;
  hasConfidentialInstructions: boolean;
  cipherInstructionsBlob?: string | null;
  cipherNonce?: string | null;
  cipherAuthTag?: string | null;
  isCompleted: boolean;
  rowVersion: number;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
  steps: ActionCardStep[];
  contacts: ActionCardContact[];
}

export interface TemplateStep {
  stepOrder: number;
  instruction: string;
  estimatedDuration?: string | null;
}

export interface ActionCardTemplate {
  id: string;
  templateCode: string;
  categoryCode: string;
  titleVi: string;
  titleEn: string;
  defaultUrgency: UrgencyStage;
  defaultPriority: CardPriority;
  suggestedSteps: TemplateStep[];
  suggestedRoles: string[];
}

export interface ActionCardFilters {
  urgency?: UrgencyStage;
  categoryId?: string;
  search?: string;
}

export interface CreateActionCardInput {
  categoryId: string;
  title: string;
  urgency: UrgencyStage;
  priority: CardPriority;
  summary?: string | null;
  assignedTrustedPersonId?: string | null;
  documentLocationHint?: string | null;
  digitalStorageLink?: string | null;
  cipherInstructionsBlob?: string | null;
  cipherNonce?: string | null;
  cipherAuthTag?: string | null;
}

export interface CreateFromItemInput {
  continuityItemId: string;
  templateCode?: string | null;
  urgency?: UrgencyStage | null;
  priority?: CardPriority | null;
  summary?: string | null;
}

export interface UpdateActionCardInput {
  title: string;
  urgency: UrgencyStage;
  priority: CardPriority;
  rowVersion: number;
  summary?: string | null;
  assignedTrustedPersonId?: string | null;
  documentLocationHint?: string | null;
  digitalStorageLink?: string | null;
  cipherInstructionsBlob?: string | null;
  cipherNonce?: string | null;
  cipherAuthTag?: string | null;
}

export interface AddStepInput {
  instruction: string;
  estimatedDuration?: string | null;
}

export interface UpdateStepInput {
  instruction: string;
  estimatedDuration?: string | null;
  isCompleted?: boolean | null;
}

export interface AddContactInput {
  contactName: string;
  relationshipOrRole: string;
  phoneNumber?: string | null;
  email?: string | null;
  contactNotes?: string | null;
}
