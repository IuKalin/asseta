import React, { useState } from 'react';
import {
  UserPlus,
  ShieldCheck,
  Key,
  Layers,
  Edit2,
  Trash2,
  RefreshCw,
  Clock,
  AlertTriangle,
} from 'lucide-react';
import { useTrustedPeople } from './hooks/useTrustedPeople';
import { TrustedPerson, CreateTrustedPersonInput, UpdateTrustedPersonInput } from '../../types/trustedPeople';
import { TrustedPersonModal } from './components/TrustedPersonModal';
import { PairingCodeModal } from './components/PairingCodeModal';
import { ScopedAccessMatrixDrawer } from './components/ScopedAccessMatrixDrawer';

export const TrustedPeopleList: React.FC = () => {
  const {
    people,
    loading,
    error,
    refresh,
    createPerson,
    updatePerson,
    revokePerson,
    regeneratePairingCode,
    updatePermissions,
  } = useTrustedPeople();

  const [selectedPerson, setSelectedPerson] = useState<TrustedPerson | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [pairingModalData, setPairingModalData] = useState<{ person: TrustedPerson; code: string } | null>(null);
  const [matrixDrawerPerson, setMatrixDrawerPerson] = useState<TrustedPerson | null>(null);

  const handleOpenCreate = () => {
    setSelectedPerson(null);
    setIsModalOpen(true);
  };

  const handleOpenEdit = (person: TrustedPerson) => {
    setSelectedPerson(person);
    setIsModalOpen(true);
  };

  const handleModalSubmit = async (data: CreateTrustedPersonInput | UpdateTrustedPersonInput) => {
    if (selectedPerson) {
      await updatePerson(selectedPerson.id, data as UpdateTrustedPersonInput);
    } else {
      const created = await createPerson(data as CreateTrustedPersonInput);
      if (created.activePairingCode) {
        setPairingModalData({ person: created, code: created.activePairingCode });
      }
    }
  };

  const handleRegenerateCode = async (person: TrustedPerson) => {
    const updated = await regeneratePairingCode(person.id);
    if (updated.activePairingCode) {
      setPairingModalData({ person: updated, code: updated.activePairingCode });
    }
  };

  const handleRevoke = async (person: TrustedPerson) => {
    if (confirm(`Bạn có chắc chắn muốn thu hồi quyền của ${person.fullName}? Các liên kết đến thẻ và danh mục sẽ được gỡ bỏ an toàn.`)) {
      await revokePerson(person.id);
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Active':
        return (
          <span className="px-2.5 py-0.5 rounded-full text-xs font-medium bg-emerald-50 text-emerald-700 border border-emerald-200 flex items-center gap-1">
            <ShieldCheck className="w-3 h-3" /> Đã kết nối
          </span>
        );
      case 'Invited':
        return (
          <span className="px-2.5 py-0.5 rounded-full text-xs font-medium bg-amber-50 text-amber-700 border border-amber-200 flex items-center gap-1">
            <Clock className="w-3 h-3" /> Đang chờ ghép đôi
          </span>
        );
      case 'Suspended':
        return (
          <span className="px-2.5 py-0.5 rounded-full text-xs font-medium bg-slate-100 text-slate-600 border border-slate-200">
            Tạm ngưng
          </span>
        );
      default:
        return (
          <span className="px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-50 text-red-700 border border-red-200">
            Đã thu hồi
          </span>
        );
    }
  };

  const getTrustLevelLabel = (level: number) => {
    switch (level) {
      case 1:
        return 'Level 1: Notice Only (Chỉ nhận tin báo)';
      case 2:
        return 'Level 2: Scoped Delegate (Phân quyền theo thẻ)';
      case 3:
        return 'Level 3: Primary Delegate (Tiếp quản toàn quyền)';
      default:
        return `Level ${level}`;
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-slate-900 tracking-tight">Mạng Lưới Người Ủy Thác</h2>
          <p className="text-sm text-slate-600 mt-1">
            Chỉ định người đáng tin cậy và thiết lập ma trận phân quyền tối thiểu theo nguyên tắc Need-to-Know.
          </p>
        </div>

        <div className="flex items-center gap-3">
          <span className="text-xs font-mono text-slate-700 px-3 py-1.5 rounded-lg bg-slate-100 border border-slate-200 tabular-nums">
            {people.length} / 5 Người Ủy Thác
          </span>
          <button
            onClick={handleOpenCreate}
            disabled={people.length >= 5}
            className="inline-flex items-center gap-2 px-4 py-2 rounded-xl bg-emerald-600 hover:bg-emerald-500 disabled:opacity-50 text-white text-sm font-medium transition-colors shadow-xs"
          >
            <UserPlus className="w-4 h-4" /> Thêm Người Ủy Thác
          </button>
        </div>
      </div>

      {error && (
        <div className="p-4 rounded-xl bg-red-50 border border-red-200 flex items-center justify-between text-red-700 text-sm">
          <div className="flex items-center gap-2">
            <AlertTriangle className="w-4 h-4" />
            <span>{error}</span>
          </div>
          <button onClick={refresh} className="underline hover:text-red-900 text-xs">
            Thử lại
          </button>
        </div>
      )}

      {loading && people.length === 0 ? (
        <div className="p-12 text-center text-slate-500 text-sm">
          <RefreshCw className="w-6 h-6 animate-spin mx-auto mb-2 text-slate-400 motion-reduce:animate-none" />
          Đang tải danh sách người ủy thác…
        </div>
      ) : people.length === 0 ? (
        <div className="p-12 rounded-2xl bg-white border border-dashed border-slate-200 shadow-sm text-center space-y-3">
          <div className="w-12 h-12 rounded-full bg-slate-100 text-slate-500 flex items-center justify-center mx-auto">
            <UserPlus className="w-6 h-6" />
          </div>
          <h3 className="font-semibold text-slate-900">Chưa có người ủy thác nào</h3>
          <p className="text-xs text-slate-500 max-w-md mx-auto">
            Hãy bắt đầu thêm từ 1 đến 3 người đáng tin cậy (vợ/chồng, con cái, cộng sự, luật sư) để sẵn sàng tiếp nhận hướng dẫn khi xảy ra sự cố.
          </p>
          <button
            onClick={handleOpenCreate}
            className="inline-flex items-center gap-2 px-4 py-2 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white text-sm font-medium transition-colors shadow-xs"
          >
            <UserPlus className="w-4 h-4" /> Thêm người đầu tiên
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
          {people.map((p) => {
            return (
              <div
                key={p.id}
                className="p-5 rounded-2xl bg-white border border-slate-200 hover:border-slate-300 hover:shadow-md transition-colors flex flex-col justify-between space-y-4 shadow-sm"
              >
                <div className="space-y-3">
                  <div className="flex items-start justify-between">
                    <div className="flex items-center gap-3">
                      <div className="w-11 h-11 rounded-xl bg-emerald-50 border border-emerald-200 text-emerald-700 font-bold flex items-center justify-center text-lg shadow-xs">
                        {p.fullName.charAt(0)}
                      </div>
                      <div>
                        <h4 className="font-semibold text-slate-900 text-base leading-tight">{p.fullName}</h4>
                        <span className="text-xs text-emerald-600 font-medium">{p.relationship}</span>
                      </div>
                    </div>
                    {getStatusBadge(p.status)}
                  </div>

                  <div className="space-y-1.5 text-xs text-slate-500 border-t border-slate-100 pt-3">
                    <div className="flex justify-between">
                      <span>Email:</span>
                      <span className="text-slate-800 font-mono">{p.email}</span>
                    </div>
                    <div className="flex justify-between">
                      <span>SĐT:</span>
                      <span className="text-slate-800 font-mono">{p.phoneNumber}</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Cấp bậc:</span>
                      <span className="text-slate-800 truncate max-w-[170px]" title={getTrustLevelLabel(p.trustLevel)}>
                        Level {p.trustLevel}
                      </span>
                    </div>
                    {p.roleDescription && (
                      <div className="mt-2 p-2 rounded-lg bg-slate-50 border border-slate-200/80 text-slate-600 italic">
                        "{p.roleDescription}"
                      </div>
                    )}
                  </div>
                </div>

                {/* Actions */}
                <div className="border-t border-slate-100 pt-3 flex items-center justify-between gap-2">
                  <div className="flex items-center gap-1.5">
                    {p.status === 'Invited' ? (
                      <button
                        onClick={() => handleRegenerateCode(p)}
                        className="p-2 rounded-lg bg-amber-50 text-amber-700 border border-amber-200 hover:bg-amber-100 text-xs font-medium flex items-center gap-1 transition-colors"
                        title="Xem hoặc cấp lại mã ghép đôi"
                      >
                        <Key className="w-3.5 h-3.5" /> Mã kết nối
                      </button>
                    ) : (
                      <button
                        onClick={() => setMatrixDrawerPerson(p)}
                        className="p-2 rounded-lg bg-indigo-50 text-indigo-700 border border-indigo-200 hover:bg-indigo-100 text-xs font-medium flex items-center gap-1 transition-colors"
                        title="Cấu hình ma trận phân quyền"
                      >
                        <Layers className="w-3.5 h-3.5" /> Phân quyền ({p.permissions.length})
                      </button>
                    )}
                  </div>

                  <div className="flex items-center gap-1">
                    <button
                      onClick={() => handleOpenEdit(p)}
                      aria-label="Chỉnh sửa hồ sơ"
                      className="p-2 rounded-lg text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors"
                      title="Chỉnh sửa hồ sơ"
                    >
                      <Edit2 className="w-3.5 h-3.5" />
                    </button>
                    <button
                      onClick={() => handleRevoke(p)}
                      aria-label="Thu hồi người ủy thác"
                      className="p-2 rounded-lg text-red-500 hover:text-red-700 hover:bg-red-50 transition-colors"
                      title="Thu hồi người ủy thác"
                    >
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* Modals */}
      {isModalOpen && (
        <TrustedPersonModal
          person={selectedPerson}
          onClose={() => setIsModalOpen(false)}
          onSubmit={handleModalSubmit}
        />
      )}

      {pairingModalData && (
        <PairingCodeModal
          person={pairingModalData.person}
          pairingCode={pairingModalData.code}
          onClose={() => setPairingModalData(null)}
        />
      )}

      {matrixDrawerPerson && (
        <ScopedAccessMatrixDrawer
          person={matrixDrawerPerson}
          onClose={() => setMatrixDrawerPerson(null)}
          onSave={async (categoryPermissions, actionCardPermissions) => {
            await updatePermissions(matrixDrawerPerson.id, {
              categoryPermissions,
              actionCardPermissions,
            });
          }}
        />
      )}
    </div>
  );
};
