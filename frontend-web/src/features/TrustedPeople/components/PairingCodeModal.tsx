import React, { useState } from 'react';
import { X, Copy, Check, ShieldCheck, Clock } from 'lucide-react';
import { TrustedPerson } from '../../../types/trustedPeople';

interface PairingCodeModalProps {
  person: TrustedPerson;
  pairingCode: string;
  expiresAt?: string | null;
  onClose: () => void;
}

export const PairingCodeModal: React.FC<PairingCodeModalProps> = ({
  person,
  pairingCode,
  expiresAt,
  onClose,
}) => {
  const [copied, setCopied] = useState(false);

  const handleCopy = () => {
    navigator.clipboard.writeText(pairingCode);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4">
      <div className="bg-white border border-slate-200 rounded-2xl max-w-md w-full p-6 space-y-6 shadow-2xl relative">
        <button
          type="button"
          onClick={onClose}
          aria-label="Đóng"
          className="absolute top-4 right-4 text-slate-400 hover:text-slate-600 p-1 rounded-lg hover:bg-slate-100 transition-colors"
        >
          <X className="w-5 h-5" />
        </button>

        <div className="text-center space-y-2">
          <div className="w-12 h-12 rounded-full bg-emerald-50 text-emerald-600 border border-emerald-100 mx-auto flex items-center justify-center shadow-xs">
            <ShieldCheck className="w-6 h-6" />
          </div>
          <h3 className="text-xl font-bold text-slate-900">Mã Ghép Đôi Danh Tính</h3>
          <p className="text-sm text-slate-500">
            Cung cấp mã này cho <strong className="text-slate-900">{person.fullName}</strong> để liên kết ứng dụng Asseta di động.
          </p>
        </div>

        <div className="p-5 rounded-xl bg-slate-50 border border-slate-200 text-center space-y-3">
          <span className="text-xs font-mono uppercase tracking-widest text-slate-500">Pairing Code (6 ký tự)</span>
          <div className="text-4xl font-mono font-black tracking-widest text-emerald-600 select-all tabular-nums">
            {pairingCode}
          </div>
          <button
            type="button"
            onClick={handleCopy}
            aria-live="polite"
            className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-white border border-slate-300 hover:bg-slate-100 text-slate-700 text-sm font-medium transition-colors shadow-xs"
          >
            {copied ? (
              <>
                <Check className="w-4 h-4 text-emerald-600" /> Đã sao chép
              </>
            ) : (
              <>
                <Copy className="w-4 h-4" /> Sao chép mã
              </>
            )}
          </button>
        </div>

        <div className="flex items-center gap-2 text-xs text-amber-800 bg-amber-50 border border-amber-200 p-3 rounded-lg">
          <Clock className="w-4 h-4 shrink-0 text-amber-600" />
          <span>
            {expiresAt ? `Hạn dùng: ${new Date(expiresAt).toLocaleString('vi-VN')}. ` : 'Mã chỉ có hiệu lực trong 48 giờ. '}
            Sau khi người ủy thác nhập mã trên ứng dụng, mã sẽ tự động vô hiệu hóa.
          </span>
        </div>

        <button
          type="button"
          onClick={onClose}
          className="w-full py-2.5 rounded-xl bg-slate-100 hover:bg-slate-200 text-slate-800 font-medium transition-colors"
        >
          Hoàn tất
        </button>
      </div>
    </div>
  );
};
