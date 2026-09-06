import React, { useState } from 'react';
import { X, UserPlus, Shield } from 'lucide-react';
import { TrustedPerson, CreateTrustedPersonInput, UpdateTrustedPersonInput } from '../../../types/trustedPeople';

interface TrustedPersonModalProps {
  person?: TrustedPerson | null;
  onClose: () => void;
  onSubmit: (data: CreateTrustedPersonInput | UpdateTrustedPersonInput) => Promise<void>;
}

export const TrustedPersonModal: React.FC<TrustedPersonModalProps> = ({
  person,
  onClose,
  onSubmit,
}) => {
  const [fullName, setFullName] = useState(person?.fullName || '');
  const [email, setEmail] = useState(person?.email || '');
  const [phoneNumber, setPhoneNumber] = useState(person?.phoneNumber || '');
  const [relationship, setRelationship] = useState(person?.relationship || '');
  const [roleDescription, setRoleDescription] = useState(person?.roleDescription || '');
  const [trustLevel, setTrustLevel] = useState<number>(person?.trustLevel || 2);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      if (person) {
        await onSubmit({
          fullName,
          email,
          phoneNumber,
          relationship,
          roleDescription: roleDescription || null,
          trustLevel,
          expectedRowVersion: person.rowVersion,
        } as UpdateTrustedPersonInput);
      } else {
        await onSubmit({
          fullName,
          email,
          phoneNumber,
          relationship,
          roleDescription: roleDescription || null,
          trustLevel,
        } as CreateTrustedPersonInput);
      }
      onClose();
    } catch (err: any) {
      setError(err?.response?.data?.error?.message || err.message || 'Lỗi khi lưu Người Ủy Thác.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4">
      <div className="bg-white border border-slate-200 rounded-2xl max-w-lg w-full p-6 space-y-5 shadow-2xl relative">
        <button
          type="button"
          onClick={onClose}
          aria-label="Đóng"
          className="absolute top-4 right-4 text-slate-400 hover:text-slate-600 p-1 rounded-lg hover:bg-slate-100 transition-colors"
        >
          <X className="w-5 h-5" />
        </button>

        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center border border-emerald-100 shadow-xs">
            <UserPlus className="w-5 h-5" />
          </div>
          <div>
            <h3 className="text-lg font-bold text-slate-900">
              {person ? 'Chỉnh Sửa Người Ủy Thác' : 'Thêm Người Ủy Thác Mới'}
            </h3>
            <p className="text-xs text-slate-500">
              {person ? 'Cập nhật thông tin liên hệ và cấp bậc ủy quyền' : 'Chỉ định người nhận quyền tiếp quản trong trường hợp khẩn cấp'}
            </p>
          </div>
        </div>

        {error && (
          <div className="p-3 rounded-lg bg-red-50 border border-red-200 text-red-700 text-xs">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="tp-fullname" className="block text-xs font-medium text-slate-700 mb-1">
              Họ và tên *
            </label>
            <input
              id="tp-fullname"
              type="text"
              required
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              placeholder="VD: Nguyễn Văn B"
              className="w-full px-3.5 py-2.5 rounded-lg bg-slate-50 border border-slate-300 text-slate-900 text-sm placeholder-slate-400 focus:bg-white focus:border-emerald-500 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label htmlFor="tp-email" className="block text-xs font-medium text-slate-700 mb-1">
                Email *
              </label>
              <input
                id="tp-email"
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="email@example.com"
                className="w-full px-3.5 py-2.5 rounded-lg bg-slate-50 border border-slate-300 text-slate-900 text-sm placeholder-slate-400 focus:bg-white focus:border-emerald-500 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
              />
            </div>
            <div>
              <label htmlFor="tp-phone" className="block text-xs font-medium text-slate-700 mb-1">
                Số điện thoại *
              </label>
              <input
                id="tp-phone"
                type="tel"
                required
                value={phoneNumber}
                onChange={(e) => setPhoneNumber(e.target.value)}
                placeholder="0901234567"
                className="w-full px-3.5 py-2.5 rounded-lg bg-slate-50 border border-slate-300 text-slate-900 text-sm placeholder-slate-400 focus:bg-white focus:border-emerald-500 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label htmlFor="tp-relation" className="block text-xs font-medium text-slate-700 mb-1">
                Mối quan hệ *
              </label>
              <input
                id="tp-relation"
                type="text"
                required
                value={relationship}
                onChange={(e) => setRelationship(e.target.value)}
                placeholder="VD: Vợ/chồng, Luật sư, CFO..."
                className="w-full px-3.5 py-2.5 rounded-lg bg-slate-50 border border-slate-300 text-slate-900 text-sm placeholder-slate-400 focus:bg-white focus:border-emerald-500 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
              />
            </div>

            <div>
              <label htmlFor="tp-trustlevel" className="block text-xs font-medium text-slate-700 mb-1">
                Cấp bậc tin cậy (Trust Level) *
              </label>
              <select
                id="tp-trustlevel"
                value={trustLevel}
                onChange={(e) => setTrustLevel(Number(e.target.value))}
                className="w-full px-3.5 py-2.5 rounded-lg bg-slate-50 border border-slate-300 text-slate-900 text-sm focus:bg-white focus:border-emerald-500 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2"
              >
                <option value={1}>Level 1: Notice Only (Chỉ nhận tin báo)</option>
                <option value={2}>Level 2: Scoped Delegate (Phân quyền thẻ/danh mục)</option>
                <option value={3}>Level 3: Primary Delegate (Đại diện chính)</option>
              </select>
            </div>
          </div>

          <div>
            <label htmlFor="tp-roledesc" className="block text-xs font-medium text-slate-700 mb-1">
              Mô tả vai trò phụ trách
            </label>
            <textarea
              id="tp-roledesc"
              rows={2}
              value={roleDescription}
              onChange={(e) => setRoleDescription(e.target.value)}
              placeholder="VD: Phụ trách nghĩa vụ tài chính gia đình và học phí cho con..."
              className="w-full px-3.5 py-2.5 rounded-lg bg-slate-50 border border-slate-300 text-slate-900 text-sm placeholder-slate-400 focus:bg-white focus:border-emerald-500 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2 resize-none"
            />
          </div>

          <div className="p-3 rounded-lg bg-slate-50 border border-slate-200 flex items-start gap-2.5 text-xs text-slate-600">
            <Shield className="w-4 h-4 text-emerald-600 shrink-0 mt-0.5" />
            <span>
              <strong>Zero-Disclosure:</strong> Người này sẽ không thể xem nội dung thẻ hoặc hồ sơ nhạy cảm cho đến khi Safe Activation được kích hoạt.
            </span>
          </div>

          <div className="flex items-center justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded-lg bg-slate-100 hover:bg-slate-200 text-slate-700 text-sm font-medium transition-colors"
            >
              Hủy
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-5 py-2 rounded-lg bg-emerald-600 hover:bg-emerald-500 disabled:opacity-50 text-white text-sm font-medium transition-colors shadow-xs"
            >
              {loading ? 'Đang lưu…' : person ? 'Cập nhật' : 'Thêm & Sinh mã ghép đôi'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
