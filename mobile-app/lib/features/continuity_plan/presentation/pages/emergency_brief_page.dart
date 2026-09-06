import 'package:flutter/material.dart';
import '../../domain/entities/continuity_plan_entity.dart';

class EmergencyBriefPage extends StatelessWidget {
  final ContinuityPlanEntity plan;

  const EmergencyBriefPage({super.key, required this.plan});

  @override
  Widget build(BuildContext context) {
    // Filter only Immediate and First 72 Hours cards for emergency response
    final emergencyCards = plan.stages
        .where((s) =>
            s.stage == UrgencyStage.immediate ||
            s.stage == UrgencyStage.first72Hours)
        .expand((s) => s.cards)
        .toList();

    return Scaffold(
      appBar: AppBar(
        title: const Text('Sổ Tay Khẩn Cấp (24h/72h)'),
        backgroundColor: Colors.red.shade800,
        foregroundColor: Colors.white,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Zero-Knowledge Security Notice
            Container(
              padding: const EdgeInsets.all(14),
              decoration: BoxDecoration(
                color: Colors.blueGrey.shade50,
                border: Border.all(color: Colors.blueGrey.shade200),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Icon(Icons.security, color: Colors.blueGrey.shade800, size: 24),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Zero-Knowledge Emergency Packet',
                          style: TextStyle(
                            fontWeight: FontWeight.bold,
                            color: Colors.blueGrey.shade900,
                            fontSize: 14,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Tài liệu này cung cấp chỉ mục hành động nhanh trong 72 giờ đầu. Để đảm bảo an toàn tuyệt đối, hướng dẫn giải mã mật mã (Cipher Instructions) hoàn toàn KHÔNG được lưu tại đây.',
                          style: TextStyle(
                            fontSize: 12,
                            color: Colors.blueGrey.shade800,
                            height: 1.4,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),

            // Emergency Contacts
            Text(
              'Danh bạ khẩn cấp',
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
            ),
            const SizedBox(height: 8),
            _buildContactSection(context, emergencyCards),
            const SizedBox(height: 24),

            // Action Items
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Danh mục hành động ưu tiên cao',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                ),
                Text(
                  '${emergencyCards.length} thẻ',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    color: Colors.red.shade700,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            if (emergencyCards.isEmpty) ...[
              const Center(
                child: Padding(
                  padding: EdgeInsets.symmetric(vertical: 24),
                  child: Text('Không có đầu việc khẩn cấp nào.'),
                ),
              ),
            ] else ...[
              ...emergencyCards.map((card) => _buildEmergencyCardTile(card)),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildContactSection(BuildContext context, List<ContinuityPlanCardItemEntity> cards) {
    final Map<String, String> contacts = {};
    for (final card in cards) {
      if (card.assignedTrustedPersonName != null) {
        final phone = card.assignedTrustedPersonPhone ?? 'Chưa có SĐT';
        contacts[card.assignedTrustedPersonName!] = phone;
      }
    }

    if (contacts.isEmpty) {
      return Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: Colors.grey.shade100,
          borderRadius: BorderRadius.circular(8),
        ),
        child: const Text(
          'Chưa có người ủy thác nào được phân công cho các đầu việc khẩn cấp.',
          style: TextStyle(fontSize: 13, color: Colors.black54),
        ),
      );
    }

    return Card(
      elevation: 1,
      child: Column(
        children: contacts.entries.map((entry) {
          return ListTile(
            leading: const CircleAvatar(
              backgroundColor: Colors.teal,
              child: Icon(Icons.person, color: Colors.white, size: 20),
            ),
            title: Text(entry.key, style: const TextStyle(fontWeight: FontWeight.w600)),
            subtitle: Text('SĐT: ${entry.value}'),
            trailing: const Icon(Icons.phone, color: Colors.teal),
          );
        }).toList(),
      ),
    );
  }

  Widget _buildEmergencyCardTile(ContinuityPlanCardItemEntity card) {
    final isImmediate = card.urgency == UrgencyStage.immediate;

    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      shape: RoundedRectangleBorder(
        side: BorderSide(
          color: isImmediate ? Colors.red.shade300 : Colors.orange.shade300,
          width: 1.5,
        ),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                  decoration: BoxDecoration(
                    color: isImmediate ? Colors.red.shade700 : Colors.orange.shade700,
                    borderRadius: BorderRadius.circular(4),
                  ),
                  child: Text(
                    card.urgency.displayNameVi,
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 11,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    card.title,
                    style: const TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
            if (card.summary != null && card.summary!.isNotEmpty) ...[
              const SizedBox(height: 6),
              Text(
                card.summary!,
                style: const TextStyle(fontSize: 13, color: Colors.black87),
              ),
            ],
            const Divider(height: 16),
            Row(
              children: [
                const Icon(Icons.person, size: 16, color: Colors.teal),
                const SizedBox(width: 6),
                Text(
                  card.assignedTrustedPersonName != null
                      ? 'Người phụ trách: ${card.assignedTrustedPersonName}'
                      : 'Người phụ trách: Chưa phân công',
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w500,
                    color: card.assignedTrustedPersonName != null
                        ? Colors.teal.shade900
                        : Colors.red.shade700,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 4),
            Row(
              children: [
                const Icon(Icons.folder, size: 16, color: Colors.indigo),
                const SizedBox(width: 6),
                Expanded(
                  child: Text(
                    card.hasDocumentLocation
                        ? 'Nơi lưu hồ sơ gốc: ${card.documentLocationHint}'
                        : 'Nơi lưu hồ sơ: Chưa có thông tin',
                    style: TextStyle(
                      fontSize: 12,
                      color: card.hasDocumentLocation
                          ? Colors.indigo.shade900
                          : Colors.amber.shade900,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
