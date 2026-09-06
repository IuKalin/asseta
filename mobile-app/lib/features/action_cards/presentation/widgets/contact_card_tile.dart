import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';
import '../../domain/entities/action_contact_entity.dart';

class ContactCardTile extends StatelessWidget {
  final ActionContactEntity contact;
  final VoidCallback? onDelete;

  const ContactCardTile({
    Key? key,
    required this.contact,
    this.onDelete,
  }) : super(key: key);

  Future<void> _makeCall(String number) async {
    final uri = Uri.parse('tel:$number');
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri);
    }
  }

  Future<void> _sendEmail(String email) async {
    final uri = Uri.parse('mailto:$email');
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFF0F172A), // Slate 900
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: const Color(0xFF1E293B)), // Slate 800
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: const Color(0xFF1E293B),
              borderRadius: BorderRadius.circular(8),
            ),
            child: const Icon(Icons.person, size: 20, color: Color(0xFF38BDF8)),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  contact.contactName,
                  style: const TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                    fontSize: 14,
                  ),
                ),
                Text(
                  contact.relationshipOrRole,
                  style: const TextStyle(
                    color: Color(0xFF94A3B8), // Slate 400
                    fontSize: 12,
                  ),
                ),
                const SizedBox(height: 6),
                Row(
                  children: [
                    if (contact.phoneNumber != null && contact.phoneNumber!.isNotEmpty)
                      InkWell(
                        onTap: () => _makeCall(contact.phoneNumber!),
                        child: Row(
                          children: [
                            const Icon(Icons.phone, size: 14, color: Color(0xFF38BDF8)),
                            const SizedBox(width: 4),
                            Text(
                              contact.phoneNumber!,
                              style: const TextStyle(
                                color: Color(0xFF38BDF8),
                                fontSize: 12,
                              ),
                            ),
                            const SizedBox(width: 12),
                          ],
                        ),
                      ),
                    if (contact.email != null && contact.email!.isNotEmpty)
                      InkWell(
                        onTap: () => _sendEmail(contact.email!),
                        child: Row(
                          children: [
                            const Icon(Icons.email, size: 14, color: Color(0xFF38BDF8)),
                            const SizedBox(width: 4),
                            Text(
                              contact.email!,
                              style: const TextStyle(
                                color: Color(0xFF38BDF8),
                                fontSize: 12,
                              ),
                            ),
                          ],
                        ),
                      ),
                  ],
                ),
                if (contact.contactNotes != null && contact.contactNotes!.isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Text(
                    '"${contact.contactNotes}"',
                    style: const TextStyle(
                      color: Color(0xFF64748B),
                      fontSize: 11,
                      fontStyle: FontStyle.italic,
                    ),
                  ),
                ],
              ],
            ),
          ),
          if (onDelete != null)
            IconButton(
              icon: const Icon(Icons.close, size: 16, color: Color(0xFF64748B)),
              onPressed: onDelete,
              tooltip: 'Xóa đầu mối này',
            ),
        ],
      ),
    );
  }
}
