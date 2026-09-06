import React from 'react';
import {
  ShieldAlert,
  Users,
  FileCheck,
  AlertTriangle,
  Printer,
  Activity
} from 'lucide-react';
import { ContinuityPlan } from '../../../types/continuityPlan';

interface PlanMetricsHeaderProps {
  plan: ContinuityPlan;
  onOpenEmergencyBrief: () => void;
  onOpenAudit: () => void;
}

export const PlanMetricsHeader: React.FC<PlanMetricsHeaderProps> = ({
  plan,
  onOpenEmergencyBrief,
  onOpenAudit,
}) => {
  const getScoreColor = (score: number) => {
    if (score >= 80) return 'text-emerald-700 border-emerald-200 bg-emerald-50/70';
    if (score >= 50) return 'text-amber-700 border-amber-200 bg-amber-50/70';
    return 'text-rose-700 border-rose-200 bg-rose-50/70';
  };

  return (
    <div className="space-y-4">
      {/* SPoF Warning Banner if detected */}
      {plan.hasSinglePointOfFailureRisk && (
        <div className="p-4 rounded-xl bg-amber-50 border border-amber-200 flex items-start gap-3">
          <AlertTriangle className="w-5 h-5 text-amber-600 mt-0.5 shrink-0" />
          <div className="flex-1">
            <h4 className="text-sm font-semibold text-amber-800">Cảnh Báo Rủi Ro Điểm Nghẽn Đơn Lẻ (Single Point of Failure)</h4>
            <p className="text-xs text-amber-700 mt-0.5">{plan.singlePointOfFailureWarning}</p>
          </div>
          <button
            type="button"
            onClick={onOpenAudit}
            className="px-3 py-1.5 text-xs font-medium bg-amber-100 hover:bg-amber-200 text-amber-800 border border-amber-300 rounded-lg transition-colors"
          >
            Xem Khuyến Nghị
          </button>
        </div>
      )}

      {/* KPI Cards Grid */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {/* Score Card */}
        <div className={`p-4 rounded-xl border flex items-center gap-4 ${getScoreColor(plan.overallPlanReadinessScore)}`}>
          <div className="text-3xl font-extrabold tabular-nums">{plan.overallPlanReadinessScore}%</div>
          <div>
            <div className="text-xs uppercase tracking-wider text-slate-600 font-semibold">Điểm Sẵn Sàng Kế Hoạch</div>
            <div className="text-xs text-slate-500 mt-0.5">
              {plan.overallPlanReadinessScore >= 80 ? 'Chuẩn bị rất tốt' : plan.overallPlanReadinessScore >= 50 ? 'Cần bổ sung' : 'Chưa hoàn thiện'}
            </div>
          </div>
        </div>

        {/* Delegate Coverage */}
        <div className="p-4 rounded-xl bg-white border border-slate-200 flex flex-col justify-between shadow-xs">
          <div className="flex items-center justify-between text-xs text-slate-600">
            <span className="flex items-center gap-1.5 font-medium"><Users className="w-4 h-4 text-sky-600" /> Bao Phủ Người Ủy Thác</span>
            <span className="font-bold text-slate-900 tabular-nums">{plan.delegateCoveragePercentage}%</span>
          </div>
          <div className="w-full bg-slate-100 rounded-full h-2 mt-3">
            <div
              className="bg-sky-500 h-2 rounded-full transition-[width] duration-500"
              style={{ width: `${plan.delegateCoveragePercentage}%` }}
            />
          </div>
          <span className="text-[11px] text-slate-500 mt-2">
            Đã gán người phụ trách cho phần lớn thẻ
          </span>
        </div>

        {/* Document Readiness */}
        <div className="p-4 rounded-xl bg-white border border-slate-200 flex flex-col justify-between shadow-xs">
          <div className="flex items-center justify-between text-xs text-slate-600">
            <span className="flex items-center gap-1.5 font-medium"><FileCheck className="w-4 h-4 text-emerald-600" /> Sẵn Sàng Vị Trí Hồ Sơ</span>
            <span className="font-bold text-slate-900 tabular-nums">{plan.documentReadinessPercentage}%</span>
          </div>
          <div className="w-full bg-slate-100 rounded-full h-2 mt-3">
            <div
              className="bg-emerald-500 h-2 rounded-full transition-[width] duration-500"
              style={{ width: `${plan.documentReadinessPercentage}%` }}
            />
          </div>
          <span className="text-[11px] text-slate-500 mt-2">
            Có chỉ dẫn nơi lưu trữ giấy tờ vật lý/số
          </span>
        </div>

        {/* Gaps & Quick Actions */}
        <div className="p-4 rounded-xl bg-white border border-slate-200 flex flex-col justify-between shadow-xs">
          <div className="flex items-center justify-between">
            <div className="text-xs text-slate-600 font-medium flex items-center gap-1.5">
              <ShieldAlert className="w-4 h-4 text-rose-500" /> Lỗ Hổng Tiếp Quản
            </div>
            <span className={`px-2 py-0.5 text-xs font-bold rounded-full border tabular-nums ${plan.gapsCount > 0 ? 'bg-rose-50 text-rose-700 border-rose-200' : 'bg-emerald-50 text-emerald-700 border-emerald-200'}`}>
              {plan.gapsCount} gaps
            </span>
          </div>

          <div className="flex gap-2 mt-3">
            <button
              type="button"
              onClick={onOpenEmergencyBrief}
              className="flex-1 px-3 py-1.5 text-xs font-medium bg-slate-100 hover:bg-slate-200 text-slate-800 rounded-lg flex items-center justify-center gap-1.5 transition-colors"
            >
              <Printer className="w-3.5 h-3.5 text-emerald-600" /> Tóm Lược
            </button>
            <button
              type="button"
              onClick={onOpenAudit}
              className="flex-1 px-3 py-1.5 text-xs font-medium bg-emerald-600 hover:bg-emerald-500 text-white rounded-lg flex items-center justify-center gap-1.5 transition-colors shadow-xs"
            >
              <Activity className="w-3.5 h-3.5" /> Kiểm Toán
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
