import React, { useState } from 'react';
import {
  KeyRound,
  Lock,
  LockOpen,
  Plus,
  RefreshCw,
  Sparkles,
  AlertCircle,
  Loader2,
} from 'lucide-react';
import { ContinuityGap, ContinuityItem } from '../../types/continuity';
import { useContinuityMap } from './hooks/useContinuityMap';
import { useAuth } from '../../contexts/AuthContext';
import { ReadinessScoreOverview } from './components/ReadinessScoreOverview';
import { ContinuityGapBanner } from './components/ContinuityGapBanner';
import { CategoryAccordion } from './components/CategoryAccordion';
import { ItemFormModal } from './components/ItemFormModal';
import { AssessmentWizard } from './components/AssessmentWizard';

export const ContinuityMapPage: React.FC = () => {
  const { user } = useAuth();
  const {
    data,
    isLoading,
    error,
    refetch,
    isKeyUnlocked,
    unlockWithPassphrase,
    lockKey,
    createItem,
    updateItem,
    deleteItem,
    submitAssessment,
    decryptNotes,
  } = useContinuityMap();

  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | undefined>();
  const [itemToEdit, setItemToEdit] = useState<ContinuityItem | null>(null);

  const [isWizardOpen, setIsWizardOpen] = useState<boolean>(false);

  // Master Key unlock prompt modal state
  const [isUnlockModalOpen, setIsUnlockModalOpen] = useState<boolean>(false);
  const [passphraseInput, setPassphraseInput] = useState<string>('');
  const [unlockError, setUnlockError] = useState<string | null>(null);
  const [isVerifying, setIsVerifying] = useState<boolean>(false);

  const handleOpenCreate = (categoryId?: string) => {
    setItemToEdit(null);
    setSelectedCategoryId(categoryId);
    setIsModalOpen(true);
  };

  const handleOpenEdit = (item: ContinuityItem) => {
    setItemToEdit(item);
    setSelectedCategoryId(item.categoryId);
    setIsModalOpen(true);
  };

  const handleUnlockSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!passphraseInput.trim()) {
      setUnlockError('Vui lòng nhập Master Key của bạn.');
      return;
    }
    try {
      setIsVerifying(true);
      setUnlockError(null);
      await unlockWithPassphrase(passphraseInput);
      setPassphraseInput('');
      setIsUnlockModalOpen(false);
    } catch (err: any) {
      const msg =
        err.response?.data?.error?.message ||
        err.response?.data?.message ||
        err.message ||
        'Master Key không chính xác.';
      setUnlockError(msg);
    } finally {
      setIsVerifying(false);
    }
  };

  const handleResolveGap = (gap: ContinuityGap) => {
    // Find item to edit
    if (!data) return;
    for (const cat of data.categories) {
      const found = cat.items.find((i) => i.id === gap.itemId);
      if (found) {
        handleOpenEdit(found);
        return;
      }
    }
  };

  if (isLoading && !data) {
    return (
      <div className="flex flex-col items-center justify-center py-20 text-slate-500 space-y-3">
        <RefreshCw className="w-8 h-8 animate-spin text-emerald-600" />
        <p className="text-sm font-medium">Đang tải Bản Đồ Kế Thừa & Tiếp Quản…</p>
      </div>
    );
  }

  if (error && !data) {
    return (
      <div className="rounded-2xl bg-red-50 border border-red-200 p-6 text-center space-y-4 max-w-lg mx-auto my-12 shadow-sm">
        <p className="text-sm text-red-700">{error}</p>
        <button
          onClick={() => refetch()}
          className="px-4 py-2 rounded-xl bg-red-600 hover:bg-red-500 text-white text-xs font-semibold transition shadow-sm"
        >
          Thử Lại
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-7xl mx-auto pb-12">
      {/* Top Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-2.5">
            <h2 className="text-2xl font-bold text-[#222222] tracking-tight">Continuity Map</h2>
            <span className="text-[10px] font-bold uppercase tracking-wider px-2.5 py-0.5 rounded-full bg-rose-50 text-[#FF385C] border border-rose-200 shadow-2xs">
              Zero-Knowledge Vault
            </span>
          </div>
          <p className="text-xs text-[#717171] mt-1">
            Bản đồ tài sản & trách nhiệm bảo vệ toàn vẹn gia đình theo tiêu chuẩn SDD Module 1.
          </p>
        </div>

        <div className="flex items-center gap-3">
          {/* Key lock/unlock button */}
          {isKeyUnlocked ? (
            <button
              onClick={lockKey}
              className="flex items-center gap-1.5 px-3 py-2 rounded-xl bg-emerald-50 hover:bg-emerald-100 text-emerald-700 border border-emerald-200 text-xs font-medium transition cursor-pointer shadow-2xs"
              title="Khóa Master Key"
              aria-label="Khóa Master Key"
            >
              <LockOpen className="w-3.5 h-3.5" />
              <span>Master Key: Đang mở</span>
            </button>
          ) : (
            <button
              onClick={() => setIsUnlockModalOpen(true)}
              className="flex items-center gap-1.5 px-3 py-2 rounded-xl bg-white hover:bg-[#F7F7F7] text-[#222222] border border-[#DDDDDD] text-xs font-medium transition cursor-pointer shadow-2xs"
              aria-label="Mở khóa ghi chú bí mật"
            >
              <Lock className="w-3.5 h-3.5 text-[#FF385C]" />
              <span>Mở khóa ghi chú bí mật</span>
            </button>
          )}

          <button
            onClick={() => setIsWizardOpen(true)}
            className="flex items-center gap-1.5 px-3.5 py-2 rounded-xl bg-white hover:bg-[#F7F7F7] text-[#222222] text-xs font-semibold transition border border-[#DDDDDD] cursor-pointer shadow-2xs"
          >
            <Sparkles className="w-3.5 h-3.5 text-[#FF385C]" />
            <span>Khảo sát nhanh</span>
          </button>

          <button
            onClick={() => handleOpenCreate()}
            className="flex items-center gap-2 px-4 py-2.5 rounded-xl bg-[#FF385C] hover:bg-[#E00B41] active:scale-[0.98] text-white font-semibold text-xs transition shadow-xs cursor-pointer"
          >
            <Plus className="w-4 h-4" />
            <span>Thêm Hạng Mục</span>
          </button>
        </div>
      </div>

      {/* Readiness Score Overview */}
      {data && (
        <ReadinessScoreOverview
          overallScore={data.overallReadinessScore}
          totalItems={data.totalItems}
          totalGaps={data.totalGaps}
          categories={data.categories}
          onOpenAssessment={() => setIsWizardOpen(true)}
        />
      )}

      {/* Gap Warning Banner */}
      {data && data.gaps.length > 0 && (
        <ContinuityGapBanner gaps={data.gaps} onResolveGap={handleResolveGap} />
      )}

      {/* Categories Accordions List */}
      <div className="space-y-4">
        {data?.categories.map((category) => (
          <CategoryAccordion
            key={category.categoryId}
            category={category}
            isKeyUnlocked={isKeyUnlocked}
            onAddItem={handleOpenCreate}
            onEditItem={handleOpenEdit}
            onDeleteItem={deleteItem}
            onDecryptNotes={decryptNotes}
            onPromptUnlock={() => setIsUnlockModalOpen(true)}
          />
        ))}
      </div>

      {/* Item Form Modal */}
      {data && (
        <ItemFormModal
          isOpen={isModalOpen}
          onClose={() => setIsModalOpen(false)}
          categories={data.categories}
          initialCategoryId={selectedCategoryId}
          itemToEdit={itemToEdit}
          isKeyUnlocked={isKeyUnlocked}
          onUnlockKey={unlockWithPassphrase}
          onSubmitCreate={async (payload, notes) => {
            await createItem(payload, notes);
          }}
          onSubmitUpdate={async (id, payload, notes) => {
            await updateItem(id, payload, notes);
          }}
          onDecryptExistingNotes={decryptNotes}
        />
      )}

      {/* Assessment Wizard Modal */}
      <AssessmentWizard
        isOpen={isWizardOpen}
        onClose={() => setIsWizardOpen(false)}
        onSubmit={async (answers) => {
          await submitAssessment(answers);
        }}
      />

      {/* Master Key Unlock Modal */}
      {isUnlockModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-xs animate-in fade-in">
          <div className="bg-white border border-[#EBEBEB] rounded-3xl max-w-md w-full p-6 sm:p-8 shadow-[0_10px_30px_rgba(0,0,0,0.1)] space-y-5">
            <div className="flex items-center gap-3 text-[#222222]">
              <div className="p-2.5 rounded-2xl bg-rose-50 text-[#FF385C] border border-rose-100">
                <KeyRound className="w-5 h-5" />
              </div>
              <div>
                <h3 className="text-base font-bold text-[#222222]">Mở Khóa Két (Master Key)</h3>
                <p className="text-[11px] text-[#717171]">
                  Bắt buộc nhập chính xác Master Key đã cấp để giải mã ghi chú bí mật
                </p>
              </div>
            </div>

            {/* Quick-fill hint for Demo account */}
            {user?.email === 'globalhelcurt14092005@gmail.com' && (
              <div className="p-3 rounded-2xl bg-rose-50/70 border border-rose-100 text-xs text-[#222222] flex items-center justify-between gap-2">
                <div>
                  <span className="text-[11px] text-[#717171] block">Tài khoản Demo có sẵn khóa:</span>
                  <code className="font-mono font-bold text-[#FF385C] select-all">AK-DEMO-2026-ASSETA-VAULT</code>
                </div>
                <button
                  type="button"
                  onClick={() => setPassphraseInput('AK-DEMO-2026-ASSETA-VAULT')}
                  className="px-2.5 py-1 rounded-xl bg-white hover:bg-rose-100/60 text-[#FF385C] border border-rose-200 text-[11px] font-semibold transition shrink-0 cursor-pointer shadow-2xs"
                >
                  Điền nhanh
                </button>
              </div>
            )}

            {unlockError && (
              <div role="alert" className="text-xs text-red-700 bg-red-50 p-3 rounded-2xl border border-red-200 flex items-start gap-2">
                <AlertCircle className="w-4 h-4 text-red-500 shrink-0 mt-0.5" />
                <span>{unlockError}</span>
              </div>
            )}

            <form onSubmit={handleUnlockSubmit} className="space-y-4">
              <div>
                <label htmlFor="passphraseInput" className="block text-xs font-semibold text-[#222222] mb-1.5">
                  Master Key cá nhân (AK-XXXX-XXXX-XXXX-XXXX)
                </label>
                <input
                  id="passphraseInput"
                  type="text"
                  placeholder="VD: AK-XXXX-XXXX-XXXX-XXXX"
                  value={passphraseInput}
                  onChange={(e) => setPassphraseInput(e.target.value)}
                  className="w-full bg-[#F7F7F7] border border-[#DDDDDD] rounded-xl px-3.5 py-2.5 text-[#222222] font-mono text-xs tracking-wider placeholder:tracking-normal placeholder-[#717171]/60 focus:outline-none focus:bg-white focus:border-[#222222] focus-visible:ring-1 focus-visible:ring-[#222222]"
                  autoFocus
                />
              </div>

              <div className="flex justify-end gap-2 pt-1">
                <button
                  type="button"
                  onClick={() => {
                    setIsUnlockModalOpen(false);
                    setUnlockError(null);
                    setPassphraseInput('');
                  }}
                  className="px-4 py-2 rounded-xl bg-white text-[#222222] border border-[#DDDDDD] text-xs font-semibold hover:bg-[#F7F7F7] transition cursor-pointer"
                >
                  Đóng
                </button>
                <button
                  type="submit"
                  disabled={isVerifying}
                  className="px-4 py-2 rounded-xl bg-[#FF385C] hover:bg-[#E00B41] active:scale-[0.98] text-white text-xs font-semibold shadow-xs transition disabled:opacity-50 flex items-center gap-1.5 cursor-pointer"
                >
                  {isVerifying ? (
                    <>
                      <Loader2 className="w-3.5 h-3.5 animate-spin" />
                      <span>Đang xác thực khóa…</span>
                    </>
                  ) : (
                    <span>Xác Nhận Mở Khóa</span>
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
