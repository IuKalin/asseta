import React from 'react';
import { AlertCircle, ArrowRight } from 'lucide-react';
import { ContinuityGap } from '../../../types/continuity';

interface Props {
  gaps: ContinuityGap[];
  onResolveGap?: (gap: ContinuityGap) => void;
}

export const ContinuityGapBanner: React.FC<Props> = ({ gaps, onResolveGap }) => {
  if (!gaps || gaps.length === 0) return null;

  return (
    <div className="rounded-xl bg-amber-50 border border-amber-200 p-4 text-amber-800 shadow-sm">
      <div className="flex items-start gap-3">
        <AlertCircle className="w-5 h-5 text-amber-600 shrink-0 mt-0.5" />
        <div className="flex-1">
          <div className="flex items-center justify-between">
            <h4 className="text-sm font-semibold text-amber-900">
              Cảnh báo Khoảng Trống Tiếp Quản (<span className="tabular-nums">{gaps.length}</span> hạng mục quan trọng chưa hoàn thiện)
            </h4>
          </div>
          <p className="text-xs text-amber-800 mt-1">
            Các hạng mục mức độ Ưu tiên cao (Critical/Important) dưới đây đang thiếu Người phụ trách tiếp quản hoặc Gợi ý vị trí hồ sơ:
          </p>

          <div className="flex flex-wrap gap-2 mt-3">
            {gaps.slice(0, 5).map((gap) => (
              <button
                key={gap.itemId}
                type="button"
                onClick={() => onResolveGap?.(gap)}
                className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-white hover:bg-amber-100 border border-amber-300 text-xs text-amber-900 transition group shadow-sm"
              >
                <span className="font-medium truncate max-w-[200px]">{gap.itemName}</span>
                <span className="text-[10px] text-amber-700 font-mono">
                  [{gap.missingFields.join(', ')}]
                </span>
                <ArrowRight className="w-3 h-3 text-amber-600 group-hover:translate-x-0.5 transition" />
              </button>
            ))}
            {gaps.length > 5 && (
              <span className="text-xs text-amber-700 self-center">
                +<span className="tabular-nums">{gaps.length - 5}</span> mục khác…
              </span>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};
