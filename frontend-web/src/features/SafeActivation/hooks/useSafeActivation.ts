import { useState, useEffect, useCallback, useRef } from 'react';
import { safeActivationService } from '../../../services/safeActivationService';
import {
  ActivationStatus,
  ActivationConfig,
} from '../../../types/safeActivation';

export function useSafeActivation(targetOwnerId?: string) {
  const [status, setStatus] = useState<ActivationStatus | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [remainingSeconds, setRemainingSeconds] = useState<number>(0);
  const timerRef = useRef<number | null>(null);

  const fetchStatus = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await safeActivationService.getStatus(targetOwnerId);
      setStatus(data);
      if (data.activeRequest && data.activeRequest.remainingSeconds > 0) {
        setRemainingSeconds(data.activeRequest.remainingSeconds);
      } else {
        setRemainingSeconds(0);
      }
    } catch (err: any) {
      setError(err?.response?.data?.error?.message || err.message || 'Không thể tải trạng thái kích hoạt an toàn.');
    } finally {
      setLoading(false);
    }
  }, [targetOwnerId]);

  useEffect(() => {
    fetchStatus();
  }, [fetchStatus]);

  // Countdown timer for pending grace period
  useEffect(() => {
    if (remainingSeconds > 0) {
      timerRef.current = window.setInterval(() => {
        setRemainingSeconds((prev) => {
          if (prev <= 1) {
            if (timerRef.current) clearInterval(timerRef.current);
            fetchStatus();
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    }

    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, [remainingSeconds, fetchStatus]);

  const vitalityCheckIn = async () => {
    try {
      const updated = await safeActivationService.vitalityCheckIn();
      setStatus(updated);
      setRemainingSeconds(0);
      return updated;
    } catch (err: any) {
      throw new Error(err?.response?.data?.error?.message || err.message || 'Lỗi khi điểm danh.');
    }
  };

  const updateConfig = async (config: ActivationConfig) => {
    try {
      const updated = await safeActivationService.updateConfig(config);
      await fetchStatus();
      return updated;
    } catch (err: any) {
      throw new Error(err?.response?.data?.error?.message || err.message || 'Lỗi khi cập nhật cấu hình.');
    }
  };

  const initiateRequest = async (ownerId: string, reason?: string) => {
    try {
      const request = await safeActivationService.initiateRequest(ownerId, reason);
      await fetchStatus();
      return request;
    } catch (err: any) {
      throw new Error(err?.response?.data?.error?.message || err.message || 'Lỗi khi gửi yêu cầu kích hoạt.');
    }
  };

  const cancelRequest = async (requestId: string) => {
    try {
      const success = await safeActivationService.cancelRequest(requestId);
      await fetchStatus();
      return success;
    } catch (err: any) {
      throw new Error(err?.response?.data?.error?.message || err.message || 'Lỗi khi hủy yêu cầu kích hoạt.');
    }
  };

  const confirmRequest = async (requestId: string, isConfirmed: boolean, note?: string) => {
    try {
      const request = await safeActivationService.confirmRequest(requestId, isConfirmed, note);
      await fetchStatus();
      return request;
    } catch (err: any) {
      throw new Error(err?.response?.data?.error?.message || err.message || 'Lỗi khi xác nhận yêu cầu.');
    }
  };

  const deactivateEmergency = async () => {
    try {
      const success = await safeActivationService.deactivateEmergency();
      await fetchStatus();
      return success;
    } catch (err: any) {
      throw new Error(err?.response?.data?.error?.message || err.message || 'Lỗi khi khôi phục trạng thái an toàn.');
    }
  };

  return {
    status,
    loading,
    error,
    remainingSeconds,
    refreshStatus: fetchStatus,
    vitalityCheckIn,
    updateConfig,
    initiateRequest,
    cancelRequest,
    confirmRequest,
    deactivateEmergency,
  };
}
