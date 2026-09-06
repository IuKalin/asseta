import React, { useEffect, useState } from 'react';
import { X, Activity, AlertTriangle, CheckCircle2, Lightbulb, ArrowRight } from 'lucide-react';
import { PlanReadinessAudit } from '../../../types/continuityPlan';

interface PlanReadinessAuditModalProps {
  isOpen: boolean;
  onClose: () => void;
  fetchAudit: () => Promise<PlanReadinessAudit>;
}

export const PlanReadinessAuditModal: React.FC<PlanReadinessAuditModalProps> = ({
  isOpen,
  onClose,
  fetchAudit,
}) => {
  const [audit, setAudit] = useState<PlanReadinessAudit | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (isOpen) {
      setLoading(true);
      fetchAudit()
        .then((data) => setAudit(data))
        .catch((err) => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [isOpen, fetchAudit]);

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4 overflow-y-auto">
      <div className="bg-white border border-slate-200 rounded-2xl w-full max-w-3xl max-h-[90vh] flex flex-col shadow-2xl overflow-hidden">
        {/* Header */}
        <div className="p-6 border-b border-slate-100 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="p-2 rounded-xl bg-emerald-50 border border-emerald-100 text-emerald-600">
              <Activity className="w-6 h-6" />
            </div>
            <div>
              <h3 className="text-xl font-bold text-slate-900">Kiểm Toán & Chẩn Đoán Kế Hoạch Tiếp Quản</h3>
              <p className="text-xs text-slate-500 mt-0.5">Phát hiện các điểm nghẽn, lỗ hổng phân công và khuyến nghị nâng cao điểm sẵn sàng.</p>
            </div>
          </div>
          <button
            type="button"
            onClick={onClose}
            aria-label="Đóng"
            className="p-1.5 text-slate-400 hover:text-slate-600 rounded-lg hover:bg-slate-100 transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Content */}
        <div className="p-6 overflow-y-auto space-y-6 flex-1 text-slate-800">
          {loading ? (
            <div className="py-12 text-center text-sm text-slate-500">Đang quét và kiểm toán dữ liệu kế hoạch…</div>
          ) : audit ? (
            <>
              {/* Overall Summary Bar */}
              <div className="grid grid-cols-3 gap-3 p-4 rounded-xl bg-slate-50 border border-slate-200 text-center">
                <div>
                  <div className="text-2xl font-black text-emerald-700 tabular-nums">{audit.overallScore}%</div>
                  <div className="text-[11px] text-slate-500 uppercase font-semibold mt-0.5">Điểm Tổng Thể</div>
                </div>
                <div>
                  <div className="text-2xl font-black text-sky-700 tabular-nums">{audit.delegateCoveragePercentage}%</div>
                  <div className="text-[11px] text-slate-500 uppercase font-semibold mt-0.5">Bao Phủ Người Gán</div>
                </div>
                <div>
                  <div className="text-2xl font-black text-emerald-600 tabular-nums">{audit.documentReadinessPercentage}%</div>
                  <div className="text-[11px] text-slate-500 uppercase font-semibold mt-0.5">Sẵn Sàng Hồ Sơ</div>
                </div>
              </div>

              {/* SPoF Warning */}
              {audit.hasSinglePointOfFailureRisk && (
                <div className="p-4 rounded-xl bg-amber-50 border border-amber-200 flex items-start gap-3">
                  <AlertTriangle className="w-5 h-5 text-amber-600 shrink-0 mt-0.5" />
                  <div className="text-xs text-amber-900">
                    <h4 className="font-bold text-amber-800 text-sm">Cảnh báo rủi ro Single Point of Failure (SPoF)</h4>
                    <p className="mt-1 leading-relaxed text-amber-700">{audit.singlePointOfFailureDetails}</p>
                  </div>
                </div>
              )}

              {/* Actionable Recommendations */}
              <div className="space-y-3">
                <h4 className="text-xs font-bold uppercase tracking-wider text-slate-500 flex items-center gap-2">
                  <Lightbulb className="w-4 h-4 text-amber-500" /> Khuyến Nghị Hành Động Để Hoàn Thiện Kế Hoạch
                </h4>
                <div className="space-y-2">
                  {audit.actionableRecommendations.map((rec, idx) => (
                    <div key={idx} className="p-3 rounded-xl bg-slate-50 border border-slate-200 flex items-start gap-2.5 text-xs text-slate-700">
                      <ArrowRight className="w-4 h-4 text-emerald-600 shrink-0 mt-0.5" />
                      <span>{rec}</span>
                    </div>
                  ))}
                </div>
              </div>

              {/* Identified Gaps Inventory */}
              <div className="space-y-3">
                <h4 className="text-xs font-bold uppercase tracking-wider text-slate-500 flex items-center gap-2">
                  <AlertTriangle className="w-4 h-4 text-rose-500" /> Danh Sách Lỗ Hổng Cần Xử Lý ({audit.identifiedGaps.length})
                </h4>

                {audit.identifiedGaps.length === 0 ? (
                  <div className="p-4 rounded-xl bg-emerald-50 border border-emerald-200 text-xs text-emerald-800 flex items-center gap-2">
                    <CheckCircle2 className="w-4 h-4 text-emerald-600" /> Kế hoạch của bạn hiện tại không có bất kỳ lỗ hổng nào!
                  </div>
                ) : (
                  <div className="space-y-2.5">
                    {audit.identifiedGaps.map((gap) => (
                      <div key={gap.cardId} className="p-3.5 rounded-xl bg-slate-50 border border-slate-200 space-y-1 text-xs">
                        <div className="flex items-center justify-between">
                          <span className="font-semibold text-slate-900">{gap.cardTitle}</span>
                          <span className="px-2 py-0.5 text-[10px] font-bold bg-slate-200 text-slate-700 rounded">
                            {gap.stage}
                          </span>
                        </div>
                        <div className="text-rose-600 font-medium">Lý do: {gap.gapReason}</div>
                        <div className="text-slate-600">Khuyến nghị: {gap.recommendedAction}</div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </>
          ) : (
            <div className="py-8 text-center text-rose-600">Không thể tải báo cáo kiểm toán.</div>
          )}
        </div>
      </div>
    </div>
  );
};
