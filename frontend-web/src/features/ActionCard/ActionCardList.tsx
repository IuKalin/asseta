import React, { useState } from 'react';
import { Layers, Search, Plus, LayoutGrid, List, Sparkles, Zap, Clock, Calendar, ShieldCheck } from 'lucide-react';
import { ActionCard, ActionCardTemplate, UrgencyStage } from '../../types/actionCard';
import { useActionCards, useActionCardDetail, useActionCardTemplates } from './hooks/useActionCards';
import { ActionCardTimelineView } from './components/ActionCardTimelineView';
import { ActionCardItemCard } from './components/ActionCardItemCard';
import { ActionCardDetailModal } from './components/ActionCardDetailModal';
import { ActionCardFormModal } from './components/ActionCardFormModal';
import { TemplateSelectorModal } from './components/TemplateSelectorModal';
import { useContinuityMap } from '../ContinuityMap/hooks/useContinuityMap';

export const ActionCardList: React.FC = () => {
  const [viewMode, setViewMode] = useState<'timeline' | 'grid'>('timeline');
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedUrgency, setSelectedUrgency] = useState<UrgencyStage | 'ALL'>('ALL');

  // Modal states
  const [selectedCardId, setSelectedCardId] = useState<string | null>(null);
  const [isDetailOpen, setIsDetailOpen] = useState(false);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [isTemplateSelectorOpen, setIsTemplateSelectorOpen] = useState(false);
  const [cardToEdit, setCardToEdit] = useState<ActionCard | null>(null);
  const [appliedTemplate, setAppliedTemplate] = useState<ActionCardTemplate | null>(null);
  const [formInitialStage, setFormInitialStage] = useState<UrgencyStage>('FIRST_72_HOURS');

  // Data hooks
  const {
    cards,
    loading,
    error,
    createCard,
    updateCard,
    deleteCard,
    fetchCards,
  } = useActionCards({
    urgency: selectedUrgency === 'ALL' ? undefined : selectedUrgency,
    search: searchQuery || undefined,
  });

  const {
    card: activeCard,
    toggleStep,
    addStep,
    deleteStep,
    reorderSteps,
    addContact,
    deleteContact,
  } = useActionCardDetail(selectedCardId);

  const { templates, loading: templatesLoading } = useActionCardTemplates();
  const { data: continuityData } = useContinuityMap();

  const dynamicCategories = continuityData?.categories?.map((c) => ({
    id: c.categoryId,
    code: c.code,
    name: c.name,
  }));

  // Handlers
  const handleOpenCardDetail = (card: ActionCard) => {
    setSelectedCardId(card.id);
    setIsDetailOpen(true);
  };

  const handleCloseDetail = () => {
    setIsDetailOpen(false);
    setSelectedCardId(null);
    fetchCards();
  };

  const handleOpenCreateModal = (stage?: UrgencyStage) => {
    setCardToEdit(null);
    setAppliedTemplate(null);
    setFormInitialStage(stage || 'FIRST_72_HOURS');
    setIsFormOpen(true);
  };

  const handleEditCard = (card: ActionCard, e?: React.MouseEvent) => {
    e?.stopPropagation();
    setCardToEdit(card);
    setAppliedTemplate(null);
    setIsFormOpen(true);
  };

  const handleDeleteCard = async (card: ActionCard, e?: React.MouseEvent) => {
    e?.stopPropagation();
    if (window.confirm(`Bạn có chắc muốn xóa thẻ hành động "${card.title}" không?`)) {
      await deleteCard(card.id);
      if (selectedCardId === card.id) {
        setIsDetailOpen(false);
      }
    }
  };

  const handleSelectTemplate = (template: ActionCardTemplate) => {
    setAppliedTemplate(template);
    setCardToEdit(null);
    setIsFormOpen(true);
  };

  return (
    <div className="space-y-6">
      {/* Top Header */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <div className="flex items-center gap-2">
            <h2 className="text-2xl font-bold tracking-tight text-slate-900">Action Cards (Kịch Bản Hành Động)</h2>
            <span className="rounded-full bg-emerald-50 border border-emerald-200 px-2.5 py-0.5 text-xs font-semibold text-emerald-700">
              Module 2
            </span>
          </div>
          <p className="mt-1 text-sm text-slate-500">
            Kịch bản xử lý khẩn cấp từng bước được phân bổ theo 4 giai đoạn thời gian sống còn.
          </p>
        </div>

        <div className="flex items-center gap-3">
          <button
            type="button"
            onClick={() => setIsTemplateSelectorOpen(true)}
            className="flex items-center gap-1.5 rounded-xl border border-emerald-200 bg-emerald-50 px-3.5 py-2 text-xs font-semibold text-emerald-700 hover:bg-emerald-100 transition"
          >
            <Sparkles className="w-3.5 h-3.5 text-emerald-600" />
            <span>Bản Mẫu Khẩn Cấp</span>
          </button>
          <button
            type="button"
            onClick={() => handleOpenCreateModal()}
            className="flex items-center gap-1.5 rounded-xl bg-emerald-600 px-4 py-2 text-xs font-semibold text-white shadow-sm hover:bg-emerald-500 transition"
          >
            <Plus className="h-4 w-4" />
            <span>Tạo Thẻ Mới</span>
          </button>
        </div>
      </div>

      {/* Control Bar: Search, Filters, View Modes */}
      <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-center sm:justify-between">
        {/* Search */}
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
          <input
            type="text"
            placeholder="Tìm kiếm thẻ, tóm tắt, vị trí hồ sơ…"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="w-full rounded-xl border border-slate-300 bg-slate-50 py-2 pl-9 pr-4 text-xs text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
          />
        </div>

        {/* Urgency Filter Pills & View mode switch */}
        <div className="flex flex-wrap items-center gap-2">
          <div className="flex items-center rounded-xl bg-slate-100 p-1 border border-slate-200">
            {(['ALL', 'IMMEDIATE', 'FIRST_72_HOURS', 'FIRST_7_DAYS', 'LONGER_TERM'] as const).map((stage) => {
              const renderLabel = () => {
                switch (stage) {
                  case 'ALL':
                    return 'Tất cả';
                  case 'IMMEDIATE':
                    return <span className="inline-flex items-center gap-1"><Zap className="w-3 h-3 text-amber-500" /> Ngay lập tức</span>;
                  case 'FIRST_72_HOURS':
                    return <span className="inline-flex items-center gap-1"><Clock className="w-3 h-3 text-rose-500" /> 72 Giờ</span>;
                  case 'FIRST_7_DAYS':
                    return <span className="inline-flex items-center gap-1"><Calendar className="w-3 h-3 text-blue-500" /> 7 Ngày</span>;
                  default:
                    return <span className="inline-flex items-center gap-1"><ShieldCheck className="w-3 h-3 text-emerald-600" /> Dài hạn</span>;
                }
              };

              return (
                <button
                  key={stage}
                  type="button"
                  onClick={() => setSelectedUrgency(stage)}
                  className={`rounded-lg px-2.5 py-1 text-xs font-medium transition ${selectedUrgency === stage
                      ? 'bg-white text-emerald-700 font-bold shadow-sm'
                      : 'text-slate-600 hover:text-slate-900'
                    }`}
                >
                  {renderLabel()}
                </button>
              );
            })}
          </div>

          <div className="flex items-center rounded-xl bg-slate-100 p-1 border border-slate-200">
            <button
              type="button"
              onClick={() => setViewMode('timeline')}
              aria-label="Xem dạng 4 cột Timeline"
              className={`rounded-lg p-1.5 transition ${viewMode === 'timeline' ? 'bg-white text-emerald-700 shadow-sm' : 'text-slate-500 hover:text-slate-900'
                }`}
              title="Xem dạng 4 cột Timeline"
            >
              <LayoutGrid className="h-4 w-4" />
            </button>
            <button
              type="button"
              onClick={() => setViewMode('grid')}
              aria-label="Xem dạng Lưới phẳng"
              className={`rounded-lg p-1.5 transition ${viewMode === 'grid' ? 'bg-white text-emerald-700 shadow-sm' : 'text-slate-500 hover:text-slate-900'
                }`}
              title="Xem dạng Lưới phẳng"
            >
              <List className="h-4 w-4" />
            </button>
          </div>
        </div>
      </div>

      {/* Error state */}
      {error && (
        <div role="alert" className="rounded-xl border border-red-200 bg-red-50 p-4 text-xs text-red-700">
          {error}
        </div>
      )}

      {/* Main Content Area */}
      {loading ? (
        <div className="flex h-64 items-center justify-center rounded-2xl border border-slate-200 bg-white">
          <div className="text-center text-sm text-slate-500">Đang tải thẻ hành động…</div>
        </div>
      ) : cards.length === 0 ? (
        <div className="flex flex-col items-center justify-center rounded-2xl border border-dashed border-slate-200 bg-slate-50 p-12 text-center">
          <Layers className="h-12 w-12 text-slate-400 mb-3" />
          <h3 className="text-base font-semibold text-slate-800">Chưa có thẻ hành động nào</h3>
          <p className="mt-1 max-w-md text-xs text-slate-500">
            Tạo thẻ hành động mới hoặc chọn từ 6 bản mẫu chuẩn hóa để hướng dẫn người thân và đối tác khi có biến cố.
          </p>
          <div className="mt-4 flex gap-3">
            <button
              type="button"
              onClick={() => setIsTemplateSelectorOpen(true)}
              className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-2 text-xs font-semibold text-emerald-700 hover:bg-emerald-100"
            >
              Chọn Bản Mẫu Có Sẵn
            </button>
            <button
              type="button"
              onClick={() => handleOpenCreateModal()}
              className="rounded-xl bg-emerald-600 px-4 py-2 text-xs font-semibold text-white hover:bg-emerald-500 shadow-sm"
            >
              Tạo Thẻ Mới
            </button>
          </div>
        </div>
      ) : viewMode === 'timeline' ? (
        <ActionCardTimelineView
          cards={cards}
          onSelectCard={handleOpenCardDetail}
          onEditCard={handleEditCard}
          onDeleteCard={handleDeleteCard}
          onAddCardToStage={(stage) => handleOpenCreateModal(stage)}
        />
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {cards.map((card) => (
            <ActionCardItemCard
              key={card.id}
              card={card}
              onClick={handleOpenCardDetail}
              onEdit={handleEditCard}
              onDelete={handleDeleteCard}
            />
          ))}
        </div>
      )}

      {/* Card Detail Modal */}
      {activeCard && isDetailOpen && (
        <ActionCardDetailModal
          card={activeCard}
          isOpen={isDetailOpen}
          onClose={handleCloseDetail}
          onToggleStep={toggleStep}
          onAddStep={addStep}
          onDeleteStep={deleteStep}
          onReorderSteps={reorderSteps}
          onAddContact={addContact}
          onDeleteContact={deleteContact}
          onEdit={(c) => {
            setIsDetailOpen(false);
            handleEditCard(c);
          }}
        />
      )}

      {/* Create / Edit Form Modal */}
      {isFormOpen && (
        <ActionCardFormModal
          isOpen={isFormOpen}
          onClose={() => {
            setIsFormOpen(false);
            setCardToEdit(null);
            setAppliedTemplate(null);
          }}
          onSubmitCreate={async (data) => {
            await createCard(data);
          }}
          onSubmitUpdate={async (id, data) => {
            await updateCard(id, data);
          }}
          initialStage={formInitialStage}
          cardToEdit={cardToEdit}
          appliedTemplate={appliedTemplate}
          onOpenTemplateSelector={() => setIsTemplateSelectorOpen(true)}
          categories={dynamicCategories && dynamicCategories.length > 0 ? dynamicCategories : undefined}
        />
      )}

      {/* Template Selector Modal */}
      {isTemplateSelectorOpen && (
        <TemplateSelectorModal
          isOpen={isTemplateSelectorOpen}
          templates={templates}
          loading={templatesLoading}
          onSelect={handleSelectTemplate}
          onClose={() => setIsTemplateSelectorOpen(false)}
        />
      )}
    </div>
  );
};
