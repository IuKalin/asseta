import React, { useState, useEffect } from 'react';
import { ActionCard, AddContactInput } from '../../../types/actionCard';
import { StepChecklistEditor } from './StepChecklistEditor';
import { CryptoService } from '../../../services/cryptoService';
import { useAuth } from '../../../contexts/AuthContext';
import { KeyRound, ShieldCheck, Lock, Phone, Mail, X } from 'lucide-react';

interface ActionCardDetailModalProps {
  card: ActionCard;
  isOpen: boolean;
  onClose: () => void;
  onToggleStep: (stepId: string, isCompleted: boolean) => Promise<void>;
  onAddStep: (instruction: string, duration?: string) => Promise<void>;
  onDeleteStep: (stepId: string) => Promise<void>;
  onReorderSteps: (orderedStepIds: string[]) => Promise<void>;
  onAddContact: (data: AddContactInput) => Promise<void>;
  onDeleteContact: (contactId: string) => Promise<void>;
  onEdit?: (card: ActionCard) => void;
}

export const ActionCardDetailModal: React.FC<ActionCardDetailModalProps> = ({
  card,
  isOpen,
  onClose,
  onToggleStep,
  onAddStep,
  onDeleteStep,
  onReorderSteps,
  onAddContact,
  onDeleteContact,
  onEdit,
}) => {
  const { isVaultUnlocked, vaultCryptoKey, unlockVault, user } = useAuth();

  // Confidential instruction decryption state
  const [passphrase, setPassphrase] = useState('');
  const [decryptedText, setDecryptedText] = useState<string | null>(null);
  const [decryptError, setDecryptError] = useState<string | null>(null);
  const [isDecrypting, setIsDecrypting] = useState(false);

  useEffect(() => {
    if (!isVaultUnlocked) {
      setDecryptedText(null);
    }
  }, [isVaultUnlocked]);

  // New contact form state
  const [contactName, setContactName] = useState('');
  const [contactRole, setContactRole] = useState('');
  const [contactPhone, setContactPhone] = useState('');
  const [contactEmail, setContactEmail] = useState('');
  const [contactNotes, setContactNotes] = useState('');
  const [isAddingContact, setIsAddingContact] = useState(false);

  if (!isOpen) return null;

  const handleDecrypt = async (e?: React.FormEvent) => {
    if (e) e.preventDefault();
    if (!card.cipherInstructionsBlob || !card.cipherNonce || !card.cipherAuthTag) {
      return;
    }

    setIsDecrypting(true);
    setDecryptError(null);
    try {
      let keyToUse = vaultCryptoKey;
      if (!isVaultUnlocked || !keyToUse) {
        if (!passphrase.trim()) {
          setDecryptError('Vui lòng nhập Master Key của bạn.');
          setIsDecrypting(false);
          return;
        }
        await unlockVault(passphrase);
        const saltHex = user?.encryptionSalt || 'a1b2c3d4e5f60718293a4b5c6d7e8f90';
        const saltBytes = CryptoService.hexToBytes(saltHex);
        keyToUse = await CryptoService.deriveMasterKey(passphrase.trim().toUpperCase(), saltBytes);
      }

      const plain = await CryptoService.decrypt(
        keyToUse,
        card.cipherInstructionsBlob,
        card.cipherNonce,
        card.cipherAuthTag
      );
      setDecryptedText(plain);
    } catch (err: any) {
      const msg =
        err.response?.data?.error?.message ||
        err.response?.data?.message ||
        err.message ||
        'Master Key không chính xác hoặc dữ liệu bị can thiệp.';
      setDecryptError(msg);
    } finally {
      setIsDecrypting(false);
    }
  };

  const handleAddContactSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!contactName.trim() || !contactRole.trim() || card.contacts.length >= 5) return;

    setIsAddingContact(true);
    try {
      await onAddContact({
        contactName: contactName.trim(),
        relationshipOrRole: contactRole.trim(),
        phoneNumber: contactPhone.trim() || undefined,
        email: contactEmail.trim() || undefined,
        contactNotes: contactNotes.trim() || undefined,
      });
      setContactName('');
      setContactRole('');
      setContactPhone('');
      setContactEmail('');
      setContactNotes('');
    } finally {
      setIsAddingContact(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="flex max-h-[90vh] w-full max-w-3xl flex-col rounded-2xl border border-slate-200 bg-white shadow-2xl overflow-hidden">
        {/* Header */}
        <div className="flex items-start justify-between border-b border-slate-200 p-5">
          <div className="space-y-1">
            <div className="flex flex-wrap items-center gap-2">
              <span className="rounded bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-700 border border-emerald-200">
                {card.categoryNameVi || card.categoryCode}
              </span>
              <span className="rounded bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-700">
                {card.urgency}
              </span>
              <span className="rounded bg-red-50 border border-red-200 px-2 py-0.5 text-xs font-medium text-red-700">
                Ưu tiên: {card.priority}
              </span>
              {card.isCompleted && (
                <span className="rounded bg-emerald-50 border border-emerald-200 px-2 py-0.5 text-xs font-medium text-emerald-700">
                  ✓ Hoàn tất đầy đủ
                </span>
              )}
            </div>
            <h2 className="text-xl font-bold text-slate-900">{card.title}</h2>
          </div>
          <div className="flex items-center gap-2">
            {onEdit && (
              <button
                type="button"
                onClick={() => onEdit(card)}
                className="rounded-lg bg-slate-100 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-200 transition border border-slate-200"
              >
                Sửa thẻ
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

        {/* Content Body */}
        <div className="flex-1 space-y-6 overflow-y-auto p-6">
          {/* Summary */}
          {card.summary && (
            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
              <h4 className="text-xs font-semibold uppercase tracking-wider text-slate-500">Tóm tắt bối cảnh</h4>
              <p className="mt-1 text-sm text-slate-700">{card.summary}</p>
            </div>
          )}

          {/* Key Locations & Assets Info */}
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div className="rounded-xl border border-slate-200 bg-slate-50 p-3.5">
              <span className="text-xs text-slate-500">Vị trí tài liệu / Hồ sơ vật lý:</span>
              <p className="mt-0.5 text-sm font-medium text-slate-800">
                {card.documentLocationHint || 'Chưa ghi nhận'}
              </p>
            </div>
            <div className="rounded-xl border border-slate-200 bg-slate-50 p-3.5">
              <span className="text-xs text-slate-500">Liên kết số / Cloud Storage:</span>
              <p className="mt-0.5 text-sm font-medium text-emerald-700 truncate">
                {card.digitalStorageLink ? (
                  <a href={card.digitalStorageLink} target="_blank" rel="noreferrer" className="hover:underline">
                    {card.digitalStorageLink}
                  </a>
                ) : (
                  'Không có'
                )}
              </p>
            </div>
          </div>

          {/* Zero-Knowledge Confidential Instructions */}
          {card.hasConfidentialInstructions && (
            <div className="rounded-xl border border-purple-200 bg-purple-50 p-4">
              <div className="flex items-center gap-2">
                <Lock className="w-5 h-5 text-purple-600 shrink-0" />
                <div>
                  <h4 className="text-sm font-semibold text-purple-900">Chỉ dẫn bí mật được mã hóa đầu-cuối (Zero-Knowledge)</h4>
                  <p className="text-xs text-purple-700">Máy chủ chỉ lưu Ciphertext. Nhập Master Passphrase để giải mã cục bộ trên trình duyệt.</p>
                </div>
              </div>

              {decryptedText ? (
                <div className="mt-3 rounded-lg border border-emerald-200 bg-emerald-50 p-3">
                  <div className="flex items-center justify-between mb-1">
                    <span className="text-xs font-semibold text-emerald-700">Nội dung đã giải mã:</span>
                    <span className="text-[10px] text-emerald-700 font-mono">Decrypted via Master Key</span>
                  </div>
                  <p className="mt-1 font-mono text-sm text-slate-800 whitespace-pre-wrap">{decryptedText}</p>
                </div>
              ) : isVaultUnlocked ? (
                <div className="mt-3 flex items-center justify-between p-3 rounded-lg bg-emerald-50 border border-emerald-200">
                  <div className="flex items-center gap-2 text-xs text-emerald-700">
                    <ShieldCheck className="w-4 h-4 text-emerald-600 shrink-0" />
                    <span>Két đã mở khóa với Master Key của bạn</span>
                  </div>
                  <button
                    type="button"
                    onClick={() => handleDecrypt()}
                    disabled={isDecrypting}
                    className="px-3.5 py-1.5 rounded-lg bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold shadow-sm transition disabled:opacity-50"
                  >
                    {isDecrypting ? 'Đang giải mã…' : 'Giải mã ngay'}
                  </button>
                </div>
              ) : (
                <form onSubmit={handleDecrypt} className="mt-3 flex flex-col gap-2 sm:flex-row">
                  <div className="relative flex-1">
                    <input
                      type="text"
                      autoComplete="off"
                      spellCheck={false}
                      placeholder="Nhập đúng Master Key (AK-XXXX-XXXX-XXXX-XXXX)…"
                      value={passphrase}
                      onChange={(e) => setPassphrase(e.target.value)}
                      className="w-full rounded-md border border-purple-300 bg-white px-3 py-1.5 text-xs font-mono text-slate-900 placeholder:font-sans placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-purple-500 focus-visible:ring-offset-2"
                    />
                  </div>
                  <button
                    type="submit"
                    disabled={!passphrase.trim() || isDecrypting}
                    className="rounded-md bg-purple-600 px-4 py-1.5 text-xs font-semibold text-white hover:bg-purple-500 disabled:opacity-50 flex items-center justify-center gap-1.5 shrink-0 transition"
                  >
                    <KeyRound className="w-3.5 h-3.5" />
                    {isDecrypting ? 'Đang xác thực…' : 'Mở khóa & Giải mã'}
                  </button>
                </form>
              )}
              {decryptError && <p role="alert" className="mt-2 text-xs text-red-700 bg-red-50 p-2 rounded border border-red-200">{decryptError}</p>}
            </div>
          )}

          {/* Step Checklist Section */}
          <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
            <StepChecklistEditor
              steps={card.steps}
              onToggleStep={onToggleStep}
              onAddStep={onAddStep}
              onDeleteStep={onDeleteStep}
              onReorderSteps={onReorderSteps}
            />
          </div>

          {/* Key Contacts Section */}
          <div className="rounded-xl border border-slate-200 bg-slate-50 p-4 space-y-3">
            <div className="flex items-center justify-between">
              <h4 className="text-sm font-semibold text-slate-800">
                Đầu mối liên hệ chủ chốt (<span className="tabular-nums">{card.contacts.length}</span>/5)
              </h4>
              {card.contacts.length >= 5 && (
                <span className="text-xs text-amber-700 font-medium">Tối đa 5 liên hệ</span>
              )}
            </div>

            <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
              {card.contacts.map((contact) => (
                <div
                  key={contact.id}
                  className="flex items-start justify-between rounded-lg border border-slate-200 bg-white p-3 shadow-sm"
                >
                  <div className="space-y-1">
                    <h5 className="text-sm font-semibold text-slate-900">{contact.contactName}</h5>
                    <span className="inline-block rounded bg-slate-100 px-1.5 py-0.5 text-xs text-slate-600">
                      {contact.relationshipOrRole}
                    </span>
                    <div className="flex flex-wrap gap-2 text-xs text-emerald-700 pt-1">
                      {contact.phoneNumber && (
                        <a href={`tel:${contact.phoneNumber}`} className="hover:underline flex items-center gap-1">
                          <Phone className="w-3 h-3 text-slate-500" /> {contact.phoneNumber}
                        </a>
                      )}
                      {contact.email && (
                        <a href={`mailto:${contact.email}`} className="hover:underline flex items-center gap-1">
                          <Mail className="w-3 h-3 text-slate-500" /> {contact.email}
                        </a>
                      )}
                    </div>
                    {contact.contactNotes && (
                      <p className="text-[11px] text-slate-500 italic">"{contact.contactNotes}"</p>
                    )}
                  </div>
                  <button
                    type="button"
                    onClick={() => onDeleteContact(contact.id)}
                    className="rounded p-1 text-slate-400 hover:text-red-600 transition"
                    title="Xóa đầu mối này"
                    aria-label="Xóa đầu mối này"
                  >
                    <X className="w-4 h-4" />
                  </button>
                </div>
              ))}
            </div>

            {/* Add Contact inline form */}
            {card.contacts.length < 5 && (
              <form onSubmit={handleAddContactSubmit} className="mt-2 space-y-2 rounded-lg border border-slate-200 p-3 bg-white shadow-sm">
                <span className="text-xs font-medium text-slate-700">+ Thêm đầu mối liên hệ mới:</span>
                <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
                  <input
                    type="text"
                    placeholder="Họ tên / Đơn vị…"
                    value={contactName}
                    onChange={(e) => setContactName(e.target.value)}
                    required
                    className="rounded-md border border-slate-300 bg-slate-50 px-3 py-1 text-xs text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
                  />
                  <input
                    type="text"
                    placeholder="Vai trò (VD: Luật sư, Quản lý tòa nhà)…"
                    value={contactRole}
                    onChange={(e) => setContactRole(e.target.value)}
                    required
                    className="rounded-md border border-slate-300 bg-slate-50 px-3 py-1 text-xs text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
                  />
                  <input
                    type="text"
                    placeholder="Số điện thoại…"
                    value={contactPhone}
                    onChange={(e) => setContactPhone(e.target.value)}
                    className="rounded-md border border-slate-300 bg-slate-50 px-3 py-1 text-xs text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
                  />
                  <input
                    type="email"
                    placeholder="Email…"
                    value={contactEmail}
                    onChange={(e) => setContactEmail(e.target.value)}
                    className="rounded-md border border-slate-300 bg-slate-50 px-3 py-1 text-xs text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
                  />
                </div>
                <input
                  type="text"
                  placeholder="Ghi chú thêm…"
                  value={contactNotes}
                  onChange={(e) => setContactNotes(e.target.value)}
                  className="w-full rounded-md border border-slate-300 bg-slate-50 px-3 py-1 text-xs text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
                />
                <button
                  type="submit"
                  disabled={!contactName.trim() || !contactRole.trim() || isAddingContact}
                  className="rounded-md bg-slate-100 border border-slate-200 px-3 py-1 text-xs font-semibold text-slate-700 hover:bg-slate-200 transition disabled:opacity-50"
                >
                  {isAddingContact ? 'Đang lưu…' : '+ Lưu liên hệ'}
                </button>
              </form>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};
