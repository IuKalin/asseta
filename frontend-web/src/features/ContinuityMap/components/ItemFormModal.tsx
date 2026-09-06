import React, { useState, useEffect } from 'react';
import { AlertCircle, Lock, Shield, X, KeyRound } from 'lucide-react';
import {
  ContinuityCategory,
  ContinuityItem,
  CreateContinuityItemPayload,
  PriorityLevel,
  UpdateContinuityItemPayload,
} from '../../../types/continuity';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  categories: ContinuityCategory[];
  initialCategoryId?: string;
  itemToEdit?: ContinuityItem | null;
  isKeyUnlocked: boolean;
  onUnlockKey: (passphrase: string) => Promise<void>;
  onSubmitCreate: (payload: CreateContinuityItemPayload, plainNotes?: string) => Promise<void>;
  onSubmitUpdate: (id: string, payload: UpdateContinuityItemPayload, plainNotes?: string) => Promise<void>;
  onDecryptExistingNotes?: (item: ContinuityItem) => Promise<string>;
}

export const ItemFormModal: React.FC<Props> = ({
  isOpen,
  onClose,
  categories,
  initialCategoryId,
  itemToEdit,
  isKeyUnlocked,
  onUnlockKey,
  onSubmitCreate,
  onSubmitUpdate,
  onDecryptExistingNotes,
}) => {
  const [categoryId, setCategoryId] = useState<string>('');
  const [name, setName] = useState<string>('');
  const [priority, setPriority] = useState<PriorityLevel>('IMPORTANT');
  const [documentLocationHint, setDocumentLocationHint] = useState<string>('');
  const [plainNotes, setPlainNotes] = useState<string>('');
  const [passphrase, setPassphrase] = useState<string>('');

  const [validationError, setValidationError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [isLoadingExistingNotes, setIsLoadingExistingNotes] = useState<boolean>(false);

  useEffect(() => {
    if (itemToEdit) {
      setCategoryId(itemToEdit.categoryId);
      setName(itemToEdit.name);
      setPriority(itemToEdit.priority);
      setDocumentLocationHint(itemToEdit.documentLocationHint || '');
      setPlainNotes('');

      if (itemToEdit.hasConfidentialNotes && isKeyUnlocked && onDecryptExistingNotes) {
        setIsLoadingExistingNotes(true);
        onDecryptExistingNotes(itemToEdit)
          .then((text) => setPlainNotes(text))
          .catch((err) => console.error('Failed to decrypt notes on edit:', err))
          .finally(() => setIsLoadingExistingNotes(false));
      }
    } else {
      setCategoryId(initialCategoryId || categories[0]?.categoryId || '');
      setName('');
      setPriority('IMPORTANT');
      setDocumentLocationHint('');
      setPlainNotes('');
    }
    setValidationError(null);
  }, [itemToEdit, initialCategoryId, categories, isKeyUnlocked, onDecryptExistingNotes]);

  if (!isOpen) return null;

  const validateInput = (): boolean => {
    if (!name.trim()) {
      setValidationError('Tên hạng mục không được để trống.');
      return false;
    }

    // Client-side regex inspection for credit cards or raw private keys
    const ccPattern = /\b(?:\d[ -]*?){13,19}\b/;
    const pkPattern = /(-----BEGIN (?:RSA |EC )?PRIVATE KEY-----|\b0x[a-fA-F0-9]{64}\b)/;

    if (ccPattern.test(name) || ccPattern.test(documentLocationHint)) {
      setValidationError('Phát hiện định dạng số thẻ tín dụng chưa mã hóa. Tuyệt đối không lưu số thẻ vào tên hoặc vị trí hồ sơ.');
      return false;
    }

    if (pkPattern.test(name) || pkPattern.test(documentLocationHint)) {
      setValidationError('Phát hiện khóa bí mật (Private Key). Tuyệt đối không lưu khóa bí mật bản rõ.');
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateInput()) return;

    try {
      setIsSubmitting(true);
      setValidationError(null);

      // If user typed notes and key is locked, unlock first using Master Key
      if (plainNotes.trim() && !isKeyUnlocked) {
        if (!passphrase.trim()) {
          setValidationError('Vui lòng nhập Master Key của bạn để mã hóa ghi chú bí mật.');
          setIsSubmitting(false);
          return;
        }
        try {
          await onUnlockKey(passphrase);
        } catch (err: any) {
          const msg =
            err.response?.data?.error?.message ||
            err.response?.data?.message ||
            err.message ||
            'Master Key không chính xác. Không thể mã hóa ghi chú.';
          setValidationError(msg);
          setIsSubmitting(false);
          return;
        }
      }

      if (itemToEdit) {
        const payload: UpdateContinuityItemPayload = {
          name: name.trim(),
          priority,
          documentLocationHint: documentLocationHint.trim() || undefined,
          rowVersion: itemToEdit.rowVersion,
        };
        await onSubmitUpdate(itemToEdit.id, payload, plainNotes);
      } else {
        const payload: CreateContinuityItemPayload = {
          categoryId,
          name: name.trim(),
          priority,
          documentLocationHint: documentLocationHint.trim() || undefined,
        };
        await onSubmitCreate(payload, plainNotes);
      }

      onClose();
    } catch (err: any) {
      setValidationError(err.response?.data?.error?.message || err.message || 'Lỗi khi lưu hạng mục');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm animate-in fade-in">
      <div className="bg-white border border-slate-200 rounded-2xl max-w-lg w-full p-6 shadow-2xl space-y-5">
        <div className="flex items-center justify-between border-b border-slate-200 pb-4">
          <div className="flex items-center gap-2">
            <div className="p-1.5 rounded-lg bg-emerald-50 text-emerald-600">
              <Shield className="w-5 h-5" />
            </div>
            <h3 className="text-lg font-bold text-slate-900">
              {itemToEdit ? 'Chỉnh Sửa Hạng Mục Tiếp Quản' : 'Thêm Hạng Mục Tiếp Quản Mới'}
            </h3>
          </div>
          <button
            type="button"
            onClick={onClose}
            aria-label="Đóng"
            className="p-1.5 rounded-lg hover:bg-slate-100 text-slate-400 hover:text-slate-600 transition"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {validationError && (
          <div role="alert" className="p-3 rounded-lg bg-red-50 border border-red-200 text-xs text-red-700 flex items-start gap-2">
            <AlertCircle className="w-4 h-4 shrink-0 mt-0.5 text-red-500" />
            <span>{validationError}</span>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4 text-xs">
          {/* Category Selection (when creating) */}
          {!itemToEdit && (
            <div className="space-y-1.5">
              <label htmlFor="categorySelect" className="text-slate-700 font-medium">Danh mục tài sản / nghĩa vụ *</label>
              <select
                id="categorySelect"
                value={categoryId}
                onChange={(e) => setCategoryId(e.target.value)}
                className="w-full bg-slate-50 border border-slate-300 rounded-lg px-3 py-2 text-slate-900 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2 text-xs"
              >
                {categories.map((c) => (
                  <option key={c.categoryId} value={c.categoryId}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* Item Name */}
          <div className="space-y-1.5">
            <label htmlFor="itemName" className="text-slate-700 font-medium">Tên hạng mục tiếp quản *</label>
            <input
              id="itemName"
              type="text"
              placeholder="VD: Hợp đồng thuê nhà căn 1204 Masteri, Sổ tiết kiệm Agribank…"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full bg-slate-50 border border-slate-300 rounded-lg px-3 py-2 text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2 text-xs"
              required
            />
          </div>

          {/* Priority */}
          <div className="space-y-1.5">
            <label className="text-slate-700 font-medium">Mức độ ưu tiên tiếp quản *</label>
            <div className="grid grid-cols-3 gap-2">
              {(['CRITICAL', 'IMPORTANT', 'LOW'] as PriorityLevel[]).map((p) => (
                <button
                  key={p}
                  type="button"
                  onClick={() => setPriority(p)}
                  className={`py-2 px-3 rounded-lg font-semibold text-center border transition text-xs ${priority === p
                      ? p === 'CRITICAL'
                        ? 'bg-red-50 border-red-300 text-red-700 font-bold'
                        : p === 'IMPORTANT'
                          ? 'bg-amber-50 border-amber-300 text-amber-800 font-bold'
                          : 'bg-sky-50 border-sky-300 text-sky-700 font-bold'
                      : 'bg-slate-50 border-slate-300 text-slate-600 hover:border-slate-400'
                    }`}
                >
                  {p}
                </button>
              ))}
            </div>
          </div>

          {/* Document Location Hint */}
          <div className="space-y-1.5">
            <label htmlFor="documentLocationHint" className="text-slate-700 font-medium">
              Gợi ý vị trí hồ sơ / cách thức truy cập (Không ghi mật khẩu)
            </label>
            <input
              id="documentLocationHint"
              type="text"
              placeholder="VD: Két sắt phòng ngủ ngăn thứ hai, Thư mục Google Drive cá nhân…"
              value={documentLocationHint}
              onChange={(e) => setDocumentLocationHint(e.target.value)}
              className="w-full bg-slate-50 border border-slate-300 rounded-lg px-3 py-2 text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2 text-xs"
            />
          </div>

          {/* Confidential Notes (Zero-Knowledge) */}
          <div className="space-y-1.5 pt-2 border-t border-slate-200">
            <div className="flex items-center justify-between">
              <label htmlFor="plainNotes" className="text-slate-700 font-medium flex items-center gap-1.5">
                <Lock className="w-3.5 h-3.5 text-emerald-600" />
                Ghi chú bí mật cá nhân (Mã hóa đầu cuối AES-256)
              </label>
              <span className="text-[10px] text-emerald-700 font-mono font-semibold">Zero-Knowledge</span>
            </div>
            <p className="text-[11px] text-slate-500">
              Nội dung này được mã hóa bằng Master Key ngay trên trình duyệt trước khi gửi lên máy chủ. Máy chủ Asseta hoàn toàn không thể đọc.
            </p>

            {isLoadingExistingNotes ? (
              <p className="text-slate-500 italic py-2">Đang giải mã ghi chú hiện tại…</p>
            ) : (
              <textarea
                id="plainNotes"
                rows={3}
                placeholder="Nhập ghi chú hướng dẫn riêng biệt cho người tiếp quản…"
                value={plainNotes}
                onChange={(e) => setPlainNotes(e.target.value)}
                className="w-full bg-slate-50 border border-slate-300 rounded-lg px-3 py-2 text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2 text-xs"
              />
            )}

            {/* Master Key prompt if entering notes while locked */}
            {plainNotes.trim() !== '' && !isKeyUnlocked && (
              <div className="p-3.5 rounded-xl bg-amber-50 border border-amber-200 space-y-2">
                <div className="flex items-center gap-2 text-amber-800 text-xs font-semibold">
                  <KeyRound className="w-4 h-4 text-amber-600 shrink-0" />
                  <span>Két đang khóa. Nhập đúng Master Key để mã hóa ghi chú này:</span>
                </div>
                <input
                  id="itemMasterKey"
                  type="text"
                  autoComplete="off"
                  spellCheck={false}
                  placeholder="VD: AK-XXXX-XXXX-XXXX-XXXX"
                  value={passphrase}
                  onChange={(e) => setPassphrase(e.target.value)}
                  className="w-full bg-white border border-amber-300 rounded-lg px-3 py-2 text-slate-900 font-mono text-xs focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
                />
              </div>
            )}
          </div>

          {/* Action buttons */}
          <div className="flex items-center justify-end gap-3 pt-3 border-t border-slate-200">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded-lg bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-medium border border-slate-200 transition"
            >
              Hủy
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-5 py-2 rounded-lg bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold shadow-sm transition disabled:opacity-50"
            >
              {isSubmitting ? 'Đang lưu…' : itemToEdit ? 'Lưu Thay Đổi' : 'Tạo Mới Hạng Mục'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
