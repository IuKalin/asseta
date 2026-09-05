import React from 'react';
import { ShieldCheck, Bell, User } from 'lucide-react';

export const Header: React.FC = () => {
  return (
    <header className="h-16 border-b border-slate-800 bg-slate-900/60 backdrop-blur px-6 flex items-center justify-between sticky top-0 z-40">
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-emerald-500/20 border border-emerald-500/40 flex items-center justify-center text-emerald-400">
          <ShieldCheck className="w-6 h-6" />
        </div>
        <div>
          <h1 className="font-bold text-lg text-white leading-tight">Asseta Monorepo</h1>
          <p className="text-xs text-emerald-400 font-mono">feat-01-continuity-map (v1.0.0 APPROVED)</p>
        </div>
      </div>

      <div className="flex items-center gap-4">
        <button className="p-2 rounded-lg bg-slate-800 text-slate-300 hover:text-white transition">
          <Bell className="w-5 h-5" />
        </button>
        <div className="flex items-center gap-2 pl-2 border-l border-slate-800">
          <div className="w-8 h-8 rounded-full bg-slate-800 flex items-center justify-center text-slate-300">
            <User className="w-4 h-4" />
          </div>
          <span className="text-sm font-medium text-slate-300">Estate Owner</span>
        </div>
      </div>
    </header>
  );
};
