import React, { useState, useEffect } from 'react';
import {
  ActionCard,
  ActionCardTemplate,
  CardPriority,
  CreateActionCardInput,
  UpdateActionCardInput,
  UrgencyStage,
} from '../../../types/actionCard';
import { CryptoService } from '../../../services/cryptoService';
import { useAuth } from '../../../contexts/AuthContext';
import { KeyRound, ShieldCheck, Lock, Sparkles, X } from 'lucide-react';
import { getErrorMessage } from '../../../utils/errorUtils';

interface ActionCardFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmitCreate: (data: CreateActionCardInput) => Promise<void>;
  onSubmitUpdate: (id: string, data: UpdateActionCardInput) => Promise<void>;
  initialStage?: UrgencyStage;
  cardToEdit?: ActionCard | null;
  appliedTemplate?: ActionCardTemplate | null;
  onOpenTemplateSelector?: () => void;
  categories?: { id: string; code: string; name: string }[];
}

const DEFAULT_CATEGORIES = [
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e01', code: 'FINANCIAL', name: 'Tài chính & Nghĩa vụ tiền tệ' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e02', code: 'PROPERTY', name: 'Tài sản & Bất động sản' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e03', code: 'INSURANCE', name: 'Bảo hiểm & Quyền lợi sức khỏe' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e04', code: 'BUSINESS', name: 'Doanh nghiệp & Quan hệ đối tác' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e05', code: 'DOCUMENTS', name: 'Hồ sơ & Giấy tờ pháp lý' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e06', code: 'FAMILY', name: 'Gia đình & Nghĩa vụ cá nhân' },
];

