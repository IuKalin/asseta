import React from 'react';
import { Map, Layers, Users, BookOpen, KeyRound, User, LogOut } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';

interface SidebarProps {
  currentTab: string;
  onTabChange: (tab: string) => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ currentTab, onTabChange }) => {
  const { user, logout } = useAuth();

  const navItems = [
    { id: 'map', label: 'Continuity Map', icon: Map, badge: 'v1.0.0' },
    { id: 'cards', label: 'Action Cards', icon: Layers },
    { id: 'people', label: 'Trusted People', icon: Users },
    { id: 'plan', label: 'Continuity Plan', icon: BookOpen },
    { id: 'activation', label: 'Safe Activation', icon: KeyRound, alert: true },
  ];

  return (
    <aside className="w-[300px] border-r border-[#EBEBEB] bg-white p-4 flex flex-col justify-between shrink-0 select-none">
      {/* Top Nav List */}
      <div className="space-y-1.5">
        <p className="text-[11px] font-bold text-[#717171] uppercase tracking-wider px-3 mb-2">
          5 MVP Core Features
        </p>
        {navItems.map((item) => {
          const Icon = item.icon;
          const isActive = currentTab === item.id;
          return (
            <button
              key={item.id}
              onClick={() => onTabChange(item.id)}
              className={`w-full flex items-center justify-between px-3.5 py-2.5 rounded-2xl text-xs font-semibold transition cursor-pointer ${
                isActive
                  ? 'bg-rose-50 text-[#FF385C] border border-rose-200/80 shadow-xs'
                  : 'text-[#717171] hover:text-[#222222] hover:bg-[#F7F7F7]'
              }`}
            >
              <div className="flex items-center gap-3">
                <Icon className={`w-4 h-4 ${isActive ? 'text-[#FF385C]' : 'text-[#717171]'}`} />
                <span>{item.label}</span>
              </div>
              {item.badge && (
                <span className="text-[10px] px-2 py-0.5 rounded-full bg-white text-[#FF385C] border border-rose-200 font-mono shadow-2xs">
                  {item.badge}
                </span>
              )}
            </button>
          );
        })}
      </div>

      {/* Bottom Area: Info Banner + User Profile & Logout (Ảnh 2) */}
      <div className="space-y-3 pt-4 border-t border-[#EBEBEB]">
        {/* Spec-Driven Info Box */}
        <div className="p-3 rounded-2xl bg-[#F7F7F7] border border-[#EBEBEB] text-xs text-[#717171] space-y-1">
          <p className="font-bold text-[#222222] text-[11px]">Spec-Driven Development</p>
          <p className="text-[11px] leading-relaxed">Đồng bộ .NET 8 API, React Web & Flutter App theo tiêu chuẩn SDD.</p>
        </div>

        {/* User Profile & Logout Card - Thiết kế chuẩn theo Ảnh 2 */}
        <div className="p-3 rounded-2xl bg-white border border-[#EBEBEB] hover:border-[#DDDDDD] transition shadow-xs flex items-center justify-between gap-2.5">
          {/* Avatar & User Info */}
          <div className="flex items-center gap-2.5 min-w-0">
            <div className="w-9 h-9 rounded-full bg-emerald-50 border border-emerald-200 flex items-center justify-center text-emerald-600 shrink-0">
              <User className="w-4 h-4" />
            </div>
            <div className="min-w-0 text-left">
              <span className="block text-xs font-bold text-[#222222] leading-tight truncate">
                {user?.fullName || 'Minh Asseta Demo'}
              </span>
              <span
                title={user?.email}
                className="block text-[11px] text-[#717171] leading-tight truncate max-w-[125px]"
              >
                {user?.email || 'globalhelcurt14092005@gmail.com'}
              </span>
            </div>
          </div>

          {/* Logout Button (Ảnh 2) */}
          <button
            onClick={logout}
            title="Đăng xuất khỏi hệ thống"
            aria-label="Đăng xuất"
            className="px-2.5 py-1.5 rounded-xl bg-[#F7F7F7] hover:bg-rose-50 text-[#222222] hover:text-[#FF385C] border border-[#DDDDDD] hover:border-rose-200 text-xs font-semibold flex items-center gap-1.5 transition active:scale-95 shrink-0 cursor-pointer shadow-2xs"
          >
            <LogOut className="w-3.5 h-3.5" />
            <span>Đăng xuất</span>
          </button>
        </div>
      </div>
    </aside>
  );
};
