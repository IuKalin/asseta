import React, { useState } from 'react';
import {
  Briefcase,
  ChevronDown,
  ChevronRight,
  FileText,
  Home,
  Plus,
  Shield,
  Users,
  Wallet,
} from 'lucide-react';
import { ContinuityCategory, ContinuityItem } from '../../../types/continuity';
import { ContinuityItemCard } from './ContinuityItemCard';

interface Props {
  category: ContinuityCategory;
  isKeyUnlocked: boolean;
  onAddItem: (categoryId: string) => void;
  onEditItem: (item: ContinuityItem) => void;
  onDeleteItem: (id: string) => void;
  onDecryptNotes: (item: ContinuityItem) => Promise<string>;
  onPromptUnlock: () => void;
}

export const CategoryAccordion: React.FC<Props> = ({
  category,
  isKeyUnlocked,
  onAddItem,
  onEditItem,
  onDeleteItem,
  onDecryptNotes,
  onPromptUnlock,
}) => {
  const [isOpen, setIsOpen] = useState<boolean>(true);

  const getCategoryIcon = (icon: string) => {
    switch (icon) {
      case 'wallet':
        return <Wallet className="w-5 h-5 text-emerald-600" />;
      case 'home':
        return <Home className="w-5 h-5 text-sky-600" />;
      case 'shield':
        return <Shield className="w-5 h-5 text-indigo-600" />;
      case 'briefcase':
        return <Briefcase className="w-5 h-5 text-amber-600" />;
      case 'file-text':
        return <FileText className="w-5 h-5 text-purple-600" />;
      default:
        return <Users className="w-5 h-5 text-rose-600" />;
    }
  };

  const getScoreColor = (score: number) => {
    if (score >= 80) return 'text-emerald-700 bg-emerald-50 border-emerald-200';
    if (score >= 50) return 'text-amber-700 bg-amber-50 border-amber-200';
    return 'text-red-700 bg-red-50 border-red-200';
  };

  return (
    <div className="rounded-2xl bg-white border border-slate-200 overflow-hidden shadow-sm transition">
      {/* Category Header */}
      <div className="p-4 sm:p-5 flex items-center justify-between gap-4 select-none">
        <button
          type="button"
          onClick={() => setIsOpen(!isOpen)}
          className="flex items-center gap-3 text-left flex-1 group"
        >
          <div className="p-2 rounded-xl bg-slate-100 group-hover:bg-slate-200 transition">
            {getCategoryIcon(category.icon)}
          </div>
          <div>
            <div className="flex items-center gap-2">
              <h3 className="text-base font-bold text-slate-900 group-hover:text-emerald-700 transition">
                {category.name}
              </h3>
              <span
                className={`text-[10px] font-mono font-bold px-2 py-0.5 rounded-full border tabular-nums ${getScoreColor(
                  category.readinessScore
                )}`}
              >
                {category.readinessScore}% Sẵn sàng
              </span>
            </div>
            <p className="text-xs text-slate-500 mt-0.5">
              <span className="tabular-nums">{category.items.length}</span> hạng mục tiếp quản đã đăng ký
            </p>
          </div>
        </button>

        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={() => onAddItem(category.categoryId)}
            className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-medium border border-slate-200 transition"
          >
            <Plus className="w-3.5 h-3.5 text-emerald-600" />
            <span className="hidden sm:inline">Thêm mục</span>
          </button>

          <button
            type="button"
            onClick={() => setIsOpen(!isOpen)}
            aria-label={isOpen ? "Thu gọn danh mục" : "Mở rộng danh mục"}
            className="p-1.5 rounded-lg text-slate-500 hover:text-slate-900 hover:bg-slate-100 transition"
          >
            {isOpen ? <ChevronDown className="w-5 h-5" /> : <ChevronRight className="w-5 h-5" />}
          </button>
        </div>
      </div>

      {/* Accordion Content */}
      {isOpen && (
        <div className="px-4 pb-4 sm:px-5 sm:pb-5 pt-1 border-t border-slate-200">
          {category.items.length === 0 ? (
            <div className="text-center py-8 rounded-xl border border-dashed border-slate-200 bg-slate-50">
              <p className="text-xs text-slate-500">Danh mục này hiện chưa có hạng mục tiếp quản nào.</p>
              <button
                type="button"
                onClick={() => onAddItem(category.categoryId)}
                className="mt-3 inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-emerald-50 hover:bg-emerald-100 text-emerald-700 border border-emerald-200 text-xs font-medium transition"
              >
                <Plus className="w-3.5 h-3.5 text-emerald-600" /> Thêm hạng mục đầu tiên
              </button>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3.5 mt-2">
              {category.items.map((item) => (
                <ContinuityItemCard
                  key={item.id}
                  item={item}
                  isKeyUnlocked={isKeyUnlocked}
                  onEdit={onEditItem}
                  onDelete={onDeleteItem}
                  onDecryptNotes={onDecryptNotes}
                  onPromptUnlock={onPromptUnlock}
                />
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
};
