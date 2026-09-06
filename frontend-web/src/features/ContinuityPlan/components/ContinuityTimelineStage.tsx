import React from 'react';
import { AlertCircle } from 'lucide-react';
import { ContinuityPlanStage } from '../../../types/continuityPlan';
import { ContinuityPlanCardItem } from './ContinuityPlanCardItem';
import { UrgencyStage } from '../../../types/actionCard';

interface ContinuityTimelineStageProps {
  stage: ContinuityPlanStage;
  onUpdateStage: (cardId: string, newStage: UrgencyStage, rowVersion: number) => Promise<void>;
  onToggleCompletion: (cardId: string, rowVersion: number) => Promise<void>;
}

export const ContinuityTimelineStage: React.FC<ContinuityTimelineStageProps> = ({
  stage,
  onUpdateStage,
  onToggleCompletion,
}) => {
  const getStageColorTheme = (stg: UrgencyStage) => {
    switch (stg) {
      case 'IMMEDIATE':
        return {
          border: 'border-rose-200',
          badge: 'bg-rose-50 text-rose-700 border-rose-200',
          accent: 'text-rose-600',
        };
      case 'FIRST_72_HOURS':
        return {
          border: 'border-amber-200',
          badge: 'bg-amber-50 text-amber-700 border-amber-200',
          accent: 'text-amber-600',
        };
      case 'FIRST_7_DAYS':
        return {
          border: 'border-blue-200',
          badge: 'bg-blue-50 text-blue-700 border-blue-200',
          accent: 'text-blue-600',
        };
      default:
        return {
          border: 'border-purple-200',
          badge: 'bg-purple-50 text-purple-700 border-purple-200',
          accent: 'text-purple-600',
        };
    }
  };

  const theme = getStageColorTheme(stage.stage);

  return (
    <div className="flex flex-col rounded-2xl bg-white border border-slate-200 p-4 space-y-4 shadow-sm">
      {/* Stage Header */}
      <div className="flex items-start justify-between gap-3 pb-3 border-b border-slate-100">
        <div>
          <div className="flex items-center gap-2">
            <span className={`px-2.5 py-0.5 text-xs font-bold border rounded-full ${theme.badge}`}>
              {stage.stageName}
            </span>
            <span className="text-xs text-slate-500 font-medium tabular-nums">
              ({stage.totalCardsCount} việc)
            </span>
          </div>
          <p className="text-xs text-slate-500 mt-1 leading-relaxed">
            {stage.stageDescription}
          </p>
        </div>

        {/* Gap Indicator */}
        {stage.gapCardsCount > 0 && (
          <span className="px-2 py-0.5 text-[11px] font-bold bg-amber-50 text-amber-700 border border-amber-200 rounded-full flex items-center gap-1 shrink-0 tabular-nums">
            <AlertCircle className="w-3 h-3" /> {stage.gapCardsCount} gap
          </span>
        )}
      </div>

      {/* Cards List */}
      <div className="space-y-3 flex-1">
        {stage.cards.length === 0 ? (
          <div className="py-8 text-center text-xs text-slate-500 border border-dashed border-slate-200 rounded-xl bg-slate-50">
            Chưa có công việc nào trong mốc này.
          </div>
        ) : (
          stage.cards.map((card) => (
            <ContinuityPlanCardItem
              key={card.id}
              card={card}
              onUpdateStage={onUpdateStage}
              onToggleCompletion={onToggleCompletion}
            />
          ))
        )}
      </div>
    </div>
  );
};
