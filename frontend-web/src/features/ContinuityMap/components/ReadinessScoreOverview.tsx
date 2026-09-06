import React from 'react';
import { AlertTriangle, CheckCircle2, ShieldAlert, ShieldCheck } from 'lucide-react';
import { ContinuityCategory } from '../../../types/continuity';

interface Props {
  overallScore: number;
  totalItems: number;
  totalGaps: number;
  categories: ContinuityCategory[];
  onOpenAssessment?: () => void;
}

export const ReadinessScoreOverview: React.FC<Props> = ({
  overallScore,
  totalItems,
  totalGaps,
  categories,
  onOpenAssessment,
}) => {
  const getScoreColor = (score: number) => {
    if (score >= 80) return 'text-emerald-700 border-emerald-300 bg-emerald-50';
    if (score >= 50) return 'text-amber-700 border-amber-300 bg-amber-50';
    return 'text-red-700 border-red-300 bg-red-50';
  };

  const getScoreProgressColor = (score: number) => {
    if (score >= 80) return 'bg-emerald-600';
    if (score >= 50) return 'bg-amber-500';
    return 'bg-red-500';
  };

  return (
    <div className="rounded-3xl bg-white border border-[#EBEBEB] p-6 sm:p-7 shadow-[0_4px_16px_rgba(0,0,0,0.04)]">
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-center">
        {/* Overall Score Circle & Summary */}
        <div className="flex items-center gap-6 border-b lg:border-b-0 lg:border-r border-[#EBEBEB] pb-6 lg:pb-0 lg:pr-6">
          <div
            className={`w-28 h-28 rounded-full border-4 flex flex-col items-center justify-center shrink-0 shadow-sm ${getScoreColor(
              overallScore
            )}`}
          >
            <span className="text-3xl font-extrabold font-mono tabular-nums">{overallScore}%</span>
            <span className="text-[10px] uppercase font-bold tracking-wider opacity-80 mt-0.5">
              Readiness
            </span>
          </div>

          <div className="space-y-1.5">
            <div className="flex items-center gap-2">
              {overallScore >= 80 ? (
                <ShieldCheck className="w-5 h-5 text-emerald-600" />
              ) : (
                <ShieldAlert className="w-5 h-5 text-amber-600" />
              )}
              <h3 className="text-lg font-bold text-slate-900">Chỉ Số Sẵn Sàng Tiếp Quản</h3>
            </div>
            <p className="text-xs text-slate-600 leading-relaxed">
              Đo lường mức độ đầy đủ thông tin pháp lý, vị trí hồ sơ và người phụ trách bàn giao.
            </p>
            <div className="flex items-center gap-4 pt-1 text-xs">
              <span className="text-slate-600">
                Tổng cộng: <strong className="text-slate-900 font-mono tabular-nums">{totalItems}</strong> hạng mục
              </span>
              {totalGaps > 0 ? (
                <span className="inline-flex items-center gap-1 text-red-600 font-medium">
                  <AlertTriangle className="w-3.5 h-3.5" />
                  <span className="tabular-nums">{totalGaps}</span> khoảng trống (Gap)
                </span>
              ) : (
                <span className="inline-flex items-center gap-1 text-emerald-600 font-medium">
                  <CheckCircle2 className="w-3.5 h-3.5" />
                  Không có gap
                </span>
              )}
            </div>
          </div>
        </div>

        {/* Categories Progress Bars */}
        <div className="lg:col-span-2 space-y-3">
          <div className="flex items-center justify-between">
            <span className="text-xs font-semibold text-slate-700 uppercase tracking-wider">
              Tiến Độ Từng Danh Mục
            </span>
            {onOpenAssessment && (
              <button
                type="button"
                onClick={onOpenAssessment}
                className="text-xs text-[#FF385C] hover:underline font-semibold transition cursor-pointer"
              >
                Khảo sát tiếp quản nhanh (12 câu hỏi)
              </button>
            )}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-2.5">
            {categories.map((cat) => (
              <div key={cat.categoryId} className="space-y-1">
                <div className="flex justify-between text-xs">
                  <span className="text-slate-700 font-medium truncate max-w-[180px]">
                    {cat.name}
                  </span>
                  <span className="font-mono text-slate-500 tabular-nums">
                    {cat.items.length} items • <strong className="text-slate-900">{cat.readinessScore}%</strong>
                  </span>
                </div>
                <div className="w-full h-1.5 bg-slate-100 rounded-full overflow-hidden">
                  <div
                    className={`h-full rounded-full transition-[width] duration-500 ${getScoreProgressColor(
                      cat.readinessScore
                    )}`}
                    style={{ width: `${Math.max(cat.readinessScore, 2)}%` }}
                  />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};
