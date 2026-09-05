import React from 'react';
import { CheckCircle2 } from 'lucide-react';

export const TrustedPeopleList: React.FC = () => {
  const people = [
    { id: '1', name: 'Sarah Connor', role: 'Primary Delegate', level: 'Level 3 (Full Vault)' },
    { id: '2', name: 'John Matrix', role: 'Business Executor', level: 'Level 2 (Business Assets)' },
    { id: '3', name: 'David Bowman', role: 'Emergency Contact', level: 'Level 1 (Notice Only)' }
  ];

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-white tracking-tight">Trusted People</h2>
        <p className="text-sm text-slate-400">Mạng lưới người được ủy thác và ma trận phân quyền (feat-03-trusted-people).</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {people.map((p) => (
          <div key={p.id} className="p-5 rounded-xl bg-slate-900/40 border border-slate-800 space-y-3">
            <div className="w-10 h-10 rounded-full bg-slate-800 flex items-center justify-center text-slate-300 font-bold">
              {p.name.charAt(0)}
            </div>
            <h3 className="font-semibold text-white">{p.name}</h3>
            <p className="text-xs text-emerald-400 font-mono flex items-center gap-1">
              <CheckCircle2 className="w-3.5 h-3.5" /> {p.role}
            </p>
            <p className="text-xs text-slate-400">{p.level}</p>
          </div>
        ))}
      </div>
    </div>
  );
};
