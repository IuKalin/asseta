import { useState, useEffect, useCallback } from 'react';
import { continuityPlanService } from '../../../services/continuityPlanService';
import {
  ContinuityPlan,
  OfflineEmergencyBrief,
  PlanReadinessAudit
} from '../../../types/continuityPlan';
import { UrgencyStage } from '../../../types/actionCard';

export function useContinuityPlan() {
  const [plan, setPlan] = useState<ContinuityPlan | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchPlan = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await continuityPlanService.getContinuityPlan();
      setPlan(data);
    } catch (err: any) {
      setError(err?.response?.data?.error?.message || err.message || 'Không thể tải Kế Hoạch Tiếp Quản.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchPlan();
  }, [fetchPlan]);

  const updateCardStage = async (cardId: string, newStage: UrgencyStage, rowVersion: number) => {
    try {
      const updatedCard = await continuityPlanService.updateCardStage(cardId, newStage, rowVersion);
      await fetchPlan();
      return updatedCard;
    } catch (err: any) {
      throw new Error(err?.response?.data?.error?.message || err.message || 'Lỗi khi chuyển đổi mốc thời gian thẻ.');
    }
  };

  const toggleCompletion = async (cardId: string, rowVersion: number) => {
    try {
      const updatedCard = await continuityPlanService.toggleCardCompletion(cardId, rowVersion);
      await fetchPlan();
      return updatedCard;
    } catch (err: any) {
      throw new Error(err?.response?.data?.error?.message || err.message || 'Lỗi khi cập nhật trạng thái hoàn tất.');
    }
  };

  const fetchEmergencyBrief = async (): Promise<OfflineEmergencyBrief> => {
    return await continuityPlanService.getEmergencyBrief();
  };

  const fetchPlanAudit = async (): Promise<PlanReadinessAudit> => {
    return await continuityPlanService.getPlanAudit();
  };

  return {
    plan,
    loading,
    error,
    refreshPlan: fetchPlan,
    updateCardStage,
    toggleCompletion,
    fetchEmergencyBrief,
    fetchPlanAudit,
  };
}
