import 'package:flutter/material.dart';
import '../../domain/entities/trusted_person_entity.dart';

class TrustedPersonCard extends StatelessWidget {
  final TrustedPersonEntity person;
  final VoidCallback onRevoke;
  final VoidCallback onRegenerateCode;

  const TrustedPersonCard({
    super.key,
    required this.person,
    required this.onRevoke,
    required this.onRegenerateCode,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isInvited = person.status == TrustedPersonStatus.invited;

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  backgroundColor: theme.colorScheme.primaryContainer,
                  child: Text(
                    person.fullName.isNotEmpty ? person.fullName[0].toUpperCase() : '?',
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: theme.colorScheme.onPrimaryContainer,
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        person.fullName,
                        style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold),
                      ),
                      Text(
                        person.relationship,
                        style: theme.textTheme.bodySmall?.copyWith(color: theme.colorScheme.primary),
                      ),
                    ],
                  ),
                ),
                _buildStatusChip(context, person.status),
              ],
            ),
            const Divider(height: 24),
            _buildInfoRow(Icons.email_outlined, person.email),
            const SizedBox(height: 4),
            _buildInfoRow(Icons.phone_outlined, person.phoneNumber),
            const SizedBox(height: 4),
            _buildInfoRow(Icons.shield_outlined, person.trustLevelLabel),
            if (person.roleDescription != null && person.roleDescription!.isNotEmpty) ...[
              const SizedBox(height: 8),
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: theme.colorScheme.surfaceVariant.withOpacity(0.5),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  '"${person.roleDescription}"',
                  style: theme.textTheme.bodySmall?.copyWith(fontStyle: FontStyle.italic),
                ),
              ),
            ],
            const SizedBox(height: 12),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                if (isInvited && person.activePairingCode != null)
                  Chip(
                    avatar: const Icon(Icons.key, size: 16),
                    label: Text(
                      'Mã: ${person.activePairingCode}',
                      style: const TextStyle(fontWeight: FontWeight.bold, letterSpacing: 1.2),
                    ),
                    backgroundColor: Colors.amber.withOpacity(0.2),
                  )
                else
                  Text(
                    'Phân quyền: ${person.permissions.length} mục',
                    style: theme.textTheme.bodySmall,
                  ),
                Row(
                  children: [
                    if (isInvited)
                      IconButton(
                        icon: const Icon(Icons.refresh, size: 20),
                        tooltip: 'Cấp lại mã',
                        onPressed: onRegenerateCode,
                      ),
                    IconButton(
                      icon: const Icon(Icons.delete_outline, size: 20, color: Colors.redAccent),
                      tooltip: 'Thu hồi',
                      onPressed: onRevoke,
                    ),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildStatusChip(BuildContext context, TrustedPersonStatus status) {
    Color color;
    switch (status) {
      case TrustedPersonStatus.active:
        color = Colors.green;
        break;
      case TrustedPersonStatus.invited:
        color = Colors.amber;
        break;
      case TrustedPersonStatus.suspended:
        color = Colors.grey;
        break;
      case TrustedPersonStatus.revoked:
        color = Colors.red;
        break;
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: color.withOpacity(0.15),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withOpacity(0.5)),
      ),
      child: Text(
        status.displayNameVi,
        style: TextStyle(color: color, fontSize: 11, fontWeight: FontWeight.bold),
      ),
    );
  }

  Widget _buildInfoRow(IconData icon, String text) {
    return Row(
      children: [
        Icon(icon, size: 16, color: Colors.grey),
        const SizedBox(width: 8),
        Expanded(
          child: Text(
            text,
            style: const TextStyle(fontSize: 13),
            overflow: TextOverflow.ellipsis,
          ),
        ),
      ],
    );
  }
}
