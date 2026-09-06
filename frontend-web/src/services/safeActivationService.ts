import { apiClient } from './apiClient';
import { ApiResponse } from '../types';
import {
  ActivationStatus,
  ActivationConfig,
  ActivationRequest,
} from '../types/safeActivation';

const getHeaders = (idempotency: boolean = false) => {
  const headers: Record<string, string> = {
    'X-Correlation-Id': typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : 'mock-uuid',
  };
  if (idempotency && typeof crypto !== 'undefined' && crypto.randomUUID) {
    headers['Idempotency-Key'] = crypto.randomUUID();
  }
  return headers;
};

export const safeActivationService = {
  async getStatus(targetOwnerId?: string): Promise<ActivationStatus> {
    const params = targetOwnerId ? { targetOwnerId } : {};
    const res = await apiClient.get<ApiResponse<ActivationStatus>>('/v1/safe-activation/status', {
      params,
      headers: getHeaders(),
    });
    return res.data.data;
  },

  async vitalityCheckIn(): Promise<ActivationStatus> {
    const res = await apiClient.post<ApiResponse<ActivationStatus>>('/v1/safe-activation/check-in', {}, {
      headers: getHeaders(true),
    });
    return res.data.data;
  },

  async updateConfig(config: ActivationConfig): Promise<ActivationConfig> {
    const res = await apiClient.put<ApiResponse<ActivationConfig>>('/v1/safe-activation/config', config, {
      headers: getHeaders(true),
    });
    return res.data.data;
  },

  async initiateRequest(targetOwnerId: string, reason?: string): Promise<ActivationRequest> {
    const res = await apiClient.post<ApiResponse<ActivationRequest>>('/v1/safe-activation/requests', {
      targetOwnerId,
      reason,
    }, {
      headers: getHeaders(true),
    });
    return res.data.data;
  },

  async cancelRequest(requestId: string): Promise<boolean> {
    const res = await apiClient.post<ApiResponse<boolean>>(`/v1/safe-activation/requests/${requestId}/cancel`, {}, {
      headers: getHeaders(true),
    });
    return res.data.data;
  },

  async confirmRequest(requestId: string, isConfirmed: boolean, note?: string): Promise<ActivationRequest> {
    const res = await apiClient.post<ApiResponse<ActivationRequest>>(`/v1/safe-activation/requests/${requestId}/confirm`, {
      isConfirmed,
      note,
    }, {
      headers: getHeaders(true),
    });
    return res.data.data;
  },

  async deactivateEmergency(): Promise<boolean> {
    const res = await apiClient.post<ApiResponse<boolean>>('/v1/safe-activation/deactivate', {}, {
      headers: getHeaders(true),
    });
    return res.data.data;
  },
};
