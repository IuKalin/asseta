import React, { useState } from 'react';
import {
  AlertTriangle,
  FileText,
  Lock,
  LockOpen,
  MapPin,
  Pencil,
  Shield,
  Trash2,
  User,
} from 'lucide-react';
import { ContinuityItem } from '../../../types/continuity';

interface Props {
  item: ContinuityItem;
  isKeyUnlocked: boolean;
  onEdit: (item: ContinuityItem) => void;
  onDelete: (id: string) => void;
  onDecryptNotes: (item: ContinuityItem) => Promise<string>;
  onPromptUnlock: () => void;
}

export const ContinuityItemCard: React.FC<Props> = ({
  item,
  isKeyUnlocked,
  onEdit,
  onDelete,
  onDecryptNotes,
  onPromptUnlock,
}) => {
  const [decryptedNotes, setDecryptedNotes] = useState<string | null>(null);
  const [isDecrypting, setIsDecrypting] = useState<boolean>(false);
  const [showNotes, setShowNotes] = useState<boolean>(false);

  React.useEffect(() => {
    if (!isKeyUnlocked) {
      setShowNotes(false);
      setDecryptedNotes(null);
    }
  }, [isKeyUnlocked]);

  const getPriorityBadge = (priority: string) => {
    switch (priority) {
      case 'CRITICAL':
        return 'bg-red-50 text-red-700 border-red-200';
      case 'IMPORTANT':
        return 'bg-amber-50 text-amber-700 border-amber-200';
      default:
        return 'bg-sky-50 text-sky-700 border-sky-200';
    }
  };

  const handleToggleNotes = async () => {
    if (!item.hasConfidentialNotes) return;

    if (!isKeyUnlocked) {
      onPromptUnlock();
      return;
    }

    if (showNotes) {
      setShowNotes(false);
      return;
    }

    if (!decryptedNotes) {
      try {
        setIsDecrypting(true);
        const text = await onDecryptNotes(item);
        setDecryptedNotes(text);
        setShowNotes(true);
      } catch (err) {
        console.error('Failed to decrypt notes:', err);
      } finally {
        setIsDecrypting(false);
      }
    } else {
      setShowNotes(true);
    }
  };

  return (
    <div className="rounded-xl bg-white border border-slate-200 hover:border-slate-300 hover:shadow-sm transition p-4 flex flex-col justify-between space-y-3">
      {/* Top Header */}
      <div className="flex items-start justify-between gap-3">
        <div className="space-y-1">
          <div className="flex items-center gap-2">
            <span
              className={`text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full border ${getPriorityBadge(
                item.priority
              )}`}
            >
              {item.priority}
            </span>
            {item.hasContinuityGap && (
              <span className="inline-flex items-center gap-1 text-[10px] text-red-700 bg-red-50 border border-red-200 px-1.5 py-0.5 rounded-md font-medium">
                <AlertTriangle className="w-3 h-3" /> Gap
              </span>
            )}
            {item.hasConfidentialNotes && (
              <span className="inline-flex items-center gap-1 text-[10px] text-emerald-700 bg-emerald-50 border border-emerald-200 px-1.5 py-0.5 rounded-md font-medium">
                <Shield className="w-3 h-3" /> Encrypted
              </span>
            )}
          </div>
          <h4 className="text-sm font-semibold text-slate-900 leading-snug">{item.name}</h4>
        </div>

        {/* Action buttons */}
        <div className="flex items-center gap-1 shrink-0">
          <button
            type="button"
            onClick={() => onEdit(item)}
            className="p-1.5 rounded-md hover:bg-slate-100 text-slate-500 hover:text-slate-900 transition"
            title="Chỉnh sửa"
            aria-label="Chỉnh sửa"
          >
            <Pencil className="w-3.5 h-3.5" />
          </button>
          <button
            type="button"
            onClick={() => onDelete(item.id)}
            className="p-1.5 rounded-md hover:bg-red-50 text-slate-500 hover:text-red-600 transition"
            title="Xóa"
            aria-label="Xóa"
          >
            <Trash2 className="w-3.5 h-3.5" />
          </button>
        </div>
      </div>

      {/* Metadata hints */}
      <div className="space-y-1.5 text-xs text-slate-500">
        <div className="flex items-center gap-1.5">
          <MapPin className="w-3.5 h-3.5 text-slate-400 shrink-0" />
          <span className="truncate">
            {item.documentLocationHint || (
              <span className="text-amber-700 italic">Chưa có vị trí hồ sơ</span>
            )}
          </span>
        </div>

        <div className="flex items-center gap-1.5">
          <User className="w-3.5 h-3.5 text-slate-400 shrink-0" />
          <span className="truncate">
            {item.assignedTrustedPersonId ? (
              <span className="text-slate-700">Đã chỉ định người tiếp quản</span>
            ) : (
              <span className="text-red-700 italic">Chưa có người phụ trách</span>
            )}
          </span>
        </div>
      </div>

      {/* Confidential Notes Section */}
      {item.hasConfidentialNotes && (
        <div className="pt-2 border-t border-slate-200">
          <button
            type="button"
            onClick={handleToggleNotes}
            disabled={isDecrypting}
            className="inline-flex items-center gap-1.5 text-xs text-emerald-600 hover:text-emerald-700 transition font-medium"
          >
            {isKeyUnlocked ? (
              showNotes ? (
                <>
                  <LockOpen className="w-3.5 h-3.5" /> Thu gọn ghi chú bí mật
                </>
              ) : (
                <>
                  <Lock className="w-3.5 h-3.5" /> Xem ghi chú bí mật (AES-256)
                </>
              )
            ) : (
              <>
                <Lock className="w-3.5 h-3.5 text-amber-600" />
                <span className="text-amber-700">Mở khóa để xem ghi chú bí mật</span>
              </>
            )}
          </button>

          {showNotes && decryptedNotes && (
            <div className="mt-2 p-2.5 rounded-lg bg-slate-50 border border-emerald-200 text-xs text-slate-800 whitespace-pre-wrap font-sans">
              <div className="flex items-center gap-1 text-[10px] text-emerald-700 uppercase font-mono mb-1">
                <FileText className="w-3 h-3" /> Ghi chú đã giải mã phía Client:
              </div>
              {decryptedNotes}
            </div>
          )}
        </div>
      )}
    </div>
  );
};
