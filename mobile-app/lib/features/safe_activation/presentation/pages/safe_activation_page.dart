import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/entities/safe_activation_entity.dart';
import '../bloc/safe_activation_bloc.dart';
import '../bloc/safe_activation_event.dart';
import '../bloc/safe_activation_state.dart';

class SafeActivationPage extends StatelessWidget {
  const SafeActivationPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(
          'Kích Hoạt An Toàn',
          style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              context.read<SafeActivationBloc>().add(const FetchSafeActivationStatusEvent());
            },
            tooltip: 'Làm mới',
          ),
          IconButton(
            icon: const Icon(Icons.settings_outlined),
            onPressed: () {
              final state = context.read<SafeActivationBloc>().state;
              if (state is SafeActivationLoaded) {
                _showSettingsSheet(context, state.status);
              }
            },
            tooltip: 'Cài đặt',
          ),
        ],
      ),
      body: BlocConsumer<SafeActivationBloc, SafeActivationState>(
        listener: (context, state) {
          if (state is SafeActivationLoaded && state.successMessage != null) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.successMessage!),
                backgroundColor: const Color(0xFF10B981),
                behavior: SnackBarBehavior.floating,
              ),
            );
          } else if (state is SafeActivationError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.message),
                backgroundColor: Colors.redAccent,
                behavior: SnackBarBehavior.floating,
              ),
            );
          }
        },
        builder: (context, state) {
          if (state is SafeActivationLoading) {
            return const Center(child: CircularProgressIndicator(color: Color(0xFF10B981)));
          }

          if (state is SafeActivationError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24.0),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.error_outline, color: Colors.redAccent, size: 48),
                    const SizedBox(height: 16),
                    Text(
                      state.message,
                      textAlign: TextAlign.center,
                      style: const TextStyle(color: Colors.white70),
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton(
                      onPressed: () {
                        context.read<SafeActivationBloc>().add(const FetchSafeActivationStatusEvent());
                      },
                      child: const Text('Thử lại'),
                    ),
                  ],
                ),
              ),
            );
          }

          if (state is SafeActivationLoaded) {
            final status = state.status;
            final isGracePeriod = status.activeRequest?.isPendingGracePeriod ?? false;

            return RefreshIndicator(
              onRefresh: () async {
                context.read<SafeActivationBloc>().add(const FetchSafeActivationStatusEvent());
              },
              child: ListView(
                padding: const EdgeInsets.all(16.0),
                children: [
                  // 1. EMERGENCY TIME-LOCK BANNER (if active)
                  if (isGracePeriod && status.activeRequest != null)
                    _buildTimeLockBanner(context, status.activeRequest!, state.countdownSeconds),

                  // 2. ACTIVATED EMERGENCY BANNER (if plan activated)
                  if (status.isEmergencyActive)
                    _buildActivatedBanner(context),

                  const SizedBox(height: 12),

                  // 3. VITALITY CHECK-IN HERO CARD
                  _buildVitalityHeroCard(context, status, state.isActionInProgress),

                  const SizedBox(height: 16),

                  // 4. PROTOCOL PARAMETERS CARD
                  _buildParametersCard(context, status),

                  const SizedBox(height: 16),

                  // 5. DELEGATE SIMULATION / TEST TRIGGER
                  _buildSimulateCard(context, status),
                ],
              ),
            );
          }

          return const SizedBox.shrink();
        },
      ),
    );
  }

  Widget _buildTimeLockBanner(
    BuildContext context,
    ActivationRequestEntity request,
    int countdownSeconds,
  ) {
    final hrs = (countdownSeconds ~/ 3600).toString().padLeft(2, '0');
    final mins = ((countdownSeconds % 3600) ~/ 60).toString().padLeft(2, '0');
    final secs = (countdownSeconds % 60).toString().padLeft(2, '0');

    return Container(
      margin: const EdgeInsets.only(bottom: 16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: const Color(0xFF450A0A),
        border: Border.all(color: Colors.redAccent, width: 2),
        borderRadius: BorderRadius.circular(16),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: const [
              Icon(Icons.warning_amber_rounded, color: Colors.redAccent, size: 28),
              SizedBox(width: 8),
              Expanded(
                child: Text(
                  'CẢNH BÁO: ĐANG TRONG THỜI GIAN ĐỆM',
                  style: TextStyle(
                    color: Colors.redAccent,
                    fontWeight: FontWeight.bold,
                    fontSize: 14,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 12),
            decoration: BoxDecoration(
              color: Colors.black45,
              borderRadius: BorderRadius.circular(12),
            ),
            child: Column(
              children: [
                const Text(
                  'THỜI GIAN CÒN LẠI ĐỂ HỦY BỎ',
                  style: TextStyle(fontSize: 11, color: Colors.white60, letterSpacing: 1),
                ),
                const SizedBox(height: 4),
                Text(
                  '$hrs:$mins:$secs',
                  style: const TextStyle(
                    fontSize: 32,
                    fontWeight: FontWeight.w900,
                    color: Colors.white,
                    fontFamily: 'monospace',
                    letterSpacing: 2,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          Text(
            'Lý do kích hoạt: ${request.reason ?? "Không rõ"}',
            style: const TextStyle(fontSize: 12, color: Colors.white70),
          ),
          Text(
            'Tiến độ Quorum: ${request.confirmationsCount} / ${request.minConfirmationsRequired} Người ủy thác xác nhận',
            style: const TextStyle(fontSize: 12, color: Colors.amberAccent, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            height: 48,
            child: ElevatedButton.icon(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.redAccent,
                foregroundColor: Colors.white,
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              ),
              onPressed: () {
                context.read<SafeActivationBloc>().add(CancelSafeActivationRequestEvent(request.id));
              },
              icon: const Icon(Icons.block),
              label: const Text(
                'HỦY YÊU CẦU KÍCH HOẠT (1-CHẠM)',
                style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildActivatedBanner(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: const Color(0xFF451A03),
        border: Border.all(color: Colors.orangeAccent, width: 1.5),
        borderRadius: BorderRadius.circular(16),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: const [
              Icon(Icons.shield_outlined, color: Colors.orangeAccent, size: 24),
              SizedBox(width: 8),
              Expanded(
                child: Text(
                  'KẾ HOẠCH ĐANG Ở TRẠNG THÁI KÍCH HOẠT',
                  style: TextStyle(color: Colors.orangeAccent, fontWeight: FontWeight.bold, fontSize: 13),
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          const Text(
            'Các người ủy thác hợp lệ hiện đã có thể truy cập kế hoạch tiếp quản.',
            style: TextStyle(fontSize: 12, color: Colors.white70),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              style: OutlinedButton.styleFrom(
                foregroundColor: Colors.orangeAccent,
                side: const BorderSide(color: Colors.orangeAccent),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
              ),
              onPressed: () {
                context.read<SafeActivationBloc>().add(const DeactivateEmergencyPlanEvent());
              },
              icon: const Icon(Icons.lock_reset),
              label: const Text('Khôi Phục Kiểm Soát & Đóng Kế Hoạch'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildVitalityHeroCard(
    BuildContext context,
    ActivationStatusEntity status,
    bool isActionInProgress,
  ) {
    Color statusColor;
    String statusTitle;

    switch (status.heartbeatStatus) {
      case HeartbeatStatus.active:
        statusColor = const Color(0xFF10B981);
        statusTitle = 'Bình Thường (An Toàn)';
        break;
      case HeartbeatStatus.warning:
        statusColor = Colors.amberAccent;
        statusTitle = 'Sắp Đến Hạn Điểm Danh';
        break;
      case HeartbeatStatus.pendingGracePeriod:
        statusColor = Colors.orangeAccent;
        statusTitle = 'Đang Đệm Chờ Kích Hoạt';
        break;
      case HeartbeatStatus.activated:
        statusColor = Colors.redAccent;
        statusTitle = 'Đã Kích Hoạt Khẩn Cấp';
        break;
    }

    return Card(
      color: const Color(0xFF1E293B),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      elevation: 4,
      child: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    Container(
                      width: 12,
                      height: 12,
                      decoration: BoxDecoration(
                        color: statusColor,
                        shape: BoxShape.circle,
                        boxShadow: [
                          BoxShadow(
                            color: statusColor.withOpacity(0.5),
                            blurRadius: 8,
                            spreadRadius: 2,
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(width: 8),
                    Text(
                      statusTitle,
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 15,
                        color: Colors.white,
                      ),
                    ),
                  ],
                ),
                Text(
                  'Chu kỳ ${status.checkInIntervalDays} ngày',
                  style: const TextStyle(fontSize: 12, color: Colors.white54),
                ),
              ],
            ),
            const SizedBox(height: 20),
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.black26,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text('Lần điểm danh gần nhất:', style: TextStyle(fontSize: 12, color: Colors.white60)),
                  Text(
                    status.lastCheckInAtUtc != null
                        ? '${status.lastCheckInAtUtc!.day}/${status.lastCheckInAtUtc!.month}/${status.lastCheckInAtUtc!.year}'
                        : 'Chưa có',
                    style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: Colors.white),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),
            SizedBox(
              width: double.infinity,
              height: 56,
              child: ElevatedButton.icon(
                style: ElevatedButton.styleFrom(
                  backgroundColor: const Color(0xFF10B981),
                  foregroundColor: Colors.white,
                  elevation: 6,
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
                ),
                onPressed: isActionInProgress
                    ? null
                    : () {
                        context.read<SafeActivationBloc>().add(const VitalityCheckInEvent());
                      },
                icon: isActionInProgress
                    ? const SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                      )
                    : const Icon(Icons.favorite, color: Colors.redAccent),
                label: const Text(
                  'TÔI VẪN ỔN (ĐIỂM DANH 1-CHẠM)',
                  style: TextStyle(fontWeight: FontWeight.w900, fontSize: 15, letterSpacing: 0.5),
                ),
              ),
            ),
            const SizedBox(height: 8),
            const Text(
              'Xác nhận bạn vẫn an toàn và tự động gia hạn chu kỳ bảo vệ',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 11, color: Colors.white54),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildParametersCard(BuildContext context, ActivationStatusEntity status) {
    return Card(
      color: const Color(0xFF1E293B),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Thông Số Giao Thức Bảo Vệ',
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14, color: Colors.white),
            ),
            const SizedBox(height: 12),
            _buildParamRow('Thời gian đệm an toàn (Time-Lock):', '${status.gracePeriodHours} giờ'),
            const Divider(color: Colors.white10, height: 16),
            _buildParamRow('Ngưỡng Quorum xác nhận:', '${status.minConfirmationsRequired} Người ủy thác'),
            const Divider(color: Colors.white10, height: 16),
            _buildParamRow('Chu kỳ điểm danh:', '${status.checkInIntervalDays} ngày'),
          ],
        ),
      ),
    );
  }

  Widget _buildParamRow(String label, String value) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: const TextStyle(fontSize: 12, color: Colors.white60)),
        Text(value, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: Colors.white)),
      ],
    );
  }

  Widget _buildSimulateCard(BuildContext context, ActivationStatusEntity status) {
    return Card(
      color: const Color(0xFF1E293B),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Mô Phỏng Người Ủy Thác Kích Hoạt',
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14, color: Colors.white),
            ),
            const SizedBox(height: 8),
            const Text(
              'Kiểm tra thử nghiệm phản ứng của hệ thống time-lock và thông báo cảnh báo.',
              style: TextStyle(fontSize: 11, color: Colors.white54),
            ),
            const SizedBox(height: 12),
            SizedBox(
              width: double.infinity,
              child: OutlinedButton.icon(
                style: OutlinedButton.styleFrom(
                  foregroundColor: Colors.amberAccent,
                  side: const BorderSide(color: Colors.amberAccent),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                ),
                onPressed: () {
                  context.read<SafeActivationBloc>().add(
                        InitiateSafeActivationRequestEvent(
                          targetOwnerId: status.ownerId,
                          reason: 'Thử nghiệm kích hoạt khẩn cấp (Simulation)',
                        ),
                      );
                },
                icon: const Icon(Icons.play_arrow),
                label: const Text('Phát Động Kích Hoạt Thử Nghiệm'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _showSettingsSheet(BuildContext context, ActivationStatusEntity status) {
    var intervalDays = status.checkInIntervalDays;
    var graceHours = status.gracePeriodHours;
    var minConfirmations = status.minConfirmationsRequired;

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: const Color(0xFF1E293B),
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      builder: (bottomSheetContext) {
        return StatefulBuilder(
          builder: (context, setState) {
            return Padding(
              padding: EdgeInsets.only(
                left: 20,
                right: 20,
                top: 20,
                bottom: MediaQuery.of(context).viewInsets.bottom + 24,
              ),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Cấu Hình Giao Thức An Toàn',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white),
                  ),
                  const SizedBox(height: 16),
                  const Text('Chu kỳ điểm danh', style: TextStyle(fontSize: 12, color: Colors.white70)),
                  DropdownButton<int>(
                    value: intervalDays,
                    isExpanded: true,
                    dropdownColor: const Color(0xFF0F172A),
                    items: const [
                      DropdownMenuItem(value: 15, child: Text('15 ngày (Thường xuyên)')),
                      DropdownMenuItem(value: 30, child: Text('30 ngày (Khuyên dùng)')),
                      DropdownMenuItem(value: 60, child: Text('60 ngày (Thưa)')),
                      DropdownMenuItem(value: 90, child: Text('90 ngày (Tối đa)')),
                    ],
                    onChanged: (val) {
                      if (val != null) setState(() => intervalDays = val);
                    },
                  ),
                  const SizedBox(height: 12),
                  const Text('Thời gian đệm an toàn (Time-Lock)', style: TextStyle(fontSize: 12, color: Colors.white70)),
                  DropdownButton<int>(
                    value: graceHours,
                    isExpanded: true,
                    dropdownColor: const Color(0xFF0F172A),
                    items: const [
                      DropdownMenuItem(value: 24, child: Text('24 giờ')),
                      DropdownMenuItem(value: 48, child: Text('48 giờ (Khuyên dùng)')),
                      DropdownMenuItem(value: 72, child: Text('72 giờ (3 ngày)')),
                      DropdownMenuItem(value: 168, child: Text('168 giờ (7 ngày)')),
                    ],
                    onChanged: (val) {
                      if (val != null) setState(() => graceHours = val);
                    },
                  ),
                  const SizedBox(height: 12),
                  const Text('Ngưỡng Quorum xác nhận', style: TextStyle(fontSize: 12, color: Colors.white70)),
                  DropdownButton<int>(
                    value: minConfirmations,
                    isExpanded: true,
                    dropdownColor: const Color(0xFF0F172A),
                    items: const [
                      DropdownMenuItem(value: 1, child: Text('1 Người ủy thác xác nhận')),
                      DropdownMenuItem(value: 2, child: Text('2 Người ủy thác xác nhận')),
                      DropdownMenuItem(value: 3, child: Text('3 Người ủy thác xác nhận')),
                    ],
                    onChanged: (val) {
                      if (val != null) setState(() => minConfirmations = val);
                    },
                  ),
                  const SizedBox(height: 20),
                  SizedBox(
                    width: double.infinity,
                    height: 48,
                    child: ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFF10B981),
                        foregroundColor: Colors.white,
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      ),
                      onPressed: () {
                        context.read<SafeActivationBloc>().add(
                              UpdateSafeActivationConfigEvent(
                                ActivationConfigEntity(
                                  checkInIntervalDays: intervalDays,
                                  gracePeriodHours: graceHours,
                                  minConfirmationsRequired: minConfirmations,
                                ),
                              ),
                            );
                        Navigator.pop(bottomSheetContext);
                      },
                      child: const Text('LƯU THAY ĐỔI', style: TextStyle(fontWeight: FontWeight.bold)),
                    ),
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }
}
