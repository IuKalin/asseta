import React from 'react';
import { ShieldAlert, Clock } from 'lucide-react';

export const SafeActivationPanel: React.FC = () => {
  return (
    <div className="space-y-6 max-w-2xl">
      <div>
        <h2 className="text-2xl font-bold text-white tracking-tight">Safe Activation Protocol</h2>
        <p className="text-sm text-slate-400">Giao thức kích hoạt an toàn với cơ chế bảo vệ Time-lock và Dead-man Switch (feat-05-safe-activation).</p>
      </div>

      <div className="p-6 rounded-xl bg-amber-500/10 border border-amber-500/30 space-y-4">
        <div className="flex items-center gap-3 text-amber-400">
          <ShieldAlert className="w-6 h-6" />
          <h3 className="font-bold text-lg">Cơ chế Bảo Vệ Time-Lock Khẩn Cấp</h3>
        </div>
        <p className="text-sm text-slate-300">
          Khi một người ủy thác kích hoạt giao thức, hệ thống sẽ bắt đầu đếm ngược thời gian chờ (48 giờ). Chủ tài sản sẽ nhận được thông báo qua SMS, Email và App. Trong thời gian này, chủ tài sản có thể hủy lệnh với 1 chạm.
        </p>

        <div className="pt-2 flex items-center gap-4">
          <button className="px-4 py-2 rounded-lg bg-amber-500 hover:bg-amber-400 text-slate-950 font-semibold text-sm transition">
            Mô Phỏng Yêu Cầu Kích Hoạt
          </button>
          <div className="flex items-center gap-1.5 text-xs text-slate-400">
            <Clock className="w-4 h-4" /> Thời gian chờ cấu hình: 48h
          </div>
        </div>
      </div>
    </div>
  );
};
