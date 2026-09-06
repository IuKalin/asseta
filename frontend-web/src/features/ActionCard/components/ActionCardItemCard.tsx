import React from 'react';
import { User, MapPin, Lock, Phone, Pencil, Trash2 } from 'lucide-react';
import { ActionCard, CardPriority } from '../../../types/actionCard';

interface ActionCardItemCardProps {
  card: ActionCard;
  onClick: (card: ActionCard) => void;
  onEdit?: (card: ActionCard, e: React.MouseEvent) => void;
  onDelete?: (card: ActionCard, e: React.MouseEvent) => void;
}

const getPriorityBadgeClass = (priority: CardPriority) => {
  switch (priority) {
    case 'CRITICAL':
      return 'bg-red-50 text-red-700 border-red-200';
    case 'IMPORTANT':
      return 'bg-amber-50 text-amber-700 border-amber-200';
    case 'LOW':
      return 'bg-sky-50 text-sky-700 border-sky-200';
    default:
      return 'bg-slate-100 text-slate-700 border-slate-200';
  }
};

const getCategoryLabel = (code: string) => {
  switch (code?.toUpperCase()) {
    case 'FINANCIAL':
      return 'Tài chính & Ngân hàng';
    case 'PROPERTY':
      return 'Bất động sản & Tài sản';
    case 'INSURANCE':
      return 'Bảo hiểm & Y tế';
    case 'BUSINESS':
      return 'Vận hành Doanh nghiệp';
    case 'DOCUMENTS':
      return 'Hồ sơ Pháp lý';
    case 'FAMILY':
      return 'Gia đình & Người phụ thuộc';
    default:
      return code || 'Tài sản';
  }
};

export const ActionCardItemCard: React.FC<ActionCardItemCardProps> = ({
  card,
  onClick,
  onEdit,
  onDelete,
}) => {
  const completedSteps = card.steps.filter((s) => s.isCompleted).length;
  const totalSteps = card.steps.length;
  const progressPercent = totalSteps > 0 ? Math.round((completedSteps / totalSteps) * 100) : 0;

  return (
    <div
      role="button"
      tabIndex={0}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault();
          onClick(card);
        }
      }}
      onClick={() => onClick(card)}
      className="group relative cursor-pointer rounded-xl border border-slate-200 bg-white p-4 transition-[transform,box-shadow,border-color] duration-200 hover:-translate-y-0.5 hover:border-slate-300 hover:shadow-md text-left"
    >
      {/* Top badges */}
      <div className="mb-2 flex items-center justify-between gap-2">
        <span className="rounded bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-700">
          {getCategoryLabel(card.categoryCode)}
        </span>
        <span
          className={`rounded border px-2 py-0.5 text-[11px] font-bold uppercase tracking-wider ${getPriorityBadgeClass(
            card.priority
          )}`}
        >
          {card.priority}
        </span>
      </div>

      {/* Title */}
      <h4 className="line-clamp-2 text-base font-semibold text-slate-900 group-hover:text-emerald-700 transition-colors">
        {card.title}
      </h4>

      {/* Summary hint */}
      {card.summary && (
        <p className="mt-1 line-clamp-2 text-xs text-slate-500">{card.summary}</p>
      )}

      {/* Steps progress bar */}
      <div className="mt-3">
        <div className="flex items-center justify-between text-xs text-slate-500">
          <span>Tiến độ thực thi</span>
          <span className="font-medium text-slate-700 tabular-nums">
            {completedSteps}/{totalSteps} bước ({progressPercent}%)
          </span>
        </div>
        <div className="mt-1 h-1.5 w-full overflow-hidden rounded-full bg-slate-100">
          <div
            className={`h-full transition-[width] duration-300 ${progressPercent === 100 ? 'bg-emerald-600' : 'bg-emerald-500'
              }`}
            style={{ width: `${progressPercent}%` }}
          />
        </div>
      </div>

      {/* Meta indicators */}
      <div className="mt-3 flex flex-wrap items-center gap-2 text-xs text-slate-600">
        {card.assignedTrustedPersonId && (
          <span className="inline-flex items-center gap-1 rounded bg-slate-100 px-1.5 py-0.5 text-slate-700">
            <User className="w-3 h-3 text-slate-500" /> Đã gán ủy thác
          </span>
        )}
        {card.documentLocationHint && (
          <span className="inline-flex items-center gap-1 rounded bg-slate-100 px-1.5 py-0.5 text-slate-700 truncate max-w-[140px]" title={card.documentLocationHint}>
            <MapPin className="w-3 h-3 text-slate-500" /> {card.documentLocationHint}
          </span>
        )}
        {card.hasConfidentialInstructions && (
          <span className="inline-flex items-center gap-1 rounded bg-purple-50 border border-purple-200 px-1.5 py-0.5 text-purple-700 font-mono text-[11px]">
            <Lock className="w-3 h-3 text-purple-600" /> Mật mã AES-256
          </span>
        )}
        {card.contacts.length > 0 && (
          <span className="inline-flex items-center gap-1 rounded bg-slate-100 px-1.5 py-0.5 text-slate-700 tabular-nums">
            <Phone className="w-3 h-3 text-slate-500" /> {card.contacts.length} liên hệ
          </span>
        )}
      </div>

      {/* Action buttons (hover) */}
      <div className="mt-3 flex items-center justify-end gap-2 border-t border-slate-200 pt-2 opacity-80 group-hover:opacity-100">
        {onEdit && (
          <button
            type="button"
            onClick={(e) => onEdit(card, e)}
            className="rounded p-1 text-slate-500 hover:bg-slate-100 hover:text-slate-900 transition"
            title="Chỉnh sửa thẻ"
            aria-label="Chỉnh sửa thẻ"
          >
            <Pencil className="w-3.5 h-3.5" />
          </button>
        )}
        {onDelete && (
          <button
            type="button"
            onClick={(e) => onDelete(card, e)}
            className="rounded p-1 text-slate-500 hover:bg-red-50 hover:text-red-600 transition"
            title="Xóa thẻ"
            aria-label="Xóa thẻ"
          >
            <Trash2 className="w-3.5 h-3.5" />
          </button>
        )}
      </div>
    </div>
  );
};
