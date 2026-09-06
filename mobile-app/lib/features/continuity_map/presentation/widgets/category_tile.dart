import 'package:flutter/material.dart';
import '../../domain/entities/continuity_category_entity.dart';
import '../../domain/entities/continuity_item_entity.dart';

class CategoryTile extends StatelessWidget {
  final ContinuityCategoryEntity category;
  final bool isKeyUnlocked;
  final Function(ContinuityCategoryEntity) onAddItem;
  final Function(ContinuityItemEntity) onEditItem;
  final Function(String) onDeleteItem;
  final Function() onPromptUnlock;

  const CategoryTile({
    super.key,
    required this.category,
    required this.isKeyUnlocked,
    required this.onAddItem,
    required this.onEditItem,
    required this.onDeleteItem,
    required this.onPromptUnlock,
  });

  IconData _getCategoryIcon(String icon) {
    switch (icon) {
      case 'wallet':
        return Icons.account_balance_wallet_outlined;
      case 'home':
        return Icons.home_work_outlined;
      case 'shield':
        return Icons.health_and_safety_outlined;
      case 'briefcase':
        return Icons.business_center_outlined;
      case 'file-text':
        return Icons.description_outlined;
      default:
        return Icons.family_restroom_outlined;
    }
  }

  Color _getScoreColor(int score) {
    if (score >= 80) return const Color(0xFF10B981);
    if (score >= 50) return const Color(0xFFF59E0B);
    return const Color(0xFFF43F5E);
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final scoreColor = _getScoreColor(category.readinessScore);

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      decoration: BoxDecoration(
        color: const Color(0xFF1E293B).withOpacity(0.6),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: const Color(0xFF334155).withOpacity(0.5)),
      ),
      child: ExpansionTile(
        initiallyExpanded: category.items.isNotEmpty,
        shape: const Border(),
        leading: Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: const Color(0xFF0F172A),
            borderRadius: BorderRadius.circular(10),
          ),
          child: Icon(_getCategoryIcon(category.icon), color: Colors.tealAccent, size: 22),
        ),
        title: Text(
          category.name,
          style: theme.textTheme.titleSmall?.copyWith(
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        subtitle: Row(
          children: [
            Text(
              '${category.items.length} mục',
              style: const TextStyle(fontSize: 11, color: Colors.white60),
            ),
            const SizedBox(width: 8),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 1),
              decoration: BoxDecoration(
                color: scoreColor.withOpacity(0.15),
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: scoreColor.withOpacity(0.4)),
              ),
              child: Text(
                '${category.readinessScore}%',
                style: TextStyle(fontSize: 10, fontWeight: FontWeight.bold, color: scoreColor),
              ),
            ),
          ],
        ),
        trailing: IconButton(
          icon: const Icon(Icons.add_circle_outline, color: Colors.tealAccent, size: 22),
          onPressed: () => onAddItem(category),
        ),
        children: [
          if (category.items.isEmpty)
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 16),
              child: Text(
                'Chưa có hạng mục tiếp quản nào.',
                style: TextStyle(fontSize: 12, color: Colors.grey.shade500),
              ),
            )
          else
            ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
              itemCount: category.items.length,
              separatorBuilder: (_, __) => const Divider(color: Color(0xFF334155), height: 1),
              itemBuilder: (context, index) {
                final item = category.items[index];
                return ListTile(
                  contentPadding: EdgeInsets.zero,
                  dense: true,
                  title: Row(
                    children: [
                      Expanded(
                        child: Text(
                          item.name,
                          style: const TextStyle(
                            fontSize: 13,
                            fontWeight: FontWeight.w600,
                            color: Colors.white,
                          ),
                        ),
                      ),
                      if (item.hasContinuityGap)
                        Container(
                          margin: const EdgeInsets.only(left: 4),
                          padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 1),
                          decoration: BoxDecoration(
                            color: Colors.redAccent.withOpacity(0.2),
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: const Text('GAP', style: TextStyle(fontSize: 9, color: Colors.redAccent)),
                        ),
                      if (item.hasConfidentialNotes)
                        const Padding(
                          padding: EdgeInsets.only(left: 4),
                          child: Icon(Icons.lock, size: 14, color: Colors.tealAccent),
                        ),
                    ],
                  ),
                  subtitle: Text(
                    item.documentLocationHint ?? 'Chưa có vị trí hồ sơ',
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade400),
                  ),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        icon: const Icon(Icons.edit_outlined, size: 18, color: Colors.white60),
                        onPressed: () => onEditItem(item),
                      ),
                      IconButton(
                        icon: const Icon(Icons.delete_outline, size: 18, color: Colors.redAccent),
                        onPressed: () => onDeleteItem(item.id),
                      ),
                    ],
                  ),
                );
              },
            ),
        ],
      ),
    );
  }
}
