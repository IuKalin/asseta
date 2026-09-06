import 'package:flutter/material.dart';
import '../../domain/entities/continuity_plan_entity.dart';

class StageTimelineTile extends StatelessWidget {
  final ContinuityPlanStageEntity stage;
  final void Function(String cardId, int rowVersion) onToggleCompletion;
  final void Function(String cardId, UrgencyStage newStage, int rowVersion) onMoveStage;
  final bool isDelegatedView;

  const StageTimelineTile({
    super.key,
    required this.stage,
    required this.onToggleCompletion,
    required this.onMoveStage,
    this.isDelegatedView = false,
  });

  Color _getStageColor(UrgencyStage s) {
    switch (s) {
      case UrgencyStage.immediate:
        return Colors.red.shade700;
      case UrgencyStage.first72Hours:
        return Colors.orange.shade700;
      case UrgencyStage.first7Days:
        return Colors.blue.shade700;
      case UrgencyStage.longerTerm:
        return Colors.green.shade700;
    }
  }

  Color _getStageBgColor(UrgencyStage s) {
    switch (s) {
      case UrgencyStage.immediate:
        return Colors.red.shade50;
      case UrgencyStage.first72Hours:
        return Colors.orange.shade50;
      case UrgencyStage.first7Days:
        return Colors.blue.shade50;
      case UrgencyStage.longerTerm:
        return Colors.green.shade50;
    }
  }

  @override
  Widget build(BuildContext context) {
    final stageColor = _getStageColor(stage.stage);
    final stageBg = _getStageBgColor(stage.stage);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      elevation: 1,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Stage Header
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: stageBg,
                    shape: BoxShape.circle,
                  ),
                  child: Icon(Icons.alarm, color: stageColor, size: 20),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        stage.stageName,
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: stageColor,
                        ),
                      ),
                      Text(
                        stage.stageDescription,
                        style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: Colors.grey.shade100,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    '${stage.completedCardsCount}/${stage.totalCardsCount}',
                    style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 12),
                  ),
                ),
                if (stage.gapCardsCount > 0) ...[
                  const SizedBox(width: 6),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                    decoration: BoxDecoration(
                      color: Colors.amber.shade100,
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: Text(
                      '${stage.gapCardsCount} hổng',
                      style: TextStyle(
                        color: Colors.amber.shade900,
                        fontWeight: FontWeight.bold,
                        fontSize: 11,
                      ),
                    ),
                  ),
                ],
              ],
            ),
            const Divider(height: 24),

            // Stage Cards
            if (stage.cards.isEmpty) ...[
              Padding(
                padding: const EdgeInsets.symmetric(vertical: 12),
                child: Center(
                  child: Text(
                    'Không có đầu việc nào trong giai đoạn này',
                    style: TextStyle(color: Colors.grey.shade500, fontSize: 13),
                  ),
                ),
              ),
            ] else ...[
              ...stage.cards.map((card) => _buildCardItem(context, card)),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildCardItem(BuildContext context, ContinuityPlanCardItemEntity card) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: card.isCompleted ? Colors.grey.shade50 : Colors.white,
        border: Border.all(
          color: card.hasStageGap ? Colors.amber.shade300 : Colors.grey.shade200,
          width: card.hasStageGap ? 1.5 : 1,
        ),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Checkbox(
                value: card.isCompleted,
                onChanged: (_) => onToggleCompletion(card.id, card.rowVersion),
                activeColor: Colors.teal,
              ),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      card.title,
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        decoration: card.isCompleted ? TextDecoration.lineThrough : null,
                        color: card.isCompleted ? Colors.grey.shade500 : Colors.black87,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: Colors.blueGrey.shade50,
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            card.categoryName,
                            style: TextStyle(fontSize: 11, color: Colors.blueGrey.shade700),
                          ),
                        ),
                        const SizedBox(width: 6),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: card.priority == 'CRITICAL'
                                ? Colors.red.shade50
                                : Colors.orange.shade50,
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            card.priority,
                            style: TextStyle(
                              fontSize: 10,
                              fontWeight: FontWeight.bold,
                              color: card.priority == 'CRITICAL'
                                  ? Colors.red.shade700
                                  : Colors.orange.shade700,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              PopupMenuButton<UrgencyStage>(
                tooltip: 'Chuyển giai đoạn',
                icon: const Icon(Icons.more_vert, size: 20),
                onSelected: (newStage) => onMoveStage(card.id, newStage, card.rowVersion),
                itemBuilder: (context) => UrgencyStage.values
                    .where((s) => s != stage.stage)
                    .map(
                      (s) => PopupMenuItem(
                        value: s,
                        child: Text('Chuyển sang: ${s.displayNameVi}'),
                      ),
                    )
                    .toList(),
              ),
            ],
          ),
          const SizedBox(height: 8),

          // Delegate Info
          Padding(
            padding: const EdgeInsets.only(left: 48),
            child: Row(
              children: [
                Icon(
                  Icons.person_outline,
                  size: 15,
                  color: card.assignedTrustedPersonName != null
                      ? Colors.teal.shade700
                      : (card.hasStageGap ? Colors.red.shade700 : Colors.grey),
                ),
                const SizedBox(width: 6),
                Expanded(
                  child: Text(
                    card.assignedTrustedPersonName != null
                        ? 'Ủy quyền: ${card.assignedTrustedPersonName}'
                        : (card.urgency == UrgencyStage.immediate ||
                                card.urgency == UrgencyStage.first72Hours
                            ? '⚠️ Chưa phân công người phụ trách'
                            : 'Chưa phân công'),
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: card.assignedTrustedPersonName != null
                          ? FontWeight.w500
                          : FontWeight.normal,
                      color: card.assignedTrustedPersonName != null
                          ? Colors.teal.shade800
                          : (card.hasStageGap ? Colors.red.shade700 : Colors.grey.shade600),
                    ),
                  ),
                ),
              ],
            ),
          ),

          // Document location hint
          const SizedBox(height: 4),
          Padding(
            padding: const EdgeInsets.only(left: 48),
            child: Row(
              children: [
                Icon(
                  Icons.folder_outlined,
                  size: 15,
                  color: card.hasDocumentLocation
                      ? Colors.indigo.shade600
                      : (card.hasStageGap ? Colors.amber.shade800 : Colors.grey),
                ),
                const SizedBox(width: 6),
                Expanded(
                  child: Text(
                    card.hasDocumentLocation
                        ? 'Hồ sơ: ${card.documentLocationHint}'
                        : (card.urgency == UrgencyStage.immediate ||
                                card.urgency == UrgencyStage.first72Hours
                            ? '⚠️ Chưa ghi chú nơi lưu hồ sơ'
                            : 'Chưa có thông tin hồ sơ'),
                    style: TextStyle(
                      fontSize: 12,
                      color: card.hasDocumentLocation
                          ? Colors.indigo.shade800
                          : (card.hasStageGap ? Colors.amber.shade900 : Colors.grey.shade600),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
