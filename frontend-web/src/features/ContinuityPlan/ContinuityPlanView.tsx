import React, { useState } from 'react';
import {
  Clock,
  LayoutGrid,
  Columns3,
  Search,
  RefreshCw,
  AlertTriangle,
  Loader2
} from 'lucide-react';
import { useContinuityPlan } from './hooks/useContinuityPlan';
import { PlanMetricsHeader } from './components/PlanMetricsHeader';
import { ContinuityTimelineStage } from './components/ContinuityTimelineStage';
import { EmergencyBriefModal } from './components/EmergencyBriefModal';
import { PlanReadinessAuditModal } from './components/PlanReadinessAuditModal';
import { UrgencyStage } from '../../types/actionCard';

export const ContinuityPlanView: React.FC = () => {
  const {
    plan,
    loading,
    error,
    refreshPlan,
    updateCardStage,
    toggleCompletion,
    fetchEmergencyBrief,
    fetchPlanAudit,
  } = useContinuityPlan();

  const [viewMode, setViewMode] = useState<'timeline' | 'kanban'>('kanban');
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('ALL');
  const [onlyGaps, setOnlyGaps] = useState(false);

  const [isEmergencyBriefOpen, setIsEmergencyBriefOpen] = useState(false);
  const [isAuditOpen, setIsAuditOpen] = useState(false);

  // Extract unique categories for filter
  const allCategories = plan
    ? Array.from(
      new Set(
        plan.stages.flatMap((s) => s.cards.map((c) => c.categoryName))
      )
    )
    : [];

  const handleUpdateStage = async (cardId: string, newStage: UrgencyStage, rowVersion: number) => {
    await updateCardStage(cardId, newStage, rowVersion);
  };

  const handleToggleCompletion = async (cardId: string, rowVersion: number) => {
    await toggleCompletion(cardId, rowVersion);
  };

  // Filter cards per stage
  const getFilteredStages = () => {
    if (!plan) return [];
    return plan.stages.map((stage) => {
      const filteredCards = stage.cards.filter((card) => {
        const matchesSearch =
          !searchQuery.trim() ||
          card.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
          (card.assignedTrustedPersonName &&
            card.assignedTrustedPersonName.toLowerCase().includes(searchQuery.toLowerCase()));

        const matchesCategory =
          selectedCategory === 'ALL' || card.categoryName === selectedCategory;

        const matchesGaps = !onlyGaps || card.hasStageGap;

        return matchesSearch && matchesCategory && matchesGaps;
      });

      return {
        ...stage,
        cards: filteredCards,
      };
    });
  };

  const filteredStages = getFilteredStages();

  return (
    <div className="space-y-6">
      {/* Top Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-black text-slate-900 tracking-tight flex items-center gap-2.5">
            <Clock className="w-6 h-6 text-emerald-600" />
            Continuity Plan – Kế Hoạch Tiếp Quản Tổng Thể
          </h2>
          <p className="text-xs text-slate-600 mt-1">
            Điều phối toàn bộ thẻ hành động theo 4 mốc thời gian thực thi: Immediate (NOW) &rarr; 72 Hours &rarr; 7 Days &rarr; 30 Days.
          </p>
        </div>

        {/* View Mode Toggle & Refresh */}
        <div className="flex items-center gap-2">
          <div className="p-1 rounded-xl bg-slate-100 border border-slate-200 flex items-center">
            <button
              type="button"
              onClick={() => setViewMode('kanban')}
              className={`px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center gap-1.5 transition-colors ${viewMode === 'kanban'
                  ? 'bg-white text-slate-900 shadow-xs'
                  : 'text-slate-600 hover:text-slate-900'
                }`}
            >
              <Columns3 className="w-3.5 h-3.5" /> 4 Cột Kanban
            </button>
            <button
              type="button"
              onClick={() => setViewMode('timeline')}
              className={`px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center gap-1.5 transition-colors ${viewMode === 'timeline'
                  ? 'bg-white text-slate-900 shadow-xs'
                  : 'text-slate-600 hover:text-slate-900'
                }`}
            >
              <LayoutGrid className="w-3.5 h-3.5" /> Dòng Thời Gian
            </button>
          </div>

          <button
            type="button"
            onClick={() => refreshPlan()}
            disabled={loading}
            className="p-2 bg-white border border-slate-200 hover:border-slate-300 text-slate-600 hover:text-slate-900 rounded-xl transition-colors shadow-xs"
            title="Tải lại dữ liệu"
            aria-label="Tải lại dữ liệu"
          >
            <RefreshCw className={`w-4 h-4 motion-reduce:animate-none ${loading ? 'animate-spin text-emerald-600' : ''}`} />
          </button>
        </div>
      </div>

      {/* KPI Metrics Header */}
      {plan && (
        <PlanMetricsHeader
          plan={plan}
          onOpenEmergencyBrief={() => setIsEmergencyBriefOpen(true)}
          onOpenAudit={() => setIsAuditOpen(true)}
        />
      )}

      {/* Filter & Search Bar */}
      <div className="p-3 rounded-xl bg-white border border-slate-200 flex flex-wrap items-center gap-3 shadow-xs">
        {/* Search */}
        <div className="relative flex-1 min-w-[200px]">
          <Search className="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
          <input
            type="text"
            placeholder="Tìm theo tiêu đề việc hoặc người phụ trách..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="w-full pl-9 pr-3 py-1.5 text-xs bg-slate-50 border border-slate-300 rounded-lg text-slate-900 placeholder-slate-400 focus:bg-white focus:outline-none focus:border-emerald-500 transition-colors"
          />
        </div>

        {/* Category Filter */}
        <select
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
          className="text-xs bg-slate-50 border border-slate-300 rounded-lg px-3 py-2 text-slate-900 focus:bg-white focus:outline-none focus:border-emerald-500"
        >
          <option value="ALL">Tất cả danh mục</option>
          {allCategories.map((cat) => (
            <option key={cat} value={cat}>
              {cat}
            </option>
          ))}
        </select>

        {/* Gaps Only Toggle */}
        <button
          type="button"
          onClick={() => setOnlyGaps(!onlyGaps)}
          className={`px-3 py-1.5 text-xs font-medium rounded-lg border flex items-center gap-1.5 transition-colors ${onlyGaps
              ? 'bg-amber-50 text-amber-800 border-amber-200'
              : 'bg-slate-50 text-slate-600 border-slate-200 hover:bg-slate-100 hover:text-slate-900'
            }`}
        >
          <AlertTriangle className="w-3.5 h-3.5" />
          <span>Chỉ xem thẻ có lỗ hổng</span>
        </button>
      </div>

      {/* Main Content Area */}
      {loading && !plan ? (
        <div className="py-20 flex flex-col items-center justify-center text-slate-500 gap-3">
          <Loader2 className="w-8 h-8 animate-spin text-emerald-600 motion-reduce:animate-none" />
          <span className="text-sm">Đang tải và tổng hợp Kế Hoạch Tiếp Quản…</span>
        </div>
      ) : error ? (
        <div className="p-4 rounded-xl bg-rose-50 border border-rose-200 text-rose-700 text-xs">
          {error}
        </div>
      ) : (
        <div
          className={
            viewMode === 'kanban'
              ? 'grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 items-start'
              : 'space-y-6'
          }
        >
          {filteredStages.map((stage) => (
            <ContinuityTimelineStage
              key={stage.stage}
              stage={stage}
              onUpdateStage={handleUpdateStage}
              onToggleCompletion={handleToggleCompletion}
            />
          ))}
        </div>
      )}

      {/* Modals */}
      <EmergencyBriefModal
        isOpen={isEmergencyBriefOpen}
        onClose={() => setIsEmergencyBriefOpen(false)}
        fetchBrief={fetchEmergencyBrief}
      />

      <PlanReadinessAuditModal
        isOpen={isAuditOpen}
        onClose={() => setIsAuditOpen(false)}
        fetchAudit={fetchPlanAudit}
      />
    </div>
  );
};
