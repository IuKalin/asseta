export type HeartbeatStatus = 'ACTIVE' | 'WARNING' | 'PENDING_GRACE_PERIOD' | 'ACTIVATED';

export interface ActivationConfirmation {
  id: string;
  trustedPersonId: string;
  trustedPersonName?: string;
  isConfirmed: boolean;
  note?: string;
  confirmedAtUtc: string;
}

export interface ActivationRequest {
  id: string;
  ownerId: string;
  triggerSource: 'TrustedPersonRequest' | 'SystemTimeout' | string;
  initiatedByTrustedPersonId?: string;
  initiatedByTrustedPersonName?: string;
  reason?: string;
  status: 'PendingGracePeriod' | 'CancelledByOwner' | 'Activated' | 'Rejected' | string;
  gracePeriodExpiresAtUtc: string;
  remainingSeconds: number;
  confirmationsCount: number;
  minConfirmationsRequired: number;
  rowVersion: number;
  confirmations: ActivationConfirmation[];
}

export interface ActivationStatus {
  ownerId: string;
  checkInIntervalDays: number;
  gracePeriodHours: number;
  minConfirmationsRequired: number;
  lastCheckInAtUtc: string;
  nextCheckInDueUtc: string;
  isDueSoon: boolean;
  isOverdue: boolean;
  heartbeatStatus: HeartbeatStatus;
  isEmergencyActive: boolean;
  activeRequest?: ActivationRequest | null;
}

export interface ActivationConfig {
  checkInIntervalDays: number;
  gracePeriodHours: number;
  minConfirmationsRequired: number;
}
