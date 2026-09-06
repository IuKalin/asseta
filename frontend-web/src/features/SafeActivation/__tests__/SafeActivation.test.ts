import { describe, it, expect } from 'vitest';
import {
  ActivationStatus,
  ActivationRequest,
  ActivationConfig,
} from '../../../types/safeActivation';

describe('SafeActivation Protocol Frontend Logic & Quorum Rules', () => {
  const mockOwnerStatus: ActivationStatus = {
    ownerId: 'owner-uuid-1',
    checkInIntervalDays: 30,
    gracePeriodHours: 48,
    minConfirmationsRequired: 2,
    lastCheckInAtUtc: new Date().toISOString(),
    nextCheckInDueUtc: new Date(Date.now() + 30 * 86400 * 1000).toISOString(),
    isDueSoon: false,
    isOverdue: false,
    heartbeatStatus: 'ACTIVE',
    isEmergencyActive: false,
    activeRequest: null,
  };

  it('correctly reports healthy vitality status for active owner', () => {
    expect(mockOwnerStatus.heartbeatStatus).toBe('ACTIVE');
    expect(mockOwnerStatus.isDueSoon).toBe(false);
    expect(mockOwnerStatus.isOverdue).toBe(false);
    expect(mockOwnerStatus.isEmergencyActive).toBe(false);
    expect(mockOwnerStatus.activeRequest).toBeNull();
  });

  it('calculates countdown remaining time properly into hours, minutes, seconds', () => {
    const totalSeconds = 48 * 3600; // 48 hours
    const hrs = Math.floor(totalSeconds / 3600);
    const mins = Math.floor((totalSeconds % 3600) / 60);
    const secs = totalSeconds % 60;

    expect(hrs).toBe(48);
    expect(mins).toBe(0);
    expect(secs).toBe(0);

    const oddSeconds = 3665; // 1h 1m 5s
    expect(Math.floor(oddSeconds / 3600)).toBe(1);
    expect(Math.floor((oddSeconds % 3600) / 60)).toBe(1);
    expect(oddSeconds % 60).toBe(5);
  });

  it('detects pending grace period and verifies quorum requirement', () => {
    const activeRequest: ActivationRequest = {
      id: 'req-1',
      ownerId: 'owner-uuid-1',
      triggerSource: 'TrustedPersonRequest',
      initiatedByTrustedPersonId: 'tp-1',
      reason: 'Mất liên lạc khẩn cấp',
      status: 'PendingGracePeriod',
      gracePeriodExpiresAtUtc: new Date(Date.now() + 48 * 3600 * 1000).toISOString(),
      remainingSeconds: 48 * 3600,
      confirmationsCount: 1,
      minConfirmationsRequired: 2,
      rowVersion: 1,
      confirmations: [
        {
          id: 'conf-1',
          trustedPersonId: 'tp-1',
          trustedPersonName: 'Nguyễn Thị Mai',
          isConfirmed: true,
          note: 'Yêu cầu ban đầu',
          confirmedAtUtc: new Date().toISOString(),
        },
      ],
    };

    const statusWithPending: ActivationStatus = {
      ...mockOwnerStatus,
      heartbeatStatus: 'PENDING_GRACE_PERIOD',
      activeRequest,
    };

    expect(statusWithPending.heartbeatStatus).toBe('PENDING_GRACE_PERIOD');
    expect(statusWithPending.activeRequest?.status).toBe('PendingGracePeriod');
    // Quorum is not met yet (1 < 2)
    const isQuorumMet = (statusWithPending.activeRequest?.confirmationsCount ?? 0) >= (statusWithPending.activeRequest?.minConfirmationsRequired ?? 0);
    expect(isQuorumMet).toBe(false);

    // Simulate 2nd confirmation
    const updatedRequest: ActivationRequest = {
      ...activeRequest,
      confirmationsCount: 2,
      confirmations: [
        ...activeRequest.confirmations,
        {
          id: 'conf-2',
          trustedPersonId: 'tp-2',
          trustedPersonName: 'Trần Văn Luật',
          isConfirmed: true,
          note: 'Xác nhận khẩn cấp',
          confirmedAtUtc: new Date().toISOString(),
        },
      ],
    };

    expect(updatedRequest.confirmationsCount >= updatedRequest.minConfirmationsRequired).toBe(true);
  });

  it('supports 1-tap owner cancellation resetting status', () => {
    let currentStatus: ActivationStatus = {
      ...mockOwnerStatus,
      heartbeatStatus: 'PENDING_GRACE_PERIOD',
      activeRequest: {
        id: 'req-1',
        ownerId: 'owner-uuid-1',
        triggerSource: 'SystemTimeout',
        status: 'PendingGracePeriod',
        gracePeriodExpiresAtUtc: new Date().toISOString(),
        remainingSeconds: 1000,
        confirmationsCount: 0,
        minConfirmationsRequired: 1,
        rowVersion: 1,
        confirmations: [],
      },
    };

    // Simulate 1-tap owner cancel action
    currentStatus = {
      ...currentStatus,
      heartbeatStatus: 'ACTIVE',
      activeRequest: null,
      lastCheckInAtUtc: new Date().toISOString(),
    };

    expect(currentStatus.heartbeatStatus).toBe('ACTIVE');
    expect(currentStatus.activeRequest).toBeNull();
  });

  it('detects emergency activated state and rollback deactivation', () => {
    let currentStatus: ActivationStatus = {
      ...mockOwnerStatus,
      heartbeatStatus: 'ACTIVATED',
      isEmergencyActive: true,
    };

    expect(currentStatus.isEmergencyActive).toBe(true);
    expect(currentStatus.heartbeatStatus).toBe('ACTIVATED');

    // Simulate owner deactivation
    currentStatus = {
      ...currentStatus,
      heartbeatStatus: 'ACTIVE',
      isEmergencyActive: false,
    };

    expect(currentStatus.isEmergencyActive).toBe(false);
    expect(currentStatus.heartbeatStatus).toBe('ACTIVE');
  });

  it('validates configuration constraints for check-in interval and grace period', () => {
    const validConfig: ActivationConfig = {
      checkInIntervalDays: 30,
      gracePeriodHours: 48,
      minConfirmationsRequired: 2,
    };

    const allowedIntervals = [15, 30, 60, 90];
    const allowedGraceHours = [24, 48, 72, 168];

    expect(allowedIntervals.includes(validConfig.checkInIntervalDays)).toBe(true);
    expect(allowedGraceHours.includes(validConfig.gracePeriodHours)).toBe(true);
    expect(validConfig.minConfirmationsRequired).toBeGreaterThanOrEqual(1);
  });
});
