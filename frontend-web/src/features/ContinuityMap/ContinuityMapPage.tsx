import React, { useEffect, useState } from 'react';
import { Asset } from '../../types';
import { continuityMapService } from '../../services/continuityMapService';
import { Shield, Plus, ArrowUpRight } from 'lucide-react';

export const ContinuityMapPage: React.FC = () => {
  const [assets, setAssets] = useState<Asset[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    continuityMapService.getContinuityMap().then((data) => {
      setAssets(data);
      setLoading(false);
    });
  }, []);

  const totalValue = assets.reduce((sum, item) => sum + item.estimatedValue, 0);

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-white tracking-tight">Continuity Map</h2>
          <p className="text-sm text-slate-400">Trực quan hóa tài sản & sơ đồ kế thừa liên tục (feat-01-continuity-map).</p>
        </div>
        <button className="flex items-center gap-2 px-4 py-2 rounded-lg bg-emerald-600 hover:bg-emerald-500 text-white font-medium text-sm transition shadow-lg shadow-emerald-900/30">
          <Plus className="w-4 h-4" />
          <span>Thêm Tài Sản Mới</span>
        </button>
      </div>

      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="p-5 rounded-xl bg-slate-900/60 border border-slate-800">
          <p className="text-xs text-slate-400 font-medium">Tổng Giá Trị Tài Sản Bảo Vệ</p>
          <p className="text-2xl font-bold text-white mt-1">${totalValue.toLocaleString()}</p>
        </div>
        <div className="p-5 rounded-xl bg-slate-900/60 border border-slate-800">
          <p className="text-xs text-slate-400 font-medium">Tổng Số Hạng Mục</p>
          <p className="text-2xl font-bold text-emerald-400 mt-1">{assets.length} Assets</p>
        </div>
        <div className="p-5 rounded-xl bg-slate-900/60 border border-slate-800">
          <p className="text-xs text-slate-400 font-medium">Trạng Thái Bảo Mật</p>
          <p className="text-2xl font-bold text-sky-400 mt-1 flex items-center gap-2">
            <Shield className="w-5 h-5 text-emerald-400" /> AES-256 Vault
          </p>
        </div>
      </div>

      {/* Asset Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {loading ? (
          <p className="text-slate-400 col-span-3">Đang tải dữ liệu tài sản...</p>
        ) : (
          assets.map((asset) => (
            <div
              key={asset.id}
              className="p-5 rounded-xl bg-slate-900/40 border border-slate-800 hover:border-slate-700 transition group relative flex flex-col justify-between"
            >
              <div>
                <div className="flex items-center justify-between mb-2">
                  <span className="text-xs px-2 py-0.5 rounded-full bg-slate-800 text-slate-300 font-mono">
                    Type #{asset.type}
                  </span>
                  <ArrowUpRight className="w-4 h-4 text-slate-500 group-hover:text-emerald-400 transition" />
                </div>
                <h3 className="font-semibold text-white text-base group-hover:text-emerald-300 transition">
                  {asset.name}
                </h3>
                <p className="text-sm text-slate-400 mt-1">{asset.description}</p>
              </div>

              <div className="mt-6 pt-4 border-t border-slate-800/80 flex items-center justify-between">
                <span className="text-xs text-slate-500">Ước tính giá trị</span>
                <span className="text-base font-bold text-emerald-400 font-mono">
                  ${asset.estimatedValue.toLocaleString()}
                </span>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};
