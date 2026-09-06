import React, { useState } from 'react';
import {
  CheckCircle2,
  Circle,
  AlertTriangle,
  User,
  Phone,
  FileText,
  ArrowRightLeft,
  ListOrdered,
  Users
} from 'lucide-react';
import { ContinuityPlanCardItem as CardItemType } from '../../../types/continuityPlan';
import { UrgencyStage } from '../../../types/actionCard';

interface ContinuityPlanCardItemProps {
  card: CardItemType;
  onUpdateStage: (cardId: string, newStage: UrgencyStage, rowVersion: number) => Promise<void>;
  onToggleCompletion: (cardId: string, rowVersion: number) => Promise<void>;
}

export const ContinuityPlanCardItem: React.FC<ContinuityPlanCardItemProps> = ({
  card,
  onUpdateStage,
  onToggleCompletion,
}) => {
  const [updating, setUpdating] = useState(false);
  const [showStageMenu, setShowStageMenu] = useState(false);

  const getPriorityBadge = (priority: string) => {
    switch (priority) {
      case 'CRITICAL':
        return <span className="px-2 py-0.5 text-[10px] font-bold bg-rose-50 text-rose-700 border border-rose-200 rounded">CRITICAL</span>;
      case 'IMPORTANT':
        return <span className="px-2 py-0.5 text-[10px] font-bold bg-amber-50 text-amber-700 border border-amber-200 rounded">IMPORTANT</span>;
      default:
        return <span className="px-2 py-0.5 text-[10px] font-medium bg-slate-100 text-slate-600 border border-slate-200 rounded">LOW</span>;
    }
  };

  const handleStageChange = async (targetStage: UrgencyStage) => {
    if (targetStage === card.urgency) return;
    setUpdating(true);
    setShowStageMenu(false);
    try {
      await onUpdateStage(card.id, targetStage, card.rowVersion);
    } finally {
      setUpdating(false);
    }
  };

  const handleToggle = async () => {
    setUpdating(true);
    try {
      await onToggleCompletion(card.id, card.rowVersion);
    } finally {
      setUpdating(false);
    }
  };

  return (
    <div className={`p-4 rounded-xl border transition-colors duration-200 ${card.isCompleted
        ? 'bg-slate-50 border-slate-200 opacity-75'
        : card.hasStageGap
          ? 'bg-white border-amber-300 shadow-sm shadow-amber-500/10'
          : 'bg-white border-slate-200 hover:border-slate-300 shadow-xs'
      }`}>
      {/* Header: Checkbox, Title & Priority */}
      <div className="flex items-start gap-3">
        <button
          type="button"
          onClick={handleToggle}
          disabled={updating}
          className="mt-0.5 text-slate-400 hover:text-emerald-600 transition-colors"
          title={card.isCompleted ? 'Đánh dấu chưa xong' : 'Đánh dấu đã sẵn sàng'}
          aria-label={card.isCompleted ? 'Đánh dấu chưa xong' : 'Đánh dấu đã sẵn sàng'}
        >
          {card.isCompleted ? (
            <CheckCircle2 className="w-5 h-5 text-emerald-600" />
          ) : (
            <Circle className="w-5 h-5" />
          )}
        </button>

        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2 flex-wrap mb-1">
            <span className="text-[11px] font-semibold text-slate-700 bg-slate-100 px-2 py-0.5 rounded">
              {card.categoryName}
            </span>
            {getPriorityBadge(card.priority)}
          </div>
          <h4 className={`text-sm font-semibold leading-snug ${card.isCompleted ? 'line-through text-slate-400' : 'text-slate-900'}`}>
            {card.title}
          </h4>
          {card.summary && (
            <p className="text-xs text-slate-500 mt-1 line-clamp-2">{card.summary}</p>
          )}
        </div>
      </div>

      {/* Stage Gap Warning Pill */}
      {card.hasStageGap && !card.isCompleted && (
        <div className="mt-3 py-1 px-2.5 rounded-lg bg-amber-50 border border-amber-200 flex items-center gap-1.5 text-xs text-amber-800">
          <AlertTriangle className="w-3.5 h-3.5 shrink-0 text-amber-600" />
          <span>Lỗ hổng: {!card.assignedTrustedPersonId ? 'Chưa có người phụ trách' : 'Chưa có vị trí hồ sơ'}</span>
        </div>
      )}

      {/* Metadata Row: Assigned Person & Document Location */}
      <div className="mt-3 pt-3 border-t border-slate-100 grid grid-cols-1 gap-2 text-xs">
        {/* Person */}
        <div className="flex items-center justify-between text-slate-600">
          <span className="flex items-center gap-1.5 text-slate-400">
            <User className="w-3.5 h-3.5 text-sky-600" /> Phụ trách:
          </span>
          {card.assignedTrustedPersonName ? (
            <span className="font-medium text-slate-900 flex items-center gap-1">
              {card.assignedTrustedPersonName}
              {card.assignedTrustedPersonPhone && (
                <span title={card.assignedTrustedPersonPhone}>
                  <Phone className="w-3 h-3 text-slate-400" />
                </span>
              )}
            </span>
          ) : (
            <span className="text-rose-600 italic">Chưa chỉ định</span>
          )}
        </div>

        {/* Location Hint */}
        {card.documentLocationHint && (
          <div className="flex items-center justify-between text-slate-600">
            <span className="flex items-center gap-1.5 text-slate-400">
              <FileText className="w-3.5 h-3.5 text-emerald-600" /> Hồ sơ:
            </span>
            <span className="font-medium text-slate-700 truncate max-w-[180px]" title={card.documentLocationHint}>
              {card.documentLocationHint}
            </span>
          </div>
        )}
      </div>

      {/* Footer: Steps/Contacts count and Move Stage dropdown */}
      <div className="mt-3 pt-2 border-t border-slate-100 flex items-center justify-between text-[11px] text-slate-500">
        <div className="flex items-center gap-3">
          <span className="flex items-center gap-1"><ListOrdered className="w-3 h-3" /> {card.stepsCount} bước</span>
          <span className="flex items-center gap-1"><Users className="w-3 h-3" /> {card.contactsCount} đầu mối</span>
        </div>

        {/* Move Stage Selector */}
        <div className="relative">
          <button
            type="button"
            onClick={() => setShowStageMenu(!showStageMenu)}
            disabled={updating}
            className="flex items-center gap-1 px-2 py-1 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded transition-colors"
            title="Chuyển mốc thời gian"
          >
            <ArrowRightLeft className="w-3 h-3" /> Đổi Mốc
          </button>

          {showStageMenu && (
            <div className="absolute right-0 bottom-full mb-1 w-44 bg-white border border-slate-200 rounded-lg shadow-xl p-1 z-20 space-y-0.5">
              <button
                type="button"
                onClick={() => handleStageChange('IMMEDIATE')}
                className={`w-full text-left px-2.5 py-1.5 text-xs rounded flex items-center justify-between ${card.urgency === 'IMMEDIATE' ? 'bg-rose-50 text-rose-700 font-semibold' : 'hover:bg-slate-100 text-slate-700'}`}
              >
                <span>Immediate (NOW)</span>
                {card.urgency === 'IMMEDIATE' && <CheckCircle2 className="w-3 h-3 text-rose-600" />}
              </button>
              <button
                type="button"
                onClick={() => handleStageChange('FIRST_72_HOURS')}
                className={`w-full text-left px-2.5 py-1.5 text-xs rounded flex items-center justify-between ${card.urgency === 'FIRST_72_HOURS' ? 'bg-amber-50 text-amber-700 font-semibold' : 'hover:bg-slate-100 text-slate-700'}`}
              >
                <span>First 72 Hours</span>
                {card.urgency === 'FIRST_72_HOURS' && <CheckCircle2 className="w-3 h-3 text-amber-600" />}
              </button>
              <button
                type="button"
                onClick={() => handleStageChange('FIRST_7_DAYS')}
                className={`w-full text-left px-2.5 py-1.5 text-xs rounded flex items-center justify-between ${card.urgency === 'FIRST_7_DAYS' ? 'bg-blue-50 text-blue-700 font-semibold' : 'hover:bg-slate-100 text-slate-700'}`}
              >
                <span>First 7 Days</span>
                {card.urgency === 'FIRST_7_DAYS' && <CheckCircle2 className="w-3 h-3 text-blue-600" />}
              </button>
              <button
                type="button"
                onClick={() => handleStageChange('LONGER_TERM')}
                className={`w-full text-left px-2.5 py-1.5 text-xs rounded flex items-center justify-between ${card.urgency === 'LONGER_TERM' ? 'bg-purple-50 text-purple-700 font-semibold' : 'hover:bg-slate-100 text-slate-700'}`}
              >
                <span>Longer-Term</span>
                {card.urgency === 'LONGER_TERM' && <CheckCircle2 className="w-3 h-3 text-purple-600" />}
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
