import 'package:flutter/material.dart';

class GapAlertBanner extends StatelessWidget {
  final bool hasSpofRisk;
  final String? spofWarning;
  final int gapsCount;

  const GapAlertBanner({
    super.key,
    required this.hasSpofRisk,
    this.spofWarning,
    required this.gapsCount,
  });

  @override
  Widget build(BuildContext context) {
    if (!hasSpofRisk && gapsCount == 0) {
      return const SizedBox.shrink();
    }

    return Column(
      children: [
        if (hasSpofRisk) ...[
          Container(
            key: const Key('spof_risk_banner'),
            margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.red.shade50,
              border: Border.all(color: Colors.red.shade300),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Icon(Icons.warning_amber_rounded, color: Colors.red.shade700, size: 24),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'CẢNH BÁO ĐIỂM NGHẼN ĐƠN LẺ (SPoF)',
                        style: TextStyle(
                          color: Colors.red.shade900,
                          fontWeight: FontWeight.bold,
                          fontSize: 13,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        spofWarning ??
                            'Một người ủy thác đang gánh trên 70% số đầu việc khẩn cấp. Hãy phân bổ thêm người phụ trách để giảm tải rủi ro.',
                        style: TextStyle(
                          color: Colors.red.shade800,
                          fontSize: 12,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
        if (gapsCount > 0) ...[
          Container(
            key: const Key('stage_gap_banner'),
            margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.amber.shade50,
              border: Border.all(color: Colors.amber.shade400),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Row(
              children: [
                Icon(Icons.error_outline_rounded, color: Colors.amber.shade800, size: 22),
                const SizedBox(width: 10),
                Expanded(
                  child: Text(
                    'Phát hiện $gapsCount đầu việc khẩn cấp (24h/72h) bị thiếu người ủy thác hoặc chưa ghi chú nơi lưu hồ sơ.',
                    style: TextStyle(
                      color: Colors.amber.shade900,
                      fontWeight: FontWeight.w500,
                      fontSize: 12,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ],
    );
  }
}
