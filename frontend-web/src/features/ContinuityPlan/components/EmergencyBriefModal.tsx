import React, { useEffect, useState } from 'react';
import { X, Printer, Phone, MapPin, CheckSquare, ShieldCheck } from 'lucide-react';
import { OfflineEmergencyBrief } from '../../../types/continuityPlan';

interface EmergencyBriefModalProps {
  isOpen: boolean;
  onClose: () => void;
  fetchBrief: () => Promise<OfflineEmergencyBrief>;
}

export const EmergencyBriefModal: React.FC<EmergencyBriefModalProps> = ({
  isOpen,
  onClose,
  fetchBrief,
}) => {
  const [brief, setBrief] = useState<OfflineEmergencyBrief | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (isOpen) {
      setLoading(true);
      fetchBrief()
        .then((data) => setBrief(data))
        .catch((err) => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [isOpen, fetchBrief]);

  if (!isOpen) return null;

  const handlePrint = () => {
    window.print();
  };

  return (
    <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4 overflow-y-auto">
      <div className="bg-white border border-slate-200 rounded-2xl w-full max-w-4xl max-h-[90vh] flex flex-col shadow-2xl overflow-hidden print:m-0 print:border-none print:shadow-none print:max-w-none print:max-h-none">
        {/* Modal Header */}
        <div className="p-6 border-b border-slate-100 flex items-center justify-between print:hidden">
          <div>
            <h3 className="text-xl font-bold text-slate-900 flex items-center gap-2">
              <ShieldCheck className="w-5 h-5 text-emerald-600" />
              Bản Tóm Lược Kế Hoạch Tiếp Quản Khẩn Cấp (Offline Emergency Brief)
            </h3>
            <p className="text-xs text-slate-500 mt-1">
              Bản trích xuất an toàn chuẩn Zero-Knowledge dùng để xem offline, gửi cho người thân hoặc in ấn cất vào két sắt.
            </p>
          </div>
          <div className="flex items-center gap-2">
            <button
              type="button"
              onClick={handlePrint}
              className="px-3 py-1.5 bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold rounded-lg flex items-center gap-1.5 transition-colors shadow-xs"
            >
              <Printer className="w-4 h-4" /> In Ra Giấy / Lưu PDF
            </button>
            <button
              type="button"
              onClick={onClose}
              aria-label="Đóng"
              className="p-1.5 text-slate-400 hover:text-slate-600 rounded-lg hover:bg-slate-100 transition-colors"
            >
              <X className="w-5 h-5" />
            </button>
          </div>
        </div>

        {/* Modal Content */}
        <div className="p-6 overflow-y-auto space-y-6 flex-1 text-slate-800">
          {loading ? (
            <div className="py-12 text-center text-sm text-slate-500">Đang tổng hợp dữ liệu tóm lược…</div>
          ) : brief ? (
            <>
              {/* Document Header */}
              <div className="border-b border-slate-200 pb-4">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="text-2xl font-black text-slate-900 tracking-tight">ASSETA CONTINUITY EMERGENCY BRIEF</h3>
                    <p className="text-xs text-slate-500 mt-0.5">Kế Hoạch Tiếp Quản Và Điều Phối Khẩn Cấp Cá Nhân</p>
                  </div>
                  <div className="text-right text-xs text-slate-500">
                    <div>Ngày khởi tạo: {new Date(brief.generatedAtUtc).toLocaleDateString('vi-VN')}</div>
                    <div className="text-emerald-700 font-semibold mt-0.5">Cơ chế bảo mật: Zero-Knowledge Safe</div>
                  </div>
                </div>
              </div>

              {/* Primary Contacts Directory */}
              <div className="p-4 rounded-xl bg-slate-50 border border-slate-200 space-y-3">
                <h4 className="text-xs font-bold uppercase tracking-wider text-sky-800 flex items-center gap-2">
                  <Phone className="w-4 h-4 text-sky-600" /> Danh Bạ Đầu Mối Liên Hệ Khẩn Cấp (Quick Contacts Directory)
                </h4>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
                  {brief.primaryContacts.map((c, idx) => (
                    <div key={idx} className="p-2.5 rounded-lg bg-white border border-slate-200 text-xs shadow-xs">
                      <div className="font-semibold text-slate-900">{c.name}</div>
                      <div className="text-slate-500 text-[11px]">{c.roleOrRelationship || 'Liên hệ'}</div>
                      <div className="text-sky-700 font-mono mt-1 flex items-center gap-1">
                        <Phone className="w-3 h-3 text-slate-400" /> {c.phone || 'Chưa cập nhật SĐT'}
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Timeline Stages */}
              <div className="space-y-6">
                {brief.stages.map((stage) => (
                  <div key={stage.stage} className="space-y-3">
                    <h4 className="text-sm font-bold text-slate-900 border-l-4 border-emerald-500 pl-3 uppercase tracking-wide">
                      {stage.stageName} ({stage.actionItems.length} hạng mục)
                    </h4>

                    {stage.actionItems.length === 0 ? (
                      <p className="text-xs text-slate-400 italic pl-3">Không có việc khẩn cấp trong mốc thời gian này.</p>
                    ) : (
                      <div className="space-y-3 pl-3">
                        {stage.actionItems.map((item) => (
                          <div key={item.id} className="p-4 rounded-xl bg-slate-50 border border-slate-200 space-y-2">
                            <div className="flex items-start justify-between">
                              <h5 className="font-semibold text-slate-900 text-sm">{item.title}</h5>
                              <span className="px-2 py-0.5 text-[10px] font-bold bg-slate-200 text-slate-700 rounded">
                                {item.priority}
                              </span>
                            </div>

                            <div className="grid grid-cols-1 md:grid-cols-2 gap-2 text-xs text-slate-700 pt-1">
                              <div>
                                <span className="text-slate-500">Người phụ trách: </span>
                                <span className="font-medium text-slate-900">{item.delegateName || 'Chưa chỉ định'}</span>
                                {item.delegatePhone && <span className="text-slate-500 ml-1">({item.delegatePhone})</span>}
                              </div>
                              {item.documentLocationHint && (
                                <div className="flex items-center gap-1">
                                  <MapPin className="w-3.5 h-3.5 text-emerald-600 shrink-0" />
                                  <span className="text-slate-500">Hồ sơ: </span>
                                  <span className="font-medium text-slate-800">{item.documentLocationHint}</span>
                                </div>
                              )}
                            </div>

                            {/* Steps */}
                            {item.keySteps.length > 0 && (
                              <div className="pt-2 border-t border-slate-200">
                                <div className="text-[11px] font-semibold text-slate-500 mb-1 flex items-center gap-1">
                                  <CheckSquare className="w-3 h-3 text-emerald-600" /> Các bước hành động:
                                </div>
                                <ol className="list-decimal list-inside text-xs space-y-1 text-slate-700">
                                  {item.keySteps.map((step, sIdx) => (
                                    <li key={sIdx}>{step}</li>
                                  ))}
                                </ol>
                              </div>
                            )}
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </>
          ) : (
            <div className="py-8 text-center text-rose-600">Không tìm thấy dữ liệu tóm lược khẩn cấp.</div>
          )}
        </div>
      </div>
    </div>
  );
};
