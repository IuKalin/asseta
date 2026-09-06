import React, { useState } from 'react';
import {
  ShieldAlert,
  Clock,
  Heart,
  CheckCircle2,
  AlertTriangle,
  Settings,
  RefreshCw,
  Users,
  ShieldCheck,
  Ban,
  Activity,
  Send,
  Loader2
} from 'lucide-react';
import { useSafeActivation } from './hooks/useSafeActivation';

export const SafeActivationPanel: React.FC = () => {
  const {
    status,
    loading,
    error,
    remainingSeconds,
    refreshStatus,
    vitalityCheckIn,
    updateConfig,
    initiateRequest,
    cancelRequest,
    confirmRequest,
    deactivateEmergency,
  } = useSafeActivation();

  const [activeTab, setActiveTab] = useState<'overview' | 'settings' | 'simulate'>('overview');
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);
  const [formError, setFormError] = useState<string | null>(null);

  // Settings form state
  const [intervalDays, setIntervalDays] = useState<number>(30);
  const [graceHours, setGraceHours] = useState<number>(48);
  const [minConfirmations, setMinConfirmations] = useState<number>(1);

  // Simulation form state
  const [simOwnerId, setSimOwnerId] = useState<string>('');
  const [simReason, setSimReason] = useState<string>('Yêu cầu kích hoạt do không liên lạc được');
  const [voteNote, setVoteNote] = useState<string>('');

  React.useEffect(() => {
    if (status) {
      setIntervalDays(status.checkInIntervalDays);
      setGraceHours(status.gracePeriodHours);
      setMinConfirmations(status.minConfirmationsRequired);
      if (!simOwnerId) {
        setSimOwnerId(status.ownerId);
      }
    }
  }, [status]);

  const showNotification = (msg: string) => {
    setSuccessMsg(msg);
    setFormError(null);
    setTimeout(() => setSuccessMsg(null), 5000);
  };

  const handleCheckIn = async () => {
    try {
      setActionLoading('checkin');
      await vitalityCheckIn();
      showNotification('Điểm danh thành công! Chu kỳ an toàn của bạn đã được gia hạn.');
    } catch (err: any) {
      setFormError(err.message || 'Lỗi khi thực hiện điểm danh.');
    } finally {
      setActionLoading(null);
    }
  };

  const handleCancelRequest = async (requestId: string) => {
    try {
      setActionLoading('cancel');
      await cancelRequest(requestId);
      showNotification('Đã hủy yêu cầu kích hoạt an toàn khẩn cấp! Trạng thái đã trở về bình thường.');
    } catch (err: any) {
      setFormError(err.message || 'Lỗi khi hủy yêu cầu.');
    } finally {
      setActionLoading(null);
    }
  };

  const handleSaveConfig = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setActionLoading('config');
      await updateConfig({
        checkInIntervalDays: Number(intervalDays),
        gracePeriodHours: Number(graceHours),
        minConfirmationsRequired: Number(minConfirmations),
      });
      showNotification('Đã cập nhật cấu hình kích hoạt an toàn thành công!');
    } catch (err: any) {
      setFormError(err.message || 'Lỗi khi cập nhật cấu hình.');
    } finally {
      setActionLoading(null);
    }
  };

  const handleInitiateRequest = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setActionLoading('initiate');
      await initiateRequest(simOwnerId || status?.ownerId || '', simReason);
      showNotification('Đã phát động yêu cầu kích hoạt khẩn cấp! Thời gian đệm time-lock đã bắt đầu.');
      setActiveTab('overview');
    } catch (err: any) {
      setFormError(err.message || 'Lỗi khi phát động yêu cầu.');
    } finally {
      setActionLoading(null);
    }
  };

  const handleVoteConfirmation = async (requestId: string, isConfirmed: boolean) => {
    try {
      setActionLoading('vote');
      await confirmRequest(requestId, isConfirmed, voteNote || undefined);
      showNotification(isConfirmed ? 'Đã ghi nhận phiếu xác nhận của bạn.' : 'Đã ghi nhận phiếu từ chối của bạn.');
    } catch (err: any) {
      setFormError(err.message || 'Lỗi khi bỏ phiếu.');
    } finally {
      setActionLoading(null);
    }
  };

  const handleDeactivate = async () => {
    try {
      setActionLoading('deactivate');
      await deactivateEmergency();
      showNotification('Đã khôi phục quyền kiểm soát và tắt trạng thái khẩn cấp.');
    } catch (err: any) {
      setFormError(err.message || 'Lỗi khi tắt chế độ khẩn cấp.');
    } finally {
      setActionLoading(null);
    }
  };

  const formatCountdown = (totalSec: number) => {
    const hrs = Math.floor(totalSec / 3600);
    const mins = Math.floor((totalSec % 3600) / 60);
    const secs = totalSec % 60;
    return `${hrs.toString().padStart(2, '0')}h : ${mins.toString().padStart(2, '0')}m : ${secs.toString().padStart(2, '0')}s`;
  };

  const pendingRequest = status?.activeRequest;
  const isGracePeriodActive = pendingRequest && pendingRequest.status === 'PendingGracePeriod';

  return (
    <div className="space-y-6 max-w-4xl mx-auto">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-slate-900 tracking-tight flex items-center gap-2.5">
            <Activity className="w-7 h-7 text-emerald-600" />
            Giao Thức Kích Hoạt An Toàn (Safe Activation)
          </h2>
          <p className="text-sm text-slate-600">
            Dead Man's Switch và cơ chế bảo vệ Time-lock đa chữ ký với quyền hủy 1-chạm của chủ tài sản.
          </p>
        </div>

        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={() => refreshStatus()}
            disabled={loading}
            className="p-2 rounded-lg bg-white hover:bg-slate-100 text-slate-700 border border-slate-300 transition-colors shadow-xs"
            title="Làm mới trạng thái"
            aria-label="Làm mới trạng thái"
          >
            <RefreshCw className={`w-4 h-4 motion-reduce:animate-none ${loading ? 'animate-spin text-emerald-600' : ''}`} />
          </button>
        </div>
      </div>

      {/* Notifications */}
      {successMsg && (
        <div role="alert" aria-live="polite" className="p-4 rounded-xl bg-emerald-50 border border-emerald-200 text-emerald-800 text-sm flex items-center gap-2">
          <CheckCircle2 className="w-5 h-5 shrink-0 text-emerald-600" />
          <span>{successMsg}</span>
        </div>
      )}

      {(error || formError) && (
        <div role="alert" aria-live="polite" className="p-4 rounded-xl bg-rose-50 border border-rose-200 text-rose-800 text-sm flex items-center gap-2">
          <AlertTriangle className="w-5 h-5 shrink-0 text-rose-600" />
          <span>{formError || error}</span>
        </div>
      )}

      {/* EMERGENCY TIME-LOCK BANNER */}
      {isGracePeriodActive && (
        <div className="p-6 rounded-2xl bg-rose-50 border-2 border-rose-500 shadow-xl space-y-4 animate-pulse motion-reduce:animate-none">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <div className="flex items-center gap-3 text-rose-600">
              <ShieldAlert className="w-8 h-8 text-rose-600 shrink-0" />
              <div>
                <h3 className="font-extrabold text-xl text-rose-900">
                  CẢNH BÁO: ĐANG TRONG THỜI GIAN ĐỆM AN TOÀN (TIME-LOCK)
                </h3>
                <p className="text-xs text-rose-800">
                  Yêu cầu kích hoạt khẩn cấp đã được gửi. Nếu bạn vẫn an toàn, hãy hủy ngay lập tức.
                </p>
              </div>
            </div>

            <div className="bg-white border border-rose-200 rounded-xl px-4 py-2 text-center shadow-xs">
              <div className="text-xs text-rose-700 font-medium">Thời gian còn lại</div>
              <div className="text-2xl font-black font-mono text-rose-900 tracking-widest tabular-nums">
                {formatCountdown(remainingSeconds)}
              </div>
            </div>
          </div>

          <div className="bg-white p-4 rounded-xl border border-rose-100 text-sm text-slate-700 space-y-2 shadow-xs">
            <div className="flex justify-between items-center text-xs">
              <span className="text-slate-500">Lý do kích hoạt:</span>
              <span className="font-medium text-slate-900">{pendingRequest?.reason || 'Không rõ'}</span>
            </div>
            <div className="flex justify-between items-center text-xs">
              <span className="text-slate-500">Tiến độ biểu quyết Quorum:</span>
              <span className="font-bold text-amber-700 tabular-nums">
                {pendingRequest?.confirmationsCount} / {pendingRequest?.minConfirmationsRequired} Người ủy thác xác nhận
              </span>
            </div>
          </div>

          <div className="flex flex-wrap items-center justify-between gap-4 pt-2">
            <button
              type="button"
              onClick={() => pendingRequest && handleCancelRequest(pendingRequest.id)}
              disabled={actionLoading === 'cancel'}
              className="px-6 py-3 rounded-xl bg-rose-600 hover:bg-rose-500 text-white font-bold text-sm transition-colors shadow-md shadow-rose-600/20 flex items-center gap-2"
            >
              {actionLoading === 'cancel' ? <Loader2 className="w-4 h-4 animate-spin motion-reduce:animate-none" /> : <Ban className="w-5 h-5" />}
              HỦY YÊU CẦU KÍCH HOẠT NGAY (1-CHẠM)
            </button>

            {/* Quick vote options for delegate simulation */}
            <div className="flex items-center gap-2">
              <input
                type="text"
                value={voteNote}
                onChange={(e) => setVoteNote(e.target.value)}
                placeholder="Ghi chú xác nhận..."
                className="bg-white border border-slate-300 rounded-lg px-2.5 py-1 text-xs text-slate-900 placeholder-slate-400 focus:outline-none focus:border-emerald-500 shadow-xs"
              />
              <button
                type="button"
                onClick={() => pendingRequest && handleVoteConfirmation(pendingRequest.id, true)}
                disabled={actionLoading === 'vote'}
                className="px-3 py-1.5 rounded-lg bg-emerald-600 hover:bg-emerald-500 text-white font-medium text-xs transition-colors shadow-xs"
              >
                Ủy Thác Xác Nhận (+1)
              </button>
            </div>
          </div>
        </div>
      )}

      {/* PLAN IS ACTIVATED BANNER */}
      {status?.isEmergencyActive && (
        <div className="p-6 rounded-2xl bg-amber-50 border border-amber-300 text-amber-900 space-y-3 shadow-xs">
          <div className="flex items-center gap-3">
            <ShieldAlert className="w-6 h-6 text-amber-600" />
            <h3 className="text-lg font-bold text-amber-950">Giao Thức Kế Thừa Đang Ở Chế Độ KÍCH HOẠT (ACTIVATED)</h3>
          </div>
          <p className="text-sm text-amber-800">
            Kế hoạch tiếp quản đã được giải phóng quyền truy cập giải mã cục bộ cho các Người Ủy Thác được phân quyền. Bạn có thể khôi phục lại quyền kiểm soát bất kỳ lúc nào.
          </p>
          <button
            type="button"
            onClick={handleDeactivate}
            disabled={actionLoading === 'deactivate'}
            className="px-4 py-2 rounded-xl bg-amber-600 hover:bg-amber-500 text-white font-bold text-sm transition-colors flex items-center gap-2 shadow-xs"
          >
            {actionLoading === 'deactivate' ? <Loader2 className="w-4 h-4 animate-spin motion-reduce:animate-none" /> : <ShieldCheck className="w-4 h-4" />}
            Khôi Phục Quyền Kiểm Soát & Đóng Kế Hoạch Khẩn Cấp
          </button>
        </div>
      )}

      {/* Tab Navigation */}
      <div className="flex border-b border-slate-200 space-x-6">
        <button
          type="button"
          onClick={() => setActiveTab('overview')}
          className={`pb-3 text-sm font-semibold transition-colors border-b-2 flex items-center gap-2 ${
            activeTab === 'overview'
              ? 'text-emerald-700 border-emerald-600'
              : 'text-slate-600 border-transparent hover:text-slate-900'
          }`}
        >
          <Activity className="w-4 h-4" /> Tổng Quan & Điểm Danh
        </button>
        <button
          type="button"
          onClick={() => setActiveTab('settings')}
          className={`pb-3 text-sm font-semibold transition-colors border-b-2 flex items-center gap-2 ${
            activeTab === 'settings'
              ? 'text-emerald-700 border-emerald-600'
              : 'text-slate-600 border-transparent hover:text-slate-900'
          }`}
        >
          <Settings className="w-4 h-4" /> Cấu Hình Chu Kỳ & Time-Lock
        </button>
        <button
          type="button"
          onClick={() => setActiveTab('simulate')}
          className={`pb-3 text-sm font-semibold transition-colors border-b-2 flex items-center gap-2 ${
            activeTab === 'simulate'
              ? 'text-emerald-700 border-emerald-600'
              : 'text-slate-600 border-transparent hover:text-slate-900'
          }`}
        >
          <Send className="w-4 h-4" /> Mô Phỏng Ủy Thác & Kích Hoạt
        </button>
      </div>

      {/* TAB 1: OVERVIEW & VITALITY CHECK-IN */}
      {activeTab === 'overview' && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {/* Main Hero Check-In Card */}
          <div className="md:col-span-2 p-6 rounded-2xl bg-white border border-slate-200 shadow-sm space-y-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-xs uppercase tracking-wider text-slate-500 font-semibold">Trạng Thái Sống (Vitality)</div>
                <div className="text-xl font-bold text-slate-900 mt-1 flex items-center gap-2">
                  <span
                    className={`w-3 h-3 rounded-full ${
                      status?.heartbeatStatus === 'ACTIVE'
                        ? 'bg-emerald-500 shadow-sm'
                        : status?.heartbeatStatus === 'WARNING'
                        ? 'bg-amber-500 shadow-sm'
                        : 'bg-rose-500 shadow-sm'
                    }`}
                  />
                  {status?.heartbeatStatus === 'ACTIVE'
                    ? 'Bình Thường (Đã Điểm Danh)'
                    : status?.heartbeatStatus === 'WARNING'
                    ? 'Sắp Đến Hạn Điểm Danh'
                    : status?.heartbeatStatus === 'PENDING_GRACE_PERIOD'
                    ? 'Đang Đệm Chờ Kích Hoạt'
                    : 'Đã Kích Hoạt Khẩn Cấp'}
                </div>
              </div>

              <div className="p-3 rounded-xl bg-slate-50 border border-slate-200 text-right">
                <div className="text-xs text-slate-500">Chu kỳ điểm danh</div>
                <div className="text-base font-bold text-slate-900 tabular-nums">{status?.checkInIntervalDays || 30} ngày</div>
              </div>
            </div>

            <div className="bg-slate-50 p-4 rounded-xl border border-slate-200 space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-slate-500">Lần điểm danh gần nhất:</span>
                <span className="text-slate-900 font-medium tabular-nums">
                  {status?.lastCheckInAtUtc ? new Date(status.lastCheckInAtUtc).toLocaleString('vi-VN') : 'Chưa điểm danh'}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-slate-500">Hạn chót điểm danh kế tiếp:</span>
                <span className={`font-semibold tabular-nums ${status?.isDueSoon || status?.isOverdue ? 'text-amber-700' : 'text-slate-900'}`}>
                  {status?.nextCheckInDueUtc ? new Date(status.nextCheckInDueUtc).toLocaleString('vi-VN') : 'Chưa xác định'}
                </span>
              </div>
            </div>

            {/* Hero 1-Tap Vitality Button */}
            <div className="pt-2">
              <button
                type="button"
                onClick={handleCheckIn}
                disabled={actionLoading === 'checkin'}
                className="w-full py-4 px-6 rounded-2xl bg-emerald-600 hover:bg-emerald-500 text-white font-extrabold text-lg transition-colors shadow-lg shadow-emerald-600/20 flex items-center justify-center gap-3 active:scale-[0.99]"
              >
                {actionLoading === 'checkin' ? (
                  <Loader2 className="w-6 h-6 animate-spin motion-reduce:animate-none" />
                ) : (
                  <Heart className="w-6 h-6 text-white fill-white" />
                )}
                TÔI VẪN ỔN (ĐIỂM DANH 1-CHẠM)
              </button>
              <p className="text-xs text-center text-slate-500 mt-2.5">
                Bấm vào đây để xác nhận bạn vẫn an toàn và tự động reset chu kỳ bảo vệ thêm {status?.checkInIntervalDays || 30} ngày.
              </p>
            </div>
          </div>

          {/* Quick Metrics & Quorum Info */}
          <div className="space-y-6">
            <div className="p-6 rounded-2xl bg-white border border-slate-200 shadow-sm space-y-4">
              <div className="flex items-center gap-2.5 text-slate-900 font-bold text-base">
                <Clock className="w-5 h-5 text-indigo-600" />
                Thời Gian Chờ Time-Lock
              </div>
              <p className="text-xs text-slate-600 leading-relaxed">
                Khi kích hoạt, hệ thống sẽ trì hoãn thực thi trong vòng <strong className="text-slate-900 tabular-nums">{status?.gracePeriodHours || 48} giờ</strong> để bạn có cơ hội kiểm tra thông báo và hủy lệnh nếu có nhầm lẫn.
              </p>
            </div>

            <div className="p-6 rounded-2xl bg-white border border-slate-200 shadow-sm space-y-4">
              <div className="flex items-center gap-2.5 text-slate-900 font-bold text-base">
                <Users className="w-5 h-5 text-amber-600" />
                Cơ Chế Quorum Đa Chữ Ký
              </div>
              <p className="text-xs text-slate-600 leading-relaxed">
                Yêu cầu kích hoạt từ Người Ủy Thác cần tối thiểu <strong className="text-slate-900 tabular-nums">{status?.minConfirmationsRequired || 1} xác nhận</strong> từ Người Ủy Thác Cấp 2 hoặc Cấp 3 để phòng ngừa hành vi đơn phương ác ý.
              </p>
            </div>
          </div>
        </div>
      )}

      {/* TAB 2: SETTINGS */}
      {activeTab === 'settings' && (
        <form onSubmit={handleSaveConfig} className="p-6 rounded-2xl bg-white border border-slate-200 shadow-sm space-y-6">
          <div>
            <h3 className="text-lg font-bold text-slate-900">Cấu Hình Chu Kỳ Điểm Danh & Time-Lock</h3>
            <p className="text-xs text-slate-500">Tùy chỉnh thông số an toàn phù hợp với thói quen sinh hoạt và mức độ bảo mật của bạn.</p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="space-y-2">
              <label htmlFor="sa-interval" className="text-sm font-semibold text-slate-700 block">Chu kỳ điểm danh (ngày)</label>
              <select
                id="sa-interval"
                value={intervalDays}
                onChange={(e) => setIntervalDays(Number(e.target.value))}
                className="w-full bg-slate-50 border border-slate-300 rounded-xl px-4 py-2.5 text-slate-900 text-sm focus:bg-white focus:outline-none focus:border-emerald-500"
              >
                <option value={15}>15 ngày (Tần suất cao)</option>
                <option value={30}>30 ngày (Tiêu chuẩn khuyên dùng)</option>
                <option value={60}>60 ngày (Thưa)</option>
                <option value={90}>90 ngày (Tối đa)</option>
              </select>
              <p className="text-xs text-slate-500">Hệ thống sẽ gửi nhắc nhở khi còn 5 ngày trước hạn.</p>
            </div>

            <div className="space-y-2">
              <label htmlFor="sa-grace" className="text-sm font-semibold text-slate-700 block">Thời gian đệm an toàn (Time-lock)</label>
              <select
                id="sa-grace"
                value={graceHours}
                onChange={(e) => setGraceHours(Number(e.target.value))}
                className="w-full bg-slate-50 border border-slate-300 rounded-xl px-4 py-2.5 text-slate-900 text-sm focus:bg-white focus:outline-none focus:border-emerald-500"
              >
                <option value={24}>24 giờ (Nhanh)</option>
                <option value={48}>48 giờ (Khuyên dùng)</option>
                <option value={72}>72 giờ (3 ngày)</option>
                <option value={168}>168 giờ (7 ngày)</option>
              </select>
              <p className="text-xs text-slate-500">Thời gian chủ tài sản được quyền hủy bỏ yêu cầu.</p>
            </div>

            <div className="space-y-2">
              <label htmlFor="sa-quorum" className="text-sm font-semibold text-slate-700 block">Ngưỡng biểu quyết tối thiểu (Quorum)</label>
              <select
                id="sa-quorum"
                value={minConfirmations}
                onChange={(e) => setMinConfirmations(Number(e.target.value))}
                className="w-full bg-slate-50 border border-slate-300 rounded-xl px-4 py-2.5 text-slate-900 text-sm focus:bg-white focus:outline-none focus:border-emerald-500"
              >
                <option value={1}>1 Người ủy thác xác nhận</option>
                <option value={2}>2 Người ủy thác xác nhận</option>
                <option value={3}>3 Người ủy thác xác nhận</option>
              </select>
              <p className="text-xs text-slate-500">Chỉ áp dụng cho người ủy thác cấp 2 và cấp 3.</p>
            </div>
          </div>

          <div className="pt-4 flex justify-end">
            <button
              type="submit"
              disabled={actionLoading === 'config'}
              className="px-6 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-sm transition-colors shadow-xs flex items-center gap-2"
            >
              {actionLoading === 'config' ? <Loader2 className="w-4 h-4 animate-spin motion-reduce:animate-none" /> : <ShieldCheck className="w-4 h-4" />}
              Lưu Cấu Hình
            </button>
          </div>
        </form>
      )}

      {/* TAB 3: SIMULATE DELEGATE REQUEST */}
      {activeTab === 'simulate' && (
        <form onSubmit={handleInitiateRequest} className="p-6 rounded-2xl bg-white border border-slate-200 shadow-sm space-y-6">
          <div>
            <h3 className="text-lg font-bold text-slate-900">Mô Phỏng Phát Động Yêu Cầu Kích Hoạt Khẩn Cấp</h3>
            <p className="text-xs text-slate-500">
              Công cụ thử nghiệm quy trình khẩn cấp: Gửi yêu cầu kích hoạt để kiểm tra thời gian đệm time-lock, thông báo cảnh báo và tính năng hủy 1-chạm.
            </p>
          </div>

          <div className="space-y-4">
            <div className="space-y-2">
              <label htmlFor="sa-sim-owner" className="text-sm font-semibold text-slate-700 block">Target Owner Id</label>
              <input
                id="sa-sim-owner"
                type="text"
                value={simOwnerId}
                onChange={(e) => setSimOwnerId(e.target.value)}
                placeholder="Nhập Owner GUID..."
                className="w-full bg-slate-50 border border-slate-300 rounded-xl px-4 py-2.5 text-slate-900 text-sm focus:bg-white focus:outline-none focus:border-emerald-500 placeholder-slate-400"
                required
              />
            </div>

            <div className="space-y-2">
              <label htmlFor="sa-sim-reason" className="text-sm font-semibold text-slate-700 block">Lý do yêu cầu kích hoạt</label>
              <textarea
                id="sa-sim-reason"
                value={simReason}
                onChange={(e) => setSimReason(e.target.value)}
                rows={3}
                placeholder="Nhập lý do kích hoạt khẩn cấp..."
                className="w-full bg-slate-50 border border-slate-300 rounded-xl px-4 py-2.5 text-slate-900 text-sm focus:bg-white focus:outline-none focus:border-emerald-500 placeholder-slate-400"
              />
            </div>
          </div>

          <div className="pt-2 flex justify-end">
            <button
              type="submit"
              disabled={actionLoading === 'initiate'}
              className="px-6 py-2.5 rounded-xl bg-amber-600 hover:bg-amber-500 text-white font-bold text-sm transition-colors shadow-xs flex items-center gap-2"
            >
              {actionLoading === 'initiate' ? <Loader2 className="w-4 h-4 animate-spin motion-reduce:animate-none" /> : <Send className="w-4 h-4" />}
              Phát Động Yêu Cầu Kích Hoạt
            </button>
          </div>
        </form>
      )}
    </div>
  );
};
