import React from 'react';
import { ShieldCheck, Bell } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';

export const Header: React.FC = () => {
  const { isVaultUnlocked, lockVault } = useAuth();

  return (
    <header className="h-16 border-b border-[#EBEBEB] bg-white/90 backdrop-blur px-6 flex items-center justify-between sticky top-0 z-40">
      {/* Brand Logo - Updated per Image 3: "Asseta" + "Version 1.0.0" */}
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-2xl bg-rose-50 border border-rose-100 flex items-center justify-center text-[#FF385C] shadow-xs">
          <ShieldCheck className="w-5 h-5" />
        </div>
        <div>
          <h1 className="font-bold text-lg text-[#222222] leading-tight tracking-tight">Asseta</h1>
          <p className="text-xs text-[#717171] font-mono">Version 1.0.0</p>
        </div>
      </div>

      {/* Header Actions: Vault Status & Notifications */}
      <div className="flex items-center gap-3">
        {isVaultUnlocked ? (
          <div className="flex items-center gap-2 px-3 py-1.5 rounded-full bg-emerald-50 border border-emerald-200 text-emerald-700 text-xs font-medium">
            <span className="w-2 h-2 rounded-full bg-emerald-500 animate-pulse motion-reduce:animate-none" />
            <span className="hidden sm:inline">Két Mở</span>
            <button
              onClick={lockVault}
              title="Khóa két mã hóa"
              aria-label="Khóa két mã hóa"
              className="text-xs text-slate-500 hover:text-slate-800 px-1.5 py-0.5 rounded hover:bg-emerald-100 transition cursor-pointer"
            >
              [Khóa]
            </button>
          </div>
        ) : (
          <div className="flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-amber-50 border border-amber-200 text-amber-700 text-xs font-medium">
            <span className="w-2 h-2 rounded-full bg-amber-500" />
            <span>Két Khóa</span>
          </div>
        )}

        <button 
          aria-label="Thông báo"
          className="w-9 h-9 rounded-full bg-white hover:bg-[#F7F7F7] text-[#222222] border border-[#EBEBEB] flex items-center justify-center transition cursor-pointer shadow-xs"
        >
          <Bell className="w-4 h-4" />
        </button>
      </div>
    </header>
  );
};
