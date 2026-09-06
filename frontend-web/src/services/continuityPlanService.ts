import { apiClient } from './apiClient';
import { ApiResponse } from '../types';
import {
  ContinuityPlan,
  ContinuityPlanCardItem,
  OfflineEmergencyBrief,
  PlanReadinessAudit
} from '../types/continuityPlan';
import { UrgencyStage } from '../types/actionCard';

const getHeaders = (idempotency: boolean = false) => {
  const headers: Record<string, string> = {
    'X-Correlation-Id': typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : 'mock-uuid',
  };
  if (idempotency && typeof crypto !== 'undefined' && crypto.randomUUID) {
    headers['Idempotency-Key'] = crypto.randomUUID();
  }
  return headers;
};

export const continuityPlanService = {
  async getContinuityPlan(): Promise<ContinuityPlan> {
    const res = await apiClient.get<ApiResponse<ContinuityPlan>>('/v1/continuity-plan', {
      headers: getHeaders(),
    });
    return res.data.data;
  },

  async getEmergencyBrief(): Promise<OfflineEmergencyBrief> {
    const res = await apiClient.get<ApiResponse<OfflineEmergencyBrief>>('/v1/continuity-plan/emergency-brief', {
      headers: getHeaders(),
    });
    return res.data.data;
  },

  async getPlanAudit(): Promise<PlanReadinessAudit> {
    const res = await apiClient.get<ApiResponse<PlanReadinessAudit>>('/v1/continuity-plan/audit', {
      headers: getHeaders(),
    });
    return res.data.data;
  },

  async getMyDelegatedPlan(): Promise<ContinuityPlan> {
    const res = await apiClient.get<ApiResponse<ContinuityPlan>>('/v1/continuity-plan/delegated', {
      headers: getHeaders(),
    });
    return res.data.data;
  },

  async updateCardStage(cardId: string, newStage: UrgencyStage, rowVersion: number): Promise<ContinuityPlanCardItem> {
    const res = await apiClient.patch<ApiResponse<ContinuityPlanCardItem>>(
      `/v1/continuity-plan/cards/${cardId}/stage`,
      { newStage, rowVersion },
      { headers: getHeaders(true) }
    );
    return res.data.data;
  },

  async toggleCardCompletion(cardId: string, rowVersion: number): Promise<ContinuityPlanCardItem> {
    const res = await apiClient.patch<ApiResponse<ContinuityPlanCardItem>>(
      `/v1/continuity-plan/cards/${cardId}/toggle-completion`,
      { rowVersion },
      { headers: getHeaders(true) }
    );
    return res.data.data;
  },
};
