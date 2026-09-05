import React from 'react';
import { Layers } from 'lucide-react';

export const ActionCardList: React.FC = () => {
  const sampleCards = [
    { id: '1', title: 'Bàn giao quyền quản trị VPS & DNS', priority: 'High', to: 'Alex (Tech Delegate)' },
    { id: '2', title: 'Thông báo giải ngân quỹ dự phòng gia đình', priority: 'High', to: 'Sarah (Financial Trustee)' },
    { id: '3', title: 'Chuyển quyền sở hữu giấy phép kinh doanh', priority: 'Medium', to: 'Legal Counsel' }
  ];

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-white tracking-tight">Action Cards</h2>
        <p className="text-sm text-slate-400">Thẻ hướng dẫn hành động khẩn cấp từng bước khi xảy ra biến cố (feat-02-action-cards).</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {sampleCards.map((card) => (
          <div key={card.id} className="p-5 rounded-xl bg-slate-900/40 border border-slate-800 space-y-3">
            <div className="flex items-center justify-between">
              <span className="text-xs font-semibold px-2 py-0.5 rounded bg-rose-500/20 text-rose-300">
                {card.priority} Priority
              </span>
              <Layers className="w-4 h-4 text-slate-500" />
            </div>
            <h3 className="font-semibold text-white">{card.title}</h3>
            <p className="text-xs text-slate-400">Chỉ định: <span className="text-slate-200">{card.to}</span></p>
          </div>
        ))}
      </div>
    </div>
  );
};
