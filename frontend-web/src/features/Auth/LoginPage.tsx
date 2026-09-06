import React, { useState } from 'react';
import { Lock, Mail, ShieldCheck, ArrowRight, Sparkles, AlertCircle, Loader2 } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { getErrorMessage } from '../../utils/errorUtils';

interface LoginPageProps {
  onNavigateToRegister: () => void;
}

export const LoginPage: React.FC<LoginPageProps> = ({ onNavigateToRegister }) => {
  const { login } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email || !password) {
      setError('Vui lòng nhập đầy đủ email và mật khẩu');
      return;
    }

    setError(null);
    setIsSubmitting(true);
    try {
      await login({ email: email.trim(), password });
    } catch (err: any) {
      const msg = getErrorMessage(err, 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
      setError(msg);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleFillDemo = () => {
    setEmail('globalhelcurt14092005@gmail.com');
    setPassword('Minhdz2005@');
    setError(null);
  };

  return (
    <div className="min-h-screen bg-[#F7F7F7] flex items-center justify-center p-4 selection:bg-[#FF385C] selection:text-white">
      <div className="relative w-full max-w-md bg-white border border-[#EBEBEB] rounded-3xl shadow-[0_6px_24px_rgba(0,0,0,0.06)] p-8 sm:p-10">

        {/* Brand Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-rose-50 border border-rose-100 text-[#FF385C] mb-4 shadow-xs">
            <ShieldCheck className="w-8 h-8" />
          </div>
          <h1 className="text-2xl font-bold text-[#222222] tracking-tight">Két Quản Trị Asseta</h1>
          <p className="text-xs text-[#717171] mt-1">Hệ thống chuyển giao tài sản & kế hoạch kế thừa liên tục</p>
        </div>

        {/* Demo Account Quick Fill Pill */}
        <div className="mb-6 p-3 rounded-2xl bg-rose-50/60 border border-rose-100 flex items-center justify-between gap-3">
          <div className="flex items-center gap-2.5 text-xs text-[#222222]">
            <Sparkles className="w-4 h-4 text-[#FF385C] shrink-0" />
            <span className="font-medium">Tài khoản Demo có sẵn dữ liệu</span>
          </div>
          <button
            type="button"
            onClick={handleFillDemo}
            className="px-2.5 py-1 rounded-xl bg-white hover:bg-rose-100/50 text-[#FF385C] border border-rose-200 text-xs font-semibold transition cursor-pointer shadow-2xs"
          >
            Điền nhanh
          </button>
        </div>

        {/* Error Alert */}
        {error && (
          <div role="alert" className="mb-6 p-3.5 rounded-2xl bg-red-50 border border-red-200 text-red-700 text-xs flex items-center gap-2.5">
            <AlertCircle className="w-4 h-4 text-red-500 shrink-0" />
            <span>{error}</span>
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="email" className="block text-xs font-semibold text-[#222222] mb-1.5">Email</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-[#717171]">
                <Mail className="w-4 h-4" />
              </div>
              <input
                id="email"
                name="email"
                type="email"
                autoComplete="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="name@example.com"
                required
                className="w-full pl-10 pr-4 py-2.5 bg-[#F7F7F7] border border-[#DDDDDD] rounded-xl text-[#222222] placeholder-[#717171]/60 text-sm focus:outline-none focus:bg-white focus:border-[#222222] focus-visible:ring-1 focus-visible:ring-[#222222] transition"
              />
            </div>
          </div>

          <div>
            <label htmlFor="password" className="block text-xs font-semibold text-[#222222] mb-1.5">Mật khẩu</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-[#717171]">
                <Lock className="w-4 h-4" />
              </div>
              <input
                id="password"
                name="password"
                type="password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                required
                className="w-full pl-10 pr-4 py-2.5 bg-[#F7F7F7] border border-[#DDDDDD] rounded-xl text-[#222222] placeholder-[#717171]/60 text-sm focus:outline-none focus:bg-white focus:border-[#222222] focus-visible:ring-1 focus-visible:ring-[#222222] transition"
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full mt-2 py-3 px-4 rounded-xl bg-[#FF385C] hover:bg-[#E00B41] active:scale-[0.99] text-white font-semibold text-sm transition shadow-xs flex items-center justify-center gap-2 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSubmitting ? (
              <>
                <Loader2 className="w-4 h-4 animate-spin" />
                <span>Đang xác thực…</span>
              </>
            ) : (
              <>
                <span>Đăng nhập</span>
                <ArrowRight className="w-4 h-4" />
              </>
            )}
          </button>
        </form>

        {/* Switch to Register */}
        <div className="mt-8 pt-6 border-t border-[#EBEBEB] text-center">
          <p className="text-xs text-[#717171]">
            Chưa có tài khoản két Asseta?{' '}
            <button
              type="button"
              onClick={onNavigateToRegister}
              className="text-[#FF385C] hover:underline font-semibold transition underline-offset-4 cursor-pointer"
            >
              Đăng ký ngay
            </button>
          </p>
        </div>

      </div>
    </div>
  );
};
