import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/crypto/crypto_service.dart';
import '../../domain/entities/action_card_entity.dart';
import '../bloc/action_card_bloc.dart';
import '../bloc/action_card_event.dart';
import '../bloc/action_card_state.dart';
import '../widgets/urgency_stage_badge.dart';
import '../widgets/reorderable_step_list.dart';
import '../widgets/contact_card_tile.dart';

class ActionCardDetailPage extends StatefulWidget {
  final String cardId;

  const ActionCardDetailPage({Key? key, required this.cardId}) : super(key: key);

  @override
  State<ActionCardDetailPage> createState() => _ActionCardDetailPageState();
}

class _ActionCardDetailPageState extends State<ActionCardDetailPage> {
  final TextEditingController _passphraseController = TextEditingController();
  final TextEditingController _stepInstructionController = TextEditingController();
  final TextEditingController _stepDurationController = TextEditingController();

  // Decryption state
  String? _decryptedText;
  String? _decryptError;
  bool _isDecrypting = false;

  @override
  void dispose() {
    _passphraseController.dispose();
    _stepInstructionController.dispose();
    _stepDurationController.dispose();
    super.dispose();
  }

  Future<void> _decryptInstructions(ActionCardEntity card) async {
    final pass = _passphraseController.text.trim();
    if (pass.isEmpty || card.cipherInstructionsBlob == null || card.cipherNonce == null || card.cipherAuthTag == null) {
      return;
    }

    setState(() {
      _isDecrypting = true;
      _decryptError = null;
    });

    try {
      final crypto = MobileCryptoService();
      final salt = utf8.encode(card.id.substring(0, 16).padRight(16, '0'));
      final key = await crypto.deriveMasterKey(pass, salt);
      final plain = await crypto.decrypt(
        key: key,
        cipherNotesBlob: card.cipherInstructionsBlob!,
        cipherNonce: card.cipherNonce!,
        cipherAuthTag: card.cipherAuthTag!,
      );

      setState(() {
        _decryptedText = plain;
      });
    } catch (e) {
      setState(() {
        _decryptError = 'Mật khẩu giải mã không chính xác hoặc dữ liệu bị thay đổi.';
      });
    } finally {
      setState(() {
        _isDecrypting = false;
      });
    }
  }

