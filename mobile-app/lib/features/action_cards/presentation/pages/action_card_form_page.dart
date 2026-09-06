import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/crypto/crypto_service.dart';
import '../../domain/entities/action_card_enums.dart';
import '../bloc/action_card_bloc.dart';
import '../bloc/action_card_event.dart';

class ActionCardFormPage extends StatefulWidget {
  final UrgencyStage initialUrgency;

  const ActionCardFormPage({
    Key? key,
    this.initialUrgency = UrgencyStage.first72Hours,
  }) : super(key: key);

  @override
  State<ActionCardFormPage> createState() => _ActionCardFormPageState();
}

class _ActionCardFormPageState extends State<ActionCardFormPage> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _summaryController = TextEditingController();
  final _locationController = TextEditingController();
  final _digitalLinkController = TextEditingController();
  final _confidentialController = TextEditingController();
  final _passphraseController = TextEditingController();

  late UrgencyStage _selectedUrgency;
  String _selectedPriority = 'CRITICAL';
  String _selectedCategoryId = '018e6e5a-7341-789a-9e12-2d93e1104e01'; // Financial default
  bool _isSubmitting = false;
  String? _errorMessage;

  final List<Map<String, String>> _categories = [
    {'id': '018e6e5a-7341-789a-9e12-2d93e1104e01', 'name': 'Tài chính & Ngân hàng'},
    {'id': '018e6e5a-7341-789a-9e12-2d93e1104e02', 'name': 'Bất động sản & Tài sản'},
    {'id': '018e6e5a-7341-789a-9e12-2d93e1104e03', 'name': 'Bảo hiểm & Y tế'},
    {'id': '018e6e5a-7341-789a-9e12-2d93e1104e04', 'name': 'Vận hành Doanh nghiệp'},
    {'id': '018e6e5a-7341-789a-9e12-2d93e1104e05', 'name': 'Hồ sơ Pháp lý'},
    {'id': '018e6e5a-7341-789a-9e12-2d93e1104e06', 'name': 'Gia đình & Người phụ thuộc'},
  ];

  @override
  void initState() {
    super.initState();
    _selectedUrgency = widget.initialUrgency;
  }

  @override
  void dispose() {
    _titleController.dispose();
    _summaryController.dispose();
    _locationController.dispose();
    _digitalLinkController.dispose();
    _confidentialController.dispose();
    _passphraseController.dispose();
    super.dispose();
  }

  Future<void> _submitForm() async {
    if (!_formKey.currentState!.validate()) return;

    // Check sensitive data (Credit Card pattern) in plaintext
    final ccRegex = RegExp(r'\b(?:\d[ -]*?){13,19}\b');
    if (ccRegex.hasMatch(_titleController.text) ||
        ccRegex.hasMatch(_summaryController.text) ||
        ccRegex.hasMatch(_locationController.text)) {
      setState(() {
        _errorMessage = 'Phát hiện số thẻ trong văn bản thường! Vui lòng chỉ nhập thông tin nhạy cảm vào mục "Chỉ dẫn bảo mật".';
      });
      return;
    }

    setState(() {
      _isSubmitting = true;
      _errorMessage = null;
    });

    try {
      String? cipherNotesBlob;
      String? cipherNonce;
      String? cipherAuthTag;

      if (_confidentialController.text.trim().isNotEmpty) {
        if (_passphraseController.text.trim().isEmpty) {
          setState(() {
            _errorMessage = 'Vui lòng nhập Passphrase để mã hóa chỉ dẫn bảo mật.';
            _isSubmitting = false;
          });
          return;
        }

        final crypto = MobileCryptoService();
        final salt = utf8.encode('action-card-salt'.padRight(16, '0'));
        final key = await crypto.deriveMasterKey(_passphraseController.text.trim(), salt);
        final encrypted = await crypto.encrypt(key, _confidentialController.text.trim());

        cipherNotesBlob = encrypted.cipherNotesBlob;
        cipherNonce = encrypted.cipherNonce;
        cipherAuthTag = encrypted.cipherAuthTag;
      }

      context.read<ActionCardBloc>().add(
            CreateActionCardEvent(
              categoryId: _selectedCategoryId,
              title: _titleController.text.trim(),
              urgency: _selectedUrgency,
              priority: _selectedPriority,
              summary: _summaryController.text.trim().isNotEmpty ? _summaryController.text.trim() : null,
              documentLocationHint: _locationController.text.trim().isNotEmpty ? _locationController.text.trim() : null,
              digitalStorageLink: _digitalLinkController.text.trim().isNotEmpty ? _digitalLinkController.text.trim() : null,
              cipherInstructionsBlob: cipherNotesBlob,
              cipherNonce: cipherNonce,
              cipherAuthTag: cipherAuthTag,
            ),
          );

      Navigator.pop(context);
    } catch (e) {
      setState(() {
        _errorMessage = e.toString();
        _isSubmitting = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF020617),
      appBar: AppBar(
        backgroundColor: const Color(0xFF020617),
        elevation: 0,
        title: const Text('Tạo Thẻ Hành Động', style: TextStyle(color: Colors.white)),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (_errorMessage != null) ...[
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: const Color(0x33EF4444),
                    borderRadius: BorderRadius.circular(8),
                    border: Border.all(color: const Color(0x55EF4444)),
                  ),
                  child: Text(_errorMessage!, style: const TextStyle(color: Color(0xFFF87171), fontSize: 12)),
                ),
                const SizedBox(height: 16),
              ],

              // Title
              const Text('Tiêu đề thẻ hành động *', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 13)),
              const SizedBox(height: 6),
              TextFormField(
                controller: _titleController,
                style: const TextStyle(color: Colors.white),
                maxLength: 200,
                validator: (val) => val == null || val.trim().isEmpty ? 'Vui lòng nhập tiêu đề' : null,
                decoration: InputDecoration(
                  hintText: 'VD: Xử lý khoản vay ngân hàng Agribank',
                  hintStyle: const TextStyle(color: Color(0xFF64748B)),
                  filled: true,
                  fillColor: const Color(0xFF0F172A),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),

              const SizedBox(height: 12),

              // Category Dropdown
              const Text('Danh mục tài sản', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 13)),
              const SizedBox(height: 6),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 12),
                decoration: BoxDecoration(
                  color: const Color(0xFF0F172A),
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: const Color(0xFF1E293B)),
                ),
                child: DropdownButtonHideUnderline(
                  child: DropdownButton<String>(
                    value: _selectedCategoryId,
                    isExpanded: true,
                    dropdownColor: const Color(0xFF0F172A),
                    style: const TextStyle(color: Colors.white, fontSize: 13),
                    items: _categories.map((c) {
                      return DropdownMenuItem<String>(
                        value: c['id'],
                        child: Text(c['name']!),
                      );
                    }).toList(),
                    onChanged: (val) {
                      if (val != null) setState(() => _selectedCategoryId = val);
                    },
                  ),
                ),
              ),

              const SizedBox(height: 14),

              // Urgency & Priority row
              Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text('Khung thời gian', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 13)),
                        const SizedBox(height: 6),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 10),
                          decoration: BoxDecoration(
                            color: const Color(0xFF0F172A),
                            borderRadius: BorderRadius.circular(10),
                            border: Border.all(color: const Color(0xFF1E293B)),
                          ),
                          child: DropdownButtonHideUnderline(
                            child: DropdownButton<UrgencyStage>(
                              value: _selectedUrgency,
                              isExpanded: true,
                              dropdownColor: const Color(0xFF0F172A),
                              style: const TextStyle(color: Colors.white, fontSize: 12),
                              items: UrgencyStage.values.map((u) {
                                return DropdownMenuItem<UrgencyStage>(
                                  value: u,
                                  child: Text(u.displayNameVi),
                                );
                              }).toList(),
                              onChanged: (val) {
                                if (val != null) setState(() => _selectedUrgency = val);
                              },
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text('Mức ưu tiên', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 13)),
                        const SizedBox(height: 6),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 10),
                          decoration: BoxDecoration(
                            color: const Color(0xFF0F172A),
                            borderRadius: BorderRadius.circular(10),
                            border: Border.all(color: const Color(0xFF1E293B)),
                          ),
                          child: DropdownButtonHideUnderline(
                            child: DropdownButton<String>(
                              value: _selectedPriority,
                              isExpanded: true,
                              dropdownColor: const Color(0xFF0F172A),
                              style: const TextStyle(color: Colors.white, fontSize: 12),
                              items: const [
                                DropdownMenuItem(value: 'CRITICAL', child: Text('🔴 CRITICAL')),
                                DropdownMenuItem(value: 'IMPORTANT', child: Text('🟡 IMPORTANT')),
                                DropdownMenuItem(value: 'LOW', child: Text('🔵 LOW')),
                              ],
                              onChanged: (val) {
                                if (val != null) setState(() => _selectedPriority = val);
                              },
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),

              const SizedBox(height: 14),

              // Summary
              const Text('Tóm tắt bối cảnh', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 13)),
              const SizedBox(height: 6),
              TextFormField(
                controller: _summaryController,
                style: const TextStyle(color: Colors.white),
                maxLines: 2,
                maxLength: 1000,
                decoration: InputDecoration(
                  hintText: 'Mô tả bối cảnh và mục tiêu xử lý của thẻ...',
                  hintStyle: const TextStyle(color: Color(0xFF64748B)),
                  filled: true,
                  fillColor: const Color(0xFF0F172A),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),

              const SizedBox(height: 14),

              // Document Location Hint
              const Text('Vị trí lưu trữ tài liệu gốc', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 13)),
              const SizedBox(height: 6),
              TextFormField(
                controller: _locationController,
                style: const TextStyle(color: Colors.white),
                maxLength: 255,
                decoration: InputDecoration(
                  hintText: 'VD: Két sắt phòng ngủ, ngăn bên trái',
                  hintStyle: const TextStyle(color: Color(0xFF64748B)),
                  filled: true,
                  fillColor: const Color(0xFF0F172A),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),

              const SizedBox(height: 14),

              // Digital Storage Link
              const Text('Liên kết số / Cloud Link', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 13)),
              const SizedBox(height: 6),
              TextFormField(
                controller: _digitalLinkController,
                style: const TextStyle(color: Colors.white),
                maxLength: 500,
                decoration: InputDecoration(
                  hintText: 'VD: https://drive.google.com/...',
                  hintStyle: const TextStyle(color: Color(0xFF64748B)),
                  filled: true,
                  fillColor: const Color(0xFF0F172A),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),

              const SizedBox(height: 16),

              // Zero-Knowledge Encrypted Box
              Container(
                padding: const EdgeInsets.all(14),
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
                        Icon(Icons.lock, size: 16, color: Color(0xFFC084FC)),
                        SizedBox(width: 6),
                        Text(
                          'Chỉ dẫn bảo mật (Mã hóa AES-256 trên thiết bị)',
                          style: TextStyle(color: Color(0xFFE9D5FF), fontWeight: FontWeight.bold, fontSize: 12),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    TextFormField(
                      controller: _confidentialController,
                      style: const TextStyle(color: Colors.white, fontFamily: 'monospace', fontSize: 12),
                      maxLines: 2,
                      decoration: InputDecoration(
                        hintText: 'Mã PIN két sắt, mã ủy quyền bí mật...',
                        hintStyle: const TextStyle(color: Color(0xFF64748B)),
                        filled: true,
                        fillColor: const Color(0xFF0F172A),
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                      ),
                    ),
                    const SizedBox(height: 8),
                    TextFormField(
                      controller: _passphraseController,
                      obscureText: true,
                      style: const TextStyle(color: Colors.white, fontSize: 12),
                      decoration: InputDecoration(
                        hintText: 'Passphrase bí mật để mã hóa...',
                        hintStyle: const TextStyle(color: Color(0xFF64748B)),
                        filled: true,
                        fillColor: const Color(0xFF0F172A),
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 24),

              // Submit Button
              SizedBox(
                width: double.infinity,
                height: 48,
                child: ElevatedButton(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF0284C7),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                  ),
                  onPressed: _isSubmitting ? null : _submitForm,
                  child: Text(
                    _isSubmitting ? 'Đang lưu...' : 'Lưu Thẻ Hành Động',
                    style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: Colors.white),
                  ),
                ),
              ),
              const SizedBox(height: 30),
            ],
          ),
        ),
      ),
    );
  }
}
