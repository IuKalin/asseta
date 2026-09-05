import React from 'react';
import { Check } from 'lucide-react';

export const ContinuityPlanView: React.FC = () => {
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-white tracking-tight">Continuity Plan</h2>
        <p className="text-sm text-slate-400">Kế hoạch duy trì liên tục và kịch bản ứng phó rủi ro (feat-04-continuity-plan).</p>
      </div>

      <div className="p-6 rounded-xl bg-slate-900/40 border border-slate-800 space-y-4">
        <h3 className="font-semibold text-lg text-white">Kịch Bản: Mất Khả Năng Vận Hành Khẩn Cấp (Incapacitation Protocol)</h3>
        <ul className="space-y-2 text-sm text-slate-300">
          <li className="flex items-center gap-2">
            <Check className="w-4 h-4 text-emerald-400" /> Kích hoạt Time-lock 48 giờ để xác minh tình trạng chủ sở hữu.
          </li>
          <li className="flex items-center gap-2">
            <Check className="w-4 h-4 text-emerald-400" /> Gửi thông báo khẩn cấp tới 3 người ủy thác cấp cao.
          </li>
          <li className="flex items-center gap-2">
            <Check className="w-4 h-4 text-emerald-400" /> Mở khóa các Action Cards theo thứ tự ưu tiên High &rarr; Low.
          </li>
        </ul>
      </div>
    </div>
  );
};
