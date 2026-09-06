import React, { useState } from 'react';
import { Lock, Mail, User as UserIcon, Phone, ShieldCheck, ArrowRight, AlertCircle, Loader2, KeyRound } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { getErrorMessage } from '../../utils/errorUtils';

interface RegisterPageProps {
  onNavigateToLogin: () => void;
}

export const RegisterPage: React.FC<RegisterPageProps> = ({ onNavigateToLogin }) => {
  const { register } = useAuth();
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!fullName.trim() || !email.trim() || !password) {
      setError('Vui lòng điền đầy đủ các thông tin bắt buộc (*)');
      return;
    }

    if (password.length < 6) {
      setError('Mật khẩu phải có ít nhất 6 ký tự');
      return;
    }

    if (password !== confirmPassword) {
      setError('Mật khẩu xác nhận không khớp');
      return;
    }

    setError(null);
    setIsSubmitting(true);
    try {
      await register({
        email: email.trim(),
        password,
        fullName: fullName.trim(),
        phoneNumber: phoneNumber.trim() || undefined,
      });
    } catch (err: any) {
      const msg = getErrorMessage(err, 'Đăng ký thất bại. Vui lòng thử lại.');
      setError(msg);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-[#F7F7F7] flex items-center justify-center p-4 selection:bg-[#FF385C] selection:text-white">
      <div className="relative w-full max-w-lg bg-white border border-[#EBEBEB] rounded-3xl shadow-[0_6px_24px_rgba(0,0,0,0.06)] p-8 sm:p-10">

        {/* Brand Header */}
        <div className="text-center mb-6">
          <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-rose-50 border border-rose-100 text-[#FF385C] mb-4 shadow-xs">
            <ShieldCheck className="w-8 h-8" />
          </div>
          <h1 className="text-2xl font-bold text-[#222222] tracking-tight">Tạo Két Quản Trị Mới</h1>
          <p className="text-xs text-[#717171] mt-1">Đăng ký tài khoản để thiết lập bản đồ tài sản liên tục và kế hoạch kế thừa</p>
        </div>

        {/* Master Key Notice Card */}
        <div className="mb-6 p-3.5 rounded-2xl bg-rose-50/60 border border-rose-100 flex items-start gap-3">
          <KeyRound className="w-5 h-5 text-[#FF385C] shrink-0 mt-0.5" />
          <p className="text-xs text-[#222222] leading-relaxed">
            Hệ thống sẽ tự động tạo một <strong className="text-[#FF385C]">Master Key</strong> duy nhất bất biến và gửi tới email của bạn để bảo vệ két an toàn.
          </p>
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
            <label htmlFor="fullName" className="block text-xs font-semibold text-[#222222] mb-1.5">
              Họ và tên <span className="text-[#FF385C]">*</span>
            </label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-[#717171]">
                <UserIcon className="w-4 h-4" />
              </div>
              <input
                id="fullName"
                name="fullName"
                type="text"
                autoComplete="name"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                placeholder="Nguyễn Văn A"
                required
                className="w-full pl-10 pr-4 py-2.5 bg-[#F7F7F7] border border-[#DDDDDD] rounded-xl text-[#222222] placeholder-[#717171]/60 text-sm focus:outline-none focus:bg-white focus:border-[#222222] focus-visible:ring-1 focus-visible:ring-[#222222] transition"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label htmlFor="reg-email" className="block text-xs font-semibold text-[#222222] mb-1.5">
                Email <span className="text-[#FF385C]">*</span>
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-[#717171]">
                  <Mail className="w-4 h-4" />
                </div>
                <input
                  id="reg-email"
                  name="email"
                  type="email"
                  autoComplete="email"
                  spellCheck={false}
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="name@example.com"
                  required
                  className="w-full pl-10 pr-4 py-2.5 bg-[#F7F7F7] border border-[#DDDDDD] rounded-xl text-[#222222] placeholder-[#717171]/60 text-sm focus:outline-none focus:bg-white focus:border-[#222222] focus-visible:ring-1 focus-visible:ring-[#222222] transition"
                />
              </div>
            </div>

            <div>
              <label htmlFor="phoneNumber" className="block text-xs font-semibold text-[#222222] mb-1.5">Số điện thoại (tùy chọn)</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-[#717171]">
                  <Phone className="w-4 h-4" />
                </div>
                <input
                  id="phoneNumber"
                  name="phoneNumber"
                  type="tel"
                  autoComplete="tel"
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                  placeholder="0912 345 678"
                  className="w-full pl-10 pr-4 py-2.5 bg-[#F7F7F7] border border-[#DDDDDD] rounded-xl text-[#222222] placeholder-[#717171]/60 text-sm focus:outline-none focus:bg-white focus:border-[#222222] focus-visible:ring-1 focus-visible:ring-[#222222] transition"
                />
              </div>
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label htmlFor="reg-password" className="block text-xs font-semibold text-[#222222] mb-1.5">
                Mật khẩu <span className="text-[#FF385C]">*</span>
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-[#717171]">
                  <Lock className="w-4 h-4" />
                </div>
                <input
                  id="reg-password"
                  name="password"
                  type="password"
                  autoComplete="new-password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="Tối thiểu 6 ký tự"
                  required
                  className="w-full pl-10 pr-4 py-2.5 bg-[#F7F7F7] border border-[#DDDDDD] rounded-xl text-[#222222] placeholder-[#717171]/60 text-sm focus:outline-none focus:bg-white focus:border-[#222222] focus-visible:ring-1 focus-visible:ring-[#222222] transition"
                />
              </div>
            </div>

            <div>
              <label htmlFor="confirmPassword" className="block text-xs font-semibold text-[#222222] mb-1.5">
                Xác nhận mật khẩu <span className="text-[#FF385C]">*</span>
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-[#717171]">
                  <Lock className="w-4 h-4" />
                </div>
                <input
                  id="confirmPassword"
                  name="confirmPassword"
                  type="password"
                  autoComplete="new-password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  placeholder="Nhập lại mật khẩu"
                  required
                  className="w-full pl-10 pr-4 py-2.5 bg-[#F7F7F7] border border-[#DDDDDD] rounded-xl text-[#222222] placeholder-[#717171]/60 text-sm focus:outline-none focus:bg-white focus:border-[#222222] focus-visible:ring-1 focus-visible:ring-[#222222] transition"
                />
              </div>
            </div>
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full mt-4 py-3 px-4 rounded-xl bg-[#FF385C] hover:bg-[#E00B41] active:scale-[0.99] text-white font-semibold text-sm transition shadow-xs flex items-center justify-center gap-2 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSubmitting ? (
              <>
                <Loader2 className="w-4 h-4 animate-spin" />
                <span>Đang khởi tạo két bảo mật…</span>
              </>
            ) : (
              <>
                <span>Khởi tạo tài khoản & Sinh Master Key</span>
                <ArrowRight className="w-4 h-4" />
              </>
            )}
          </button>
        </form>

        {/* Switch to Login */}
        <div className="mt-8 pt-6 border-t border-[#EBEBEB] text-center">
          <p className="text-xs text-[#717171]">
            Đã có tài khoản két Asseta?{' '}
            <button
              type="button"
              onClick={onNavigateToLogin}
              className="text-[#FF385C] hover:underline font-semibold transition underline-offset-4 cursor-pointer"
            >
              Đăng nhập ngay
            </button>
          </p>
        </div>

      </div>
    </div>
  );
};
