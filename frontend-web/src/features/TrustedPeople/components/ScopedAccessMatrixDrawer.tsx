import React, { useState } from 'react';
import { X, Layers, CheckSquare, Square, ShieldCheck } from 'lucide-react';
import { TrustedPerson, CategoryPermissionInput, ActionCardPermissionInput } from '../../../types/trustedPeople';

interface ScopedAccessMatrixDrawerProps {
  person: TrustedPerson;
  onClose: () => void;
  onSave: (
    categoryPermissions: CategoryPermissionInput[],
    actionCardPermissions: ActionCardPermissionInput[]
  ) => Promise<void>;
}

// 6 danh mục chuẩn của Asseta
const DEFAULT_CATEGORIES = [
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e01', code: 'FINANCIAL', nameVi: 'Tài chính & Ngân hàng', desc: 'Tài khoản, khoản vay, thẻ tín dụng, nghĩa vụ định kỳ' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e02', code: 'PROPERTY', nameVi: 'Bất động sản & Xe cộ', desc: 'Nhà ở, căn hộ cho thuê, phương tiện đi lại' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e03', code: 'INSURANCE', nameVi: 'Bảo hiểm nhân thọ & Sức khỏe', desc: 'Hợp đồng bảo hiểm, quyền lợi bồi thường' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e04', code: 'BUSINESS', nameVi: 'Vận hành Công ty & Đối tác', desc: 'Doanh nghiệp, khách hàng, nhân sự chủ chốt' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e05', code: 'DOCUMENTS', nameVi: 'Hồ sơ Pháp lý & Hợp đồng', desc: 'Giấy tờ tài sản, chứng nhận pháp lý' },
  { id: '018e6e5a-7341-789a-9e12-2d93e1104e06', code: 'FAMILY', nameVi: 'Gia đình & Người phụ thuộc', desc: 'Học phí con cái, người cần chăm sóc đặc biệt' },
];

export const ScopedAccessMatrixDrawer: React.FC<ScopedAccessMatrixDrawerProps> = ({
  person,
  onClose,
  onSave,
}) => {
  // Lấy các categoryId đã được phân quyền canView = true
  const initialCategoryIds = new Set(
    person.permissions
      .filter((p) => p.permissionType === 'Category' && p.canView && p.targetCategoryId)
      .map((p) => p.targetCategoryId!)
  );

  const [selectedCategoryIds, setSelectedCategoryIds] = useState<Set<string>>(initialCategoryIds);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const toggleCategory = (categoryId: string) => {
    const updated = new Set(selectedCategoryIds);
    if (updated.has(categoryId)) {
      updated.delete(categoryId);
    } else {
      updated.add(categoryId);
    }
    setSelectedCategoryIds(updated);
  };

  const handleSave = async () => {
    setLoading(true);
    setError(null);
    try {
      const categoryInputs: CategoryPermissionInput[] = Array.from(selectedCategoryIds).map((catId) => ({
        categoryId: catId,
        canView: true,
      }));
      await onSave(categoryInputs, []);
      onClose();
    } catch (err: any) {
      setError(err?.response?.data?.error?.message || err.message || 'Lỗi khi cập nhật ma trận phân quyền.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-end bg-black/40 backdrop-blur-sm">
      <div className="bg-white border-l border-slate-200 w-full max-w-md h-full flex flex-col shadow-2xl">
        <div className="p-6 border-b border-slate-100 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-purple-50 text-purple-600 border border-purple-100 flex items-center justify-center shadow-xs">
              <Layers className="w-5 h-5" />
            </div>
            <div>
              <h3 className="text-lg font-bold text-slate-900">Ma Trận Phân Quyền</h3>
              <p className="text-xs text-slate-500">
                Ủy quyền cho: <strong className="text-slate-900">{person.fullName}</strong> ({person.relationship})
              </p>
            </div>
          </div>
          <button
            type="button"
            onClick={onClose}
            aria-label="Đóng"
            className="text-slate-400 hover:text-slate-600 p-1.5 rounded-lg hover:bg-slate-100 transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="p-6 flex-1 overflow-y-auto space-y-4">
          <div className="p-3 rounded-xl bg-slate-50 border border-slate-200 flex items-start gap-2.5 text-xs text-slate-600">
            <ShieldCheck className="w-4 h-4 text-emerald-600 shrink-0 mt-0.5" />
            <span>
              <strong>Nguyên tắc Need-to-Know:</strong> Người ủy thác chỉ được cấp quyền xem các danh mục được tích chọn dưới đây khi kích hoạt khẩn cấp.
            </span>
          </div>

          {error && (
            <div className="p-3 rounded-lg bg-red-50 border border-red-200 text-red-700 text-xs">
              {error}
            </div>
          )}

          <div className="space-y-3">
            <h4 className="text-xs font-semibold uppercase tracking-wider text-slate-500">
              Phân quyền theo Danh Mục ({selectedCategoryIds.size}/6 danh mục)
            </h4>

            {DEFAULT_CATEGORIES.map((cat) => {
              const isSelected = selectedCategoryIds.has(cat.id);
              return (
                <button
                  type="button"
                  key={cat.id}
                  onClick={() => toggleCategory(cat.id)}
                  className={`w-full text-left p-4 rounded-xl border transition-colors flex items-start gap-3.5 select-none shadow-xs ${isSelected
                      ? 'bg-purple-50/70 border-purple-300 text-slate-900'
                      : 'bg-white border-slate-200 text-slate-700 hover:border-slate-300 hover:bg-slate-50'
                    }`}
                >
                  <div className="mt-0.5 text-purple-600">
                    {isSelected ? <CheckSquare className="w-5 h-5" /> : <Square className="w-5 h-5 text-slate-400" />}
                  </div>
                  <div className="flex-1">
                    <div className="text-sm font-semibold text-slate-900">{cat.nameVi}</div>
                    <div className="text-xs text-slate-500 mt-0.5">{cat.desc}</div>
                  </div>
                </button>
              );
            })}
          </div>
        </div>

        <div className="p-6 border-t border-slate-100 bg-slate-50 flex items-center justify-end gap-3">
          <button
            type="button"
            onClick={onClose}
            className="px-4 py-2.5 rounded-lg bg-white border border-slate-300 hover:bg-slate-100 text-slate-700 text-sm font-medium transition-colors"
          >
            Hủy
          </button>
          <button
            type="button"
            disabled={loading}
            onClick={handleSave}
            className="px-5 py-2.5 rounded-lg bg-purple-600 hover:bg-purple-500 disabled:opacity-50 text-white text-sm font-medium transition-colors shadow-xs"
          >
            {loading ? 'Đang lưu…' : 'Lưu Ma Trận Quyền'}
          </button>
        </div>
      </div>
    </div>
  );
};