  void _showAddStepDialog(BuildContext context, String cardId) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: const Color(0xFF0F172A),
        title: const Text('Thêm Bước Hành Động', style: TextStyle(color: Colors.white, fontSize: 16)),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: _stepInstructionController,
              style: const TextStyle(color: Colors.white),
              maxLength: 500,
              decoration: const InputDecoration(
                hintText: 'Nội dung bước hành động...',
                hintStyle: TextStyle(color: Color(0xFF64748B)),
              ),
            ),
            const SizedBox(height: 8),
            TextField(
              controller: _stepDurationController,
              style: const TextStyle(color: Colors.white),
              maxLength: 50,
              decoration: const InputDecoration(
                hintText: 'Thời gian ước tính (VD: 30 phút)...',
                hintStyle: TextStyle(color: Color(0xFF64748B)),
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx),
            child: const Text('Hủy', style: TextStyle(color: Color(0xFF94A3B8))),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: const Color(0xFF0284C7)),
            onPressed: () {
              final instruction = _stepInstructionController.text.trim();
              final duration = _stepDurationController.text.trim();
              if (instruction.isNotEmpty) {
                context.read<ActionCardBloc>().add(
                      AddStepEvent(
                        cardId: cardId,
                        instruction: instruction,
                        estimatedDuration: duration.isNotEmpty ? duration : null,
                      ),
                    );
                _stepInstructionController.clear();
                _stepDurationController.clear();
                Navigator.pop(ctx);
              }
            },
            child: const Text('Thêm'),
          ),
        ],
      ),
    );
  }

  void _showAddContactDialog(BuildContext context, String cardId) {
    final nameCtrl = TextEditingController();
    final roleCtrl = TextEditingController();
    final phoneCtrl = TextEditingController();
    final emailCtrl = TextEditingController();
    final notesCtrl = TextEditingController();

    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: const Color(0xFF0F172A),
        title: const Text('Thêm Đầu Mối Liên Hệ', style: TextStyle(color: Colors.white, fontSize: 16)),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: nameCtrl,
                style: const TextStyle(color: Colors.white),
                decoration: const InputDecoration(
                  labelText: 'Họ tên *',
                  labelStyle: TextStyle(color: Color(0xFF94A3B8)),
                ),
              ),
              TextField(
                controller: roleCtrl,
                style: const TextStyle(color: Colors.white),
                decoration: const InputDecoration(
                  labelText: 'Vai trò (VD: Luật sư, Cán bộ tín dụng) *',
                  labelStyle: TextStyle(color: Color(0xFF94A3B8)),
                ),
              ),
              TextField(
                controller: phoneCtrl,
                style: const TextStyle(color: Colors.white),
                keyboardType: TextInputType.phone,
                decoration: const InputDecoration(
                  labelText: 'Số điện thoại',
                  labelStyle: TextStyle(color: Color(0xFF94A3B8)),
                ),
              ),
              TextField(
                controller: emailCtrl,
                style: const TextStyle(color: Colors.white),
                keyboardType: TextInputType.emailAddress,
                decoration: const InputDecoration(
                  labelText: 'Email',
                  labelStyle: TextStyle(color: Color(0xFF94A3B8)),
                ),
              ),
              TextField(
                controller: notesCtrl,
                style: const TextStyle(color: Colors.white),
                decoration: const InputDecoration(
                  labelText: 'Ghi chú thêm',
                  labelStyle: TextStyle(color: Color(0xFF94A3B8)),
                ),
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx),
            child: const Text('Hủy', style: TextStyle(color: Color(0xFF94A3B8))),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: const Color(0xFF0284C7)),
            onPressed: () {
              final name = nameCtrl.text.trim();
              final role = roleCtrl.text.trim();
              if (name.isNotEmpty && role.isNotEmpty) {
                context.read<ActionCardBloc>().add(
                      AddContactEvent(
                        cardId: cardId,
                        contactName: name,
                        relationshipOrRole: role,
                        phoneNumber: phoneCtrl.text.trim().isNotEmpty ? phoneCtrl.text.trim() : null,
                        email: emailCtrl.text.trim().isNotEmpty ? emailCtrl.text.trim() : null,
                        contactNotes: notesCtrl.text.trim().isNotEmpty ? notesCtrl.text.trim() : null,
                      ),
                    );
                Navigator.pop(ctx);
              }
            },
            child: const Text('Lưu'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<ActionCardBloc, ActionCardState>(
      builder: (context, state) {
        if (state is! ActionCardLoaded) {
          return const Scaffold(
            backgroundColor: Color(0xFF020617),
            body: Center(child: CircularProgressIndicator(color: Color(0xFF38BDF8))),
          );
        }

        final cardList = state.allCards.where((c) => c.id == widget.cardId).toList();
        if (cardList.isEmpty) {
          return Scaffold(
            backgroundColor: const Color(0xFF020617),
            appBar: AppBar(backgroundColor: const Color(0xFF020617)),
            body: const Center(
              child: Text('Thẻ hành động không tồn tại hoặc đã bị xóa.', style: TextStyle(color: Colors.white70)),
            ),
          );
        }

        final card = cardList.first;

        return Scaffold(
          backgroundColor: const Color(0xFF020617),
          appBar: AppBar(
            backgroundColor: const Color(0xFF020617),
            elevation: 0,
            title: const Text('Chi Tiết Thẻ', style: TextStyle(color: Colors.white)),
            actions: [
              IconButton(
                icon: const Icon(Icons.delete_outline, color: Colors.redAccent),
                tooltip: 'Xóa thẻ',
                onPressed: () {
                  showDialog(
                    context: context,
                    builder: (ctx) => AlertDialog(
                      backgroundColor: const Color(0xFF0F172A),
                      title: const Text('Xác nhận xóa thẻ', style: TextStyle(color: Colors.white)),
                      content: Text(
                        'Bạn có chắc chắn muốn xóa "${card.title}" không?',
                        style: const TextStyle(color: Color(0xFF94A3B8)),
                      ),
                      actions: [
                        TextButton(
                          onPressed: () => Navigator.pop(ctx),
                          child: const Text('Hủy', style: TextStyle(color: Color(0xFF94A3B8))),
                        ),
                        ElevatedButton(
                          style: ElevatedButton.styleFrom(backgroundColor: Colors.redAccent),
                          onPressed: () {
                            context.read<ActionCardBloc>().add(DeleteActionCardEvent(card.id));
                            Navigator.pop(ctx); // Close dialog
                            Navigator.pop(context); // Close page
                          },
                          child: const Text('Xóa'),
                        ),
                      ],
                    ),
                  );
                },
              ),
            ],
          ),
          body: SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Header tags
                Row(
                  children: [
                    UrgencyStageBadge(stage: card.urgency),
                    const SizedBox(width: 8),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: card.priority == 'CRITICAL'
                            ? const Color(0x33EF4444)
                            : const Color(0x33F59E0B),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        card.priority,
                        style: TextStyle(
                          color: card.priority == 'CRITICAL' ? const Color(0xFFF87171) : const Color(0xFFFBBF24),
                          fontSize: 11,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                    const Spacer(),
                    if (card.isCompleted)
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                        decoration: BoxDecoration(
                          color: const Color(0x3310B981),
                          borderRadius: BorderRadius.circular(6),
                        ),
                        child: const Text(
                          '✓ Hoàn tất',
                          style: TextStyle(color: Color(0xFF34D399), fontSize: 11, fontWeight: FontWeight.bold),
                        ),
                      ),
                  ],
                ),
                const SizedBox(height: 12),

                // Title
                Text(
                  card.title,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                  ),
                ),

                // Summary
                if (card.summary != null && card.summary!.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Container(
                    width: double.infinity,
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: const Color(0xFF0F172A),
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: const Color(0xFF1E293B)),
                    ),
                    child: Text(
                      card.summary!,
                      style: const TextStyle(color: Color(0xFF94A3B8), fontSize: 13),
                    ),
                  ),
                ],

                const SizedBox(height: 16),

                // Physical & Digital Storage Hints
                Row(
                  children: [
                    Expanded(
                      child: Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: const Color(0xFF0F172A),
                          borderRadius: BorderRadius.circular(10),
                          border: Border.all(color: const Color(0xFF1E293B)),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text('Vị trí hồ sơ gốc:', style: TextStyle(color: Color(0xFF64748B), fontSize: 11)),
                            const SizedBox(height: 4),
                            Text(
                              card.documentLocationHint ?? 'Chưa ghi nhận',
                              style: const TextStyle(color: Colors.white, fontSize: 12, fontWeight: FontWeight.w600),
                            ),
                          ],
                        ),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: const Color(0xFF0F172A),
                          borderRadius: BorderRadius.circular(10),
                          border: Border.all(color: const Color(0xFF1E293B)),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text('Lưu trữ số / Cloud:', style: TextStyle(color: Color(0xFF64748B), fontSize: 11)),
                            const SizedBox(height: 4),
                            Text(
                              card.digitalStorageLink ?? 'Không có',
                              overflow: TextOverflow.ellipsis,
                              style: const TextStyle(color: Color(0xFF38BDF8), fontSize: 12, fontWeight: FontWeight.w600),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),

                const SizedBox(height: 20),

                // Zero-Knowledge Confidential Section
                if (card.hasConfidentialInstructions) ...[
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: const Color(0x22A855F7),
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(color: const Color(0x55A855F7)),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: const [
                            Icon(Icons.lock, size: 18, color: Color(0xFFC084FC)),
                            SizedBox(width: 6),
                            Text(
                              'Chỉ dẫn bảo mật (Zero-Knowledge)',
                              style: TextStyle(color: Color(0xFFE9D5FF), fontWeight: FontWeight.bold, fontSize: 13),
                            ),
                          ],
                        ),
                        const SizedBox(height: 6),
                        const Text(
                          'Nội dung được mã hóa AES-256-GCM. Nhập Master Passphrase để giải mã:',
                          style: TextStyle(color: Color(0xFFC084FC), fontSize: 11),
                        ),
                        const SizedBox(height: 10),
                        if (_decryptedText != null)
                          Container(
                            width: double.infinity,
                            padding: const EdgeInsets.all(12),
                            decoration: BoxDecoration(
                              color: const Color(0x3310B981),
                              borderRadius: BorderRadius.circular(8),
                              border: Border.all(color: const Color(0x5510B981)),
                            ),
                            child: Text(
                              _decryptedText!,
                              style: const TextStyle(color: Colors.white, fontFamily: 'monospace', fontSize: 13),
                            ),
                          )
                        else ...[
                          Row(
                            children: [
                              Expanded(
                                child: TextField(
                                  controller: _passphraseController,
                                  obscureText: true,
                                  style: const TextStyle(color: Colors.white),
                                  decoration: InputDecoration(
                                    hintText: 'Nhập Passphrase...',
                                    hintStyle: const TextStyle(color: Color(0xFF64748B)),
                                    filled: true,
                                    fillColor: const Color(0xFF0F172A),
                                    contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                                  ),
                                ),
                              ),
                              const SizedBox(width: 8),
                              ElevatedButton(
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: const Color(0xFF9333EA),
                                  padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                                ),
                                onPressed: _isDecrypting ? null : () => _decryptInstructions(card),
                                child: Text(_isDecrypting ? '...' : 'Giải mã'),
                              ),
                            ],
                          ),
                          if (_decryptError != null) ...[
                            const SizedBox(height: 6),
                            Text(_decryptError!, style: const TextStyle(color: Colors.redAccent, fontSize: 11)),
                          ],
                        ],
                      ],
                    ),
                  ),
                  const SizedBox(height: 20),
                ],

                // Action Steps Section
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Các bước hành động (${card.steps.length}/20)',
                      style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 16),
                    ),
                    if (card.steps.length < 20)
                      TextButton.icon(
                        icon: const Icon(Icons.add, size: 16, color: Color(0xFF38BDF8)),
                        label: const Text('Thêm bước', style: TextStyle(color: Color(0xFF38BDF8), fontSize: 12)),
                        onPressed: () => _showAddStepDialog(context, card.id),
                      ),
                  ],
                ),
                const SizedBox(height: 8),
                ReorderableStepList(
                  steps: card.steps,
                  onToggleStep: (stepId, isCompleted) {
                    final step = card.steps.firstWhere((s) => s.id == stepId);
                    context.read<ActionCardBloc>().add(
                          ToggleStepCompletionEvent(
                            cardId: card.id,
                            stepId: stepId,
                            instruction: step.instruction,
                            estimatedDuration: step.estimatedDuration,
                            isCompleted: isCompleted,
                          ),
                        );
                  },
                  onReorder: (reorderedIds) {
                    context.read<ActionCardBloc>().add(
                          ReorderStepsEvent(
                            cardId: card.id,
                            orderedStepIds: reorderedIds,
                          ),
                        );
                  },
                  onDeleteStep: (stepId) {
                    context.read<ActionCardBloc>().add(
                          DeleteStepEvent(
                            cardId: card.id,
                            stepId: stepId,
                          ),
                        );
                  },
                ),

                const SizedBox(height: 24),

                // Key Contacts Section
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Đầu mối liên hệ (${card.contacts.length}/5)',
                      style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 16),
                    ),
                    if (card.contacts.length < 5)
                      TextButton.icon(
                        icon: const Icon(Icons.add, size: 16, color: Color(0xFF38BDF8)),
                        label: const Text('Thêm liên hệ', style: TextStyle(color: Color(0xFF38BDF8), fontSize: 12)),
                        onPressed: () => _showAddContactDialog(context, card.id),
                      ),
                  ],
                ),
                const SizedBox(height: 8),
                if (card.contacts.isEmpty)
                  Container(
                    width: double.infinity,
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: const Color(0xFF0F172A),
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: const Color(0xFF1E293B)),
                    ),
                    child: const Center(
                      child: Text(
                        'Chưa có đầu mối liên hệ được gán.',
                        style: TextStyle(color: Color(0xFF64748B), fontSize: 12),
                      ),
                    ),
                  )
                else
                  ...card.contacts.map(
                    (contact) => ContactCardTile(
                      contact: contact,
                      onDelete: () {
                        context.read<ActionCardBloc>().add(
                              DeleteContactEvent(
                                cardId: card.id,
                                contactId: contact.id,
                              ),
                            );
                      },
                    ),
                  ),
                const SizedBox(height: 40),
              ],
            ),
          ),
        );
      },
    );
  }
}
