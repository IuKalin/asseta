import 'package:flutter/material.dart';
import '../../domain/entities/continuity_map_entity.dart';

class GapAlertCard extends StatelessWidget {
  final List<ContinuityGapEntity> gaps;
  final Function(ContinuityGapEntity)? onTapGap;

  const GapAlertCard({
    super.key,
    required this.gaps,
    this.onTapGap,
  });

  @override
  Widget build(BuildContext context) {
    if (gaps.isEmpty) return const SizedBox.shrink();

    final theme = Theme.of(context);

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.amber.shade900.withOpacity(0.18),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: Colors.amber.shade700.withOpacity(0.5)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(Icons.warning_amber_rounded, color: Colors.amber.shade400, size: 20),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Khoảng Trống Tiếp Quản (${gaps.length} mục)',
                  style: theme.textTheme.titleSmall?.copyWith(
                    color: Colors.amber.shade300,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 6),
          Text(
            'Các hạng mục mức độ Critical/Important đang thiếu người phụ trách hoặc vị trí hồ sơ:',
            style: theme.textTheme.bodySmall?.copyWith(
              color: Colors.amber.shade200.withOpacity(0.8),
              fontSize: 11,
            ),
          ),
          const SizedBox(height: 10),
          Wrap(
            spacing: 6,
            runSpacing: 6,
            children: gaps.take(3).map((gap) {
              return ActionChip(
                backgroundColor: Colors.amber.shade900.withOpacity(0.3),
                side: BorderSide(color: Colors.amber.shade700.withOpacity(0.4)),
                labelPadding: const EdgeInsets.symmetric(horizontal: 4),
                avatar: const Icon(Icons.arrow_forward, size: 12, color: Colors.amber),
                label: Text(
                  gap.itemName,
                  style: const TextStyle(fontSize: 11, color: Colors.amber),
                ),
                onPressed: () => onTapGap?.call(gap),
              );
            }).toList(),
          ),
        ],
      ),
    );
  }
}