export const ActionCardFormModal: React.FC<ActionCardFormModalProps> = ({
  isOpen,
  onClose,
  onSubmitCreate,
  onSubmitUpdate,
  initialStage = 'FIRST_72_HOURS',
  cardToEdit,
  appliedTemplate,
  onOpenTemplateSelector,
  categories = DEFAULT_CATEGORIES,
}) => {
  const { isVaultUnlocked, vaultCryptoKey, unlockVault, user } = useAuth();

  const [title, setTitle] = useState('');
  const [categoryId, setCategoryId] = useState(
    categories[0]?.id || DEFAULT_CATEGORIES[0].id
  );
  const [urgency, setUrgency] = useState<UrgencyStage>(initialStage);
  const [priority, setPriority] = useState<CardPriority>('CRITICAL');
  const [summary, setSummary] = useState('');
  const [documentLocationHint, setDocumentLocationHint] = useState('');
  const [digitalStorageLink, setDigitalStorageLink] = useState('');

  // Confidential notes (Client-Side Encrypted)
  const [confidentialInstructions, setConfidentialInstructions] = useState('');
  const [passphrase, setPassphrase] = useState('');

  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (cardToEdit) {
      setTitle(cardToEdit.title);
      setCategoryId(cardToEdit.categoryId);
      setUrgency(cardToEdit.urgency);
      setPriority(cardToEdit.priority);
      setSummary(cardToEdit.summary || '');
      setDocumentLocationHint(cardToEdit.documentLocationHint || '');
      setDigitalStorageLink(cardToEdit.digitalStorageLink || '');
      setConfidentialInstructions('');
      setPassphrase('');
    } else if (appliedTemplate) {
      setTitle(appliedTemplate.titleVi);
      const cat = categories.find((c) => c.code === appliedTemplate.categoryCode);
      if (cat) setCategoryId(cat.id);
      setUrgency(appliedTemplate.defaultUrgency);
      setPriority(appliedTemplate.defaultPriority);
      setSummary(`Tạo từ bản mẫu: ${appliedTemplate.titleVi}`);
    } else {
      setTitle('');
      setCategoryId(categories[0]?.id || '');
      setUrgency(initialStage);
      setPriority('CRITICAL');
      setSummary('');
      setDocumentLocationHint('');
      setDigitalStorageLink('');
      setConfidentialInstructions('');
      setPassphrase('');
    }
  }, [cardToEdit, appliedTemplate, initialStage, categories]);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim()) {
      setError('Vui lòng nhập tiêu đề thẻ hành động.');
      return;
    }

    // Client-side Zero-Knowledge Check for Plaintext Credit Cards
    const ccRegex = /\b(?:\d[ -]*?){13,19}\b/;
    if (ccRegex.test(title) || ccRegex.test(summary) || ccRegex.test(documentLocationHint)) {
      setError('Phát hiện số thẻ trong văn bản chưa mã hóa! Vui lòng chỉ nhập thông tin nhạy cảm vào ô "Chỉ dẫn bảo mật" và nhập Passphrase.');
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      let cipherInstructionsBlob: string | undefined;
      let cipherNonce: string | undefined;
      let cipherAuthTag: string | undefined;

      if (confidentialInstructions.trim()) {
        let activeKey = vaultCryptoKey;
        if (!isVaultUnlocked || !activeKey) {
          if (!passphrase.trim()) {
            setError('Két đang khóa. Vui lòng nhập Master Key của bạn để mã hóa chỉ dẫn bảo mật.');
            setIsSubmitting(false);
            return;
          }
          try {
            await unlockVault(passphrase);
            const saltHex = user?.encryptionSalt || 'a1b2c3d4e5f60718293a4b5c6d7e8f90';
            const saltBytes = CryptoService.hexToBytes(saltHex);
            activeKey = await CryptoService.deriveMasterKey(passphrase.trim().toUpperCase(), saltBytes);
          } catch (err: any) {
            const msg =
              err.response?.data?.error?.message ||
              err.response?.data?.message ||
              err.message ||
              'Master Key không chính xác. Không thể mã hóa chỉ dẫn bảo mật.';
            setError(msg);
            setIsSubmitting(false);
            return;
          }
        }

        const encrypted = await CryptoService.encrypt(activeKey, confidentialInstructions.trim());
        cipherInstructionsBlob = encrypted.cipherNotesBlob;
        cipherNonce = encrypted.cipherNonce;
        cipherAuthTag = encrypted.cipherAuthTag;
      }

      if (cardToEdit) {
        await onSubmitUpdate(cardToEdit.id, {
          title: title.trim(),
          urgency,
          priority,
          rowVersion: cardToEdit.rowVersion,
          summary: summary.trim() || undefined,
          documentLocationHint: documentLocationHint.trim() || undefined,
          digitalStorageLink: digitalStorageLink.trim() || undefined,
          cipherInstructionsBlob,
          cipherNonce,
          cipherAuthTag,
        });
      } else {
        await onSubmitCreate({
          categoryId,
          title: title.trim(),
          urgency,
          priority,
          summary: summary.trim() || undefined,
          documentLocationHint: documentLocationHint.trim() || undefined,
          digitalStorageLink: digitalStorageLink.trim() || undefined,
          cipherInstructionsBlob,
          cipherNonce,
          cipherAuthTag,
        });
      }
      onClose();
    } catch (err: any) {
      setError(getErrorMessage(err, 'Lỗi khi lưu thẻ hành động.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="flex max-h-[90vh] w-full max-w-2xl flex-col rounded-2xl border border-slate-200 bg-white shadow-2xl overflow-hidden">
        {/* Header */}
        <div className="flex items-center justify-between border-b border-slate-200 p-5">
          <div className="flex items-center gap-3">
            <div>
              <h3 className="text-lg font-bold text-slate-900">
                {cardToEdit ? 'Chỉnh Sửa Thẻ Hành Động' : 'Tạo Mới Thẻ Hành Động'}
              </h3>
              <p className="text-xs text-slate-500">
                {cardToEdit
                  ? 'Cập nhật nội dung và hướng dẫn xử lý'
                  : 'Thiết lập chỉ dẫn khẩn cấp bảo đảm tính liên tục'}
              </p>
            </div>
          </div>
          <div className="flex items-center gap-2">
            {!cardToEdit && onOpenTemplateSelector && (
              <button
                type="button"
                onClick={onOpenTemplateSelector}
                className="flex items-center gap-1.5 rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-1.5 text-xs font-semibold text-emerald-700 hover:bg-emerald-100 transition"
              >
                <Sparkles className="w-3.5 h-3.5 text-emerald-600" />
                <span>Chọn Bản Mẫu</span>
              </button>
            )}
            <button
              type="button"
              onClick={onClose}
              aria-label="Đóng"
              className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-100 hover:text-slate-600 transition"
            >
              <X className="w-5 h-5" />
            </button>
          </div>
        </div>

        {/* Form Body */}
        <form onSubmit={handleSubmit} className="flex-1 space-y-4 overflow-y-auto p-6">
          {error && (
            <div role="alert" className="rounded-lg border border-red-200 bg-red-50 p-3 text-xs text-red-700">
              {error}
            </div>
          )}

          {/* Title */}
          <div>
            <label htmlFor="cardTitle" className="block text-xs font-semibold text-slate-700">
              Tiêu đề Thẻ Hành Động <span className="text-red-500">*</span>
            </label>
            <input
              id="cardTitle"
              type="text"
              required
              maxLength={200}
              placeholder="VD: Xử lý khoản vay thế chấp ngân hàng"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 px-3.5 py-2 text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
            />
          </div>

          {/* Category & Urgency */}
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            {!cardToEdit && (
              <div>
                <label htmlFor="cardCategory" className="block text-xs font-semibold text-slate-700">Danh mục tài sản</label>
                <select
                  id="cardCategory"
                  value={categoryId}
                  onChange={(e) => setCategoryId(e.target.value)}
                  className="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 px-3 py-2 text-sm text-slate-900 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
                >
                  {categories.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </div>
            )}

            <div>
              <label htmlFor="cardUrgency" className="block text-xs font-semibold text-slate-700">Giai đoạn khẩn cấp</label>
              <select
                id="cardUrgency"
                value={urgency}
                onChange={(e) => setUrgency(e.target.value as UrgencyStage)}
                className="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 px-3 py-2 text-sm text-slate-900 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
              >
                <option value="IMMEDIATE">⚡ Ngay Lập Tức (0 - 24 giờ)</option>
                <option value="FIRST_72_HOURS">⏳ 72 Giờ Đầu Tiên (Ngày 1 - 3)</option>
                <option value="FIRST_7_DAYS">📅 7 Ngày Đầu (Tuần 1)</option>
                <option value="LONGER_TERM">🛡️ Dài Hạn (&gt; 7 ngày)</option>
              </select>
            </div>

            <div>
              <label htmlFor="cardPriority" className="block text-xs font-semibold text-slate-700">Mức độ ưu tiên</label>
              <select
                id="cardPriority"
                value={priority}
                onChange={(e) => setPriority(e.target.value as CardPriority)}
                className="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 px-3 py-2 text-sm text-slate-900 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
              >
                <option value="CRITICAL">🔴 CRITICAL (Cốt tử)</option>
                <option value="IMPORTANT">🟡 IMPORTANT (Quan trọng)</option>
                <option value="LOW">🔵 LOW (Thứ yếu)</option>
              </select>
            </div>
          </div>

          {/* Summary */}
          <div>
            <label htmlFor="cardSummary" className="block text-xs font-semibold text-slate-700">Tóm tắt bối cảnh / Mục đích</label>
            <textarea
              id="cardSummary"
              rows={2}
              maxLength={1000}
              placeholder="Mô tả tóm tắt hoàn cảnh và mục tiêu xử lý của thẻ này…"
              value={summary}
              onChange={(e) => setSummary(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 px-3.5 py-2 text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
            />
          </div>

          {/* Location Hint & Cloud Link */}
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div>
              <label htmlFor="cardLocationHint" className="block text-xs font-semibold text-slate-700">Vị trí tài liệu vật lý</label>
              <input
                id="cardLocationHint"
                type="text"
                maxLength={255}
                placeholder="VD: Két sắt phòng làm việc, ngăn dưới"
                value={documentLocationHint}
                onChange={(e) => setDocumentLocationHint(e.target.value)}
                className="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 px-3.5 py-2 text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
              />
            </div>
            <div>
              <label htmlFor="cardDigitalLink" className="block text-xs font-semibold text-slate-700">Liên kết số / Cloud Link</label>
              <input
                id="cardDigitalLink"
                type="url"
                maxLength={500}
                placeholder="VD: https://drive.google.com/…"
                value={digitalStorageLink}
                onChange={(e) => setDigitalStorageLink(e.target.value)}
                className="mt-1 w-full rounded-lg border border-slate-300 bg-slate-50 px-3.5 py-2 text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
              />
            </div>
          </div>

          {/* Zero-Knowledge Confidential Instructions Box */}
          <div className="rounded-xl border border-purple-200 bg-purple-50 p-4 space-y-3">
            <div className="flex items-center gap-2">
              <Lock className="w-4 h-4 text-purple-600" />
              <div>
                <h4 className="text-xs font-bold text-purple-900 uppercase tracking-wider">
                  Chỉ dẫn bảo mật (Mã hóa Zero-Knowledge)
                </h4>
                <p className="text-[11px] text-purple-700">
                  Dữ liệu này được mã hóa AES-256-GCM ngay trên trình duyệt trước khi gửi đi.
                </p>
              </div>
            </div>

            <textarea
              id="confidentialInstructions"
              rows={2}
              placeholder="Nhập mã pin két, chỉ dẫn mở khóa, mật mã tài khoản dự phòng…"
              value={confidentialInstructions}
              onChange={(e) => setConfidentialInstructions(e.target.value)}
              className="w-full rounded-lg border border-purple-300 bg-white px-3.5 py-2 text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-purple-500 focus-visible:ring-offset-2 font-mono text-xs"
            />

            {confidentialInstructions && (
              <div>
                {isVaultUnlocked ? (
                  <div className="flex items-center gap-2 p-2.5 rounded-lg bg-emerald-50 border border-emerald-200 text-emerald-700 text-xs">
                    <ShieldCheck className="w-4 h-4 text-emerald-600 shrink-0" />
                    <span>Két đã mở khóa (Mã hóa an toàn bằng Master Key của bạn)</span>
                  </div>
                ) : (
                  <div className="space-y-1.5">
                    <label htmlFor="actionCardMasterKey" className="flex items-center gap-1.5 text-xs font-semibold text-purple-900">
                      <KeyRound className="w-3.5 h-3.5 text-amber-600" />
                      Master Key để mã hóa <span className="text-red-500">*</span>
                    </label>
                    <input
                      id="actionCardMasterKey"
                      type="text"
                      autoComplete="off"
                      spellCheck={false}
                      placeholder="VD: AK-XXXX-XXXX-XXXX-XXXX"
                      value={passphrase}
                      onChange={(e) => setPassphrase(e.target.value)}
                      className="mt-1 w-full rounded-lg border border-purple-300 bg-white px-3.5 py-1.5 text-xs font-mono text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-purple-500 focus-visible:ring-offset-2"
                    />
                    <p className="text-[11px] text-slate-500">
                      Bắt buộc nhập chính xác Master Key đã cấp khi đăng ký tài khoản.
                    </p>
                  </div>
                )}
              </div>
            )}
          </div>

          {/* Footer Submit */}
          <div className="flex items-center justify-end gap-3 border-t border-slate-200 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="rounded-lg border border-slate-200 bg-slate-100 px-4 py-2 text-xs font-medium text-slate-700 hover:bg-slate-200 transition"
            >
              Hủy
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="rounded-lg bg-emerald-600 px-5 py-2 text-xs font-semibold text-white shadow-sm hover:bg-emerald-500 transition disabled:opacity-50"
            >
              {isSubmitting ? 'Đang lưu…' : cardToEdit ? 'Lưu thay đổi' : 'Tạo thẻ hành động'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
