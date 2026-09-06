import React from 'react';
import { Zap, Clock, Calendar, ShieldCheck, ClipboardList } from 'lucide-react';
import { ActionCard, UrgencyStage } from '../../../types/actionCard';
import { ActionCardItemCard } from './ActionCardItemCard';

interface ActionCardTimelineViewProps {
  cards: ActionCard[];
  onSelectCard: (card: ActionCard) => void;
  onEditCard?: (card: ActionCard, e: React.MouseEvent) => void;
  onDeleteCard?: (card: ActionCard, e: React.MouseEvent) => void;
  onAddCardToStage?: (stage: UrgencyStage) => void;
}

interface StageColumnConfig {
  stage: UrgencyStage;
  title: string;
  subTitle: string;
  icon: React.ReactNode;
  borderColor: string;
  badgeBg: string;
}

const STAGE_CONFIGS: StageColumnConfig[] = [
  {
    stage: 'IMMEDIATE',
    title: 'Ngay Lập Tức',
    subTitle: '0 - 24 giờ đầu tiên',
    icon: <Zap className="h-5 w-5 text-red-600" aria-hidden="true" />,
    borderColor: 'border-red-200 bg-red-50/30',
    badgeBg: 'bg-red-50 text-red-700 border border-red-200',
  },
  {
    stage: 'FIRST_72_HOURS',
    title: '72 Giờ Đầu Tiên',
    subTitle: 'Ngày 1 - 3 sau sự kiện',
    icon: <Clock className="h-5 w-5 text-amber-600" aria-hidden="true" />,
    borderColor: 'border-amber-200 bg-amber-50/30',
    badgeBg: 'bg-amber-50 text-amber-700 border border-amber-200',
  },
  {
    stage: 'FIRST_7_DAYS',
    title: '7 Ngày Đầu',
    subTitle: 'Tuần đầu tiên',
    icon: <Calendar className="h-5 w-5 text-blue-600" aria-hidden="true" />,
    borderColor: 'border-blue-200 bg-blue-50/30',
    badgeBg: 'bg-blue-50 text-blue-700 border border-blue-200',
  },
  {
    stage: 'LONGER_TERM',
    title: 'Dài Hạn',
    subTitle: 'Sau tuần đầu tiên',
    icon: <ShieldCheck className="h-5 w-5 text-purple-600" aria-hidden="true" />,
    borderColor: 'border-purple-200 bg-purple-50/30',
    badgeBg: 'bg-purple-50 text-purple-700 border border-purple-200',
  },
];

export const ActionCardTimelineView: React.FC<ActionCardTimelineViewProps> = ({
  cards,
  onSelectCard,
  onEditCard,
  onDeleteCard,
  onAddCardToStage,
}) => {
  return (
    <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
      {STAGE_CONFIGS.map((config) => {
        const stageCards = cards.filter((c) => c.urgency === config.stage);

        return (
          <div
            key={config.stage}
            className={`flex flex-col rounded-2xl border ${config.borderColor} p-4 shadow-sm`}
          >
            {/* Stage Header */}
            <div className="mb-4 flex items-center justify-between border-b border-slate-200/80 pb-3">
              <div className="flex items-center gap-2">
                <span>{config.icon}</span>
                <div>
                  <h3 className="text-sm font-semibold text-slate-900">{config.title}</h3>
                  <p className="text-[11px] text-slate-500">{config.subTitle}</p>
                </div>
              </div>
              <span className={`rounded-full px-2 py-0.5 text-xs font-bold tabular-nums ${config.badgeBg}`}>
                {stageCards.length}
              </span>
            </div>

            {/* Stage Cards List */}
            <div className="flex-1 space-y-3 overflow-y-auto pr-0.5">
              {stageCards.length === 0 ? (
                <div className="flex flex-col items-center justify-center rounded-xl border border-dashed border-slate-200 bg-white/60 p-6 text-center text-slate-500">
                  <ClipboardList className="h-8 w-8 text-slate-400 mb-1" aria-hidden="true" />
                  <p className="text-xs">Chưa có thẻ hành động</p>
                  {onAddCardToStage && (
                    <button
                      type="button"
                      onClick={() => onAddCardToStage(config.stage)}
                      className="mt-3 rounded-lg bg-slate-100 px-2.5 py-1 text-xs font-medium text-slate-700 hover:bg-slate-200 hover:text-slate-900 transition-colors"
                    >
                      + Thêm thẻ
                    </button>
                  )}
                </div>
              ) : (
                stageCards.map((card) => (
                  <ActionCardItemCard
                    key={card.id}
                    card={card}
                    onClick={onSelectCard}
                    onEdit={onEditCard}
                    onDelete={onDeleteCard}
                  />
                ))
              )}
            </div>

            {/* Bottom Add Action */}
            {onAddCardToStage && stageCards.length > 0 && (
              <button
                type="button"
                onClick={() => onAddCardToStage(config.stage)}
                className="mt-3 w-full rounded-xl border border-dashed border-slate-300 bg-white/50 py-2 text-xs font-medium text-slate-600 hover:border-emerald-500 hover:bg-emerald-50/50 hover:text-emerald-700 transition-colors"
              >
                + Thêm thẻ {config.title}
              </button>
            )}
          </div>
        );
      })}
    </div>
  );
};
