import React from 'react';
import { X, ClipboardList, Users } from 'lucide-react';
import { ActionCardTemplate } from '../../../types/actionCard';

interface TemplateSelectorModalProps {
  isOpen: boolean;
  templates: ActionCardTemplate[];
  loading: boolean;
  onSelect: (template: ActionCardTemplate) => void;
  onClose: () => void;
}

export const TemplateSelectorModal: React.FC<TemplateSelectorModalProps> = ({
  isOpen,
  templates,
  loading,
  onSelect,
  onClose,
}) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="flex max-h-[85vh] w-full max-w-2xl flex-col rounded-2xl border border-slate-200 bg-white shadow-2xl overflow-hidden">
        {/* Header */}
        <div className="flex items-center justify-between border-b border-slate-100 p-5">
          <div>
            <h3 className="text-lg font-bold text-slate-900">Chọn Bản Mẫu (Template) Thẻ Hành Động</h3>
            <p className="text-xs text-slate-500">Các kịch bản khẩn cấp mẫu chuẩn hóa theo thực tiễn Việt Nam</p>
          </div>
          <button
            type="button"
            onClick={onClose}
            aria-label="Đóng"
            className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-100 hover:text-slate-600 transition-colors"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* Templates Grid */}
        <div className="flex-1 space-y-3 overflow-y-auto p-5">
          {loading ? (
            <div className="py-12 text-center text-sm text-slate-500">Đang tải danh sách bản mẫu…</div>
          ) : templates.length === 0 ? (
            <div className="py-12 text-center text-sm text-slate-500">Không tìm thấy bản mẫu phù hợp.</div>
          ) : (
            templates.map((tpl) => (
              <button
                type="button"
                key={tpl.id}
                onClick={() => {
                  onSelect(tpl);
                  onClose();
                }}
                className="group w-full text-left rounded-xl border border-slate-200 bg-white p-4 transition-colors hover:border-emerald-500 hover:bg-emerald-50/40 shadow-xs"
              >
                <div className="flex items-center justify-between gap-2">
                  <span className="rounded bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-700">
                    {tpl.categoryCode}
                  </span>
                  <div className="flex gap-1.5">
                    <span className="rounded bg-amber-50 border border-amber-200 px-2 py-0.5 text-[11px] font-semibold text-amber-700">
                      {tpl.defaultUrgency}
                    </span>
                    <span className="rounded bg-red-50 border border-red-200 px-2 py-0.5 text-[11px] font-semibold text-red-700">
                      {tpl.defaultPriority}
                    </span>
                  </div>
                </div>

                <h4 className="mt-2 text-base font-semibold text-slate-900 group-hover:text-emerald-700 transition-colors">
                  {tpl.titleVi}
                </h4>
                <p className="text-xs text-slate-500">{tpl.titleEn}</p>

                {/* Preview steps count */}
                <div className="mt-3 flex items-center gap-4 text-xs text-slate-500">
                  <span className="inline-flex items-center gap-1">
                    <ClipboardList className="h-3.5 w-3.5 text-slate-400" aria-hidden="true" />
                    {tpl.suggestedSteps.length} bước gợi ý
                  </span>
                  <span className="inline-flex items-center gap-1">
                    <Users className="h-3.5 w-3.5 text-slate-400" aria-hidden="true" />
                    {tpl.suggestedRoles.length} vai trò liên hệ
                  </span>
                </div>
              </button>
            ))
          )}
        </div>
      </div>
    </div>
  );
};
