import React, { useState } from 'react';
import { ShieldAlert, Copy, Check, Download, AlertTriangle, KeyRound } from 'lucide-react';

interface MasterKeyBackupModalProps {
  masterKey: string;
  email: string;
  onClose: () => void;
}

export const MasterKeyBackupModal: React.FC<MasterKeyBackupModalProps> = ({
  masterKey,
  email,
  onClose,
}) => {
  const [copied, setCopied] = useState(false);
  const [confirmed, setConfirmed] = useState(false);

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(masterKey);
      setCopied(true);
      setTimeout(() => setCopied(false), 3000);
    } catch {
      // Fallback
      const el = document.createElement('textarea');
      el.value = masterKey;
      document.body.appendChild(el);
      el.select();
      document.execCommand('copy');
      document.body.removeChild(el);
      setCopied(true);
      setTimeout(() => setCopied(false), 3000);
    }
  };

  const handleDownload = () => {
    const content = `=====================================================
ASSETA VAULT - MASTER CONTINUITY KEY
=====================================================
Tài khoản email: ${email}
Master Key: ${masterKey}
Ngày tạo: ${new Date().toLocaleString('vi-VN')}

LƯU Ý QUAN TRỌNG:
1. Master Key này là chìa khóa duy nhất để mã hóa và giải mã dữ liệu tài sản nhạy cảm trong hệ thống Asseta.
2. Asseta tuân thủ kiến trúc Zero-Knowledge: Chúng tôi KHÔNG lưu trữ Master Key dạng văn bản thuần trên máy chủ.
3. Nếu bạn làm mất khóa này, KHÔNG AI (kể cả quản trị viên Asseta) có thể khôi phục lại kho dữ liệu của bạn.
4. Hãy lưu file này vào USB bảo mật, két sắt số hoặc in ra giấy cất giữ cẩn thận.
=====================================================`;

    const blob = new Blob([content], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `asseta-master-key-${email.replace(/[@.]/g, '_')}.txt`;
    link.click();
    URL.revokeObjectURL(url);
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm">
      <div className="relative w-full max-w-xl bg-white border border-slate-200 rounded-2xl shadow-2xl overflow-hidden p-6 sm:p-8 animate-in fade-in zoom-in-95 duration-200">

        {/* Glow Header */}
        <div className="flex items-center gap-4 mb-6">
          <div className="w-14 h-14 rounded-2xl bg-emerald-50 border border-emerald-200 flex items-center justify-center text-emerald-600 shrink-0">
            <KeyRound className="w-8 h-8" />
          </div>
          <div>
            <span className="px-2.5 py-0.5 rounded-full text-xs font-semibold bg-emerald-50 text-emerald-700 border border-emerald-200">
              BẢO MẬT ZERO-KNOWLEDGE
            </span>
            <h2 className="text-2xl font-bold text-slate-900 mt-1">Lưu trữ Master Key của bạn</h2>
            <p className="text-sm text-slate-600">
              Khóa dự phòng bất biến vừa được khởi tạo cho tài khoản{' '}
              <span className="text-emerald-700 font-medium">{email}</span>
            </p>
          </div>
        </div>

        {/* Master Key Display Box */}
        <div className="bg-slate-50 border border-slate-200 rounded-xl p-5 mb-5 shadow-sm">
          <div className="text-xs uppercase tracking-wider text-slate-500 mb-2 font-mono flex items-center justify-between">
            <span>MASTER CONTINUITY KEY</span>
            <span className="text-emerald-700 text-[11px] font-semibold">KHÔNG THỂ THAY ĐỔI</span>
          </div>
          <div className="font-mono text-xl sm:text-2xl font-bold text-emerald-700 tracking-wider text-center py-3 select-all bg-emerald-50 border border-emerald-200 rounded-lg tabular-nums">
            {masterKey}
          </div>

          <div className="grid grid-cols-2 gap-3 mt-4">
            <button
              onClick={handleCopy}
              type="button"
              className="flex items-center justify-center gap-2 px-4 py-2.5 rounded-lg bg-white hover:bg-slate-50 text-slate-700 font-medium text-sm transition border border-slate-300 shadow-sm"
            >
              {copied ? (
                <span aria-live="polite" className="flex items-center gap-1.5 text-emerald-700">
                  <Check className="w-4 h-4 text-emerald-600" />
                  <span>Đã sao chép!</span>
                </span>
              ) : (
                <>
                  <Copy className="w-4 h-4 text-slate-500" />
                  <span>Sao chép khóa</span>
                </>
              )}
            </button>

            <button
              onClick={handleDownload}
              type="button"
              className="flex items-center justify-center gap-2 px-4 py-2.5 rounded-lg bg-emerald-50 hover:bg-emerald-100 text-emerald-700 font-medium text-sm transition border border-emerald-200"
            >
              <Download className="w-4 h-4 text-emerald-600" />
              <span>Tải file dự phòng (.txt)</span>
            </button>
          </div>
        </div>

        {/* Warning Notices */}
        <div className="space-y-3 mb-6">
          <div className="flex items-start gap-3 p-3.5 rounded-xl bg-amber-50 border border-amber-200 text-amber-800 text-xs leading-relaxed">
            <AlertTriangle className="w-5 h-5 text-amber-600 shrink-0 mt-0.5" />
            <div>
              <p className="font-semibold text-amber-900">Đã gửi vào Email & Không thể khôi phục:</p>
              <p className="text-amber-800 mt-0.5">
                Hệ thống đã gửi một bản sao đến hòm thư <strong className="text-slate-900 font-semibold">{email}</strong>.
                Do kiến trúc Zero-Knowledge, Asseta không bao giờ lưu trữ khóa này trên máy chủ. Nếu đánh mất, bạn sẽ không thể giải mã các tài sản được mã hóa.
              </p>
            </div>
          </div>
        </div>

        {/* Checkbox Confirmation */}
        <label className="flex items-start gap-3 p-3 bg-slate-50 rounded-xl border border-slate-200 cursor-pointer hover:bg-slate-100 transition mb-6 select-none">
          <input
            type="checkbox"
            checked={confirmed}
            onChange={(e) => setConfirmed(e.target.checked)}
            className="mt-1 w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500 focus:ring-offset-white"
          />
          <span className="text-xs text-slate-700 leading-normal">
            Tôi đã sao chép hoặc tải xuống Master Key và hiểu rằng chìa khóa này là bất biến, không thể cấp lại nếu làm mất.
          </span>
        </label>

        {/* Action Button */}
        <button
          onClick={onClose}
          disabled={!confirmed}
          className={`w-full py-3 px-6 rounded-xl font-semibold text-sm transition flex items-center justify-center gap-2 ${confirmed
              ? 'bg-emerald-600 hover:bg-emerald-500 text-white shadow-sm cursor-pointer'
              : 'bg-slate-100 text-slate-400 border border-slate-200 cursor-not-allowed'
            }`}
        >
          <ShieldAlert className="w-4 h-4" />
          <span>Vào Bảng Điều Khiển Asseta</span>
        </button>
      </div>
    </div>
  );
};
