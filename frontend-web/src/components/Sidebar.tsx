import React from 'react';
import { Map, Layers, Users, BookOpen, KeyRound } from 'lucide-react';

interface SidebarProps {
  currentTab: string;
  onTabChange: (tab: string) => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ currentTab, onTabChange }) => {
  const navItems = [
    { id: 'map', label: 'Continuity Map', icon: Map, badge: 'v1.0.0' },
    { id: 'cards', label: 'Action Cards', icon: Layers },
    { id: 'people', label: 'Trusted People', icon: Users },
    { id: 'plan', label: 'Continuity Plan', icon: BookOpen },
    { id: 'activation', label: 'Safe Activation', icon: KeyRound, alert: true },
  ];

  return (
    <aside className="w-64 border-r border-slate-800 bg-slate-900/40 p-4 flex flex-col justify-between">
      <div className="space-y-1">
        <p className="text-xs font-semibold text-slate-500 uppercase tracking-wider px-3 mb-2">5 MVP Core Features</p>
        {navItems.map((item) => {
          const Icon = item.icon;
          const isActive = currentTab === item.id;
          return (
            <button
              key={item.id}
              onClick={() => onTabChange(item.id)}
              className={`w-full flex items-center justify-between px-3 py-2.5 rounded-lg text-sm font-medium transition ${
                isActive
                  ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/30'
                  : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/60'
              }`}
            >
              <div className="flex items-center gap-3">
                <Icon className="w-4 h-4" />
                <span>{item.label}</span>
              </div>
              {item.badge && (
                <span className="text-[10px] px-1.5 py-0.5 rounded bg-emerald-500/20 text-emerald-300 font-mono">
                  {item.badge}
                </span>
              )}
            </button>
          );
        })}
      </div>

      <div className="p-3 rounded-xl bg-slate-800/40 border border-slate-800 text-xs text-slate-400 space-y-1">
        <p className="font-semibold text-slate-300">Spec-Driven Dev</p>
        <p>Synchronized across .NET 8 API, React Web & Flutter App.</p>
      </div>
    </aside>
  );
};
