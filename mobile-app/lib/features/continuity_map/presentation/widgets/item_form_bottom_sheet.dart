import 'package:flutter/material.dart';
import '../../domain/entities/continuity_category_entity.dart';
import '../../domain/entities/continuity_item_entity.dart';

class ItemFormBottomSheet extends StatefulWidget {
  final List<ContinuityCategoryEntity> categories;
  final ContinuityCategoryEntity? initialCategory;
  final ContinuityItemEntity? itemToEdit;
  final bool isKeyUnlocked;
  final Function(String categoryId, String name, ItemPriority priority, String? hint, String? notes)
      onSubmitCreate;
  final Function(String id, String name, ItemPriority priority, String? hint, String? notes, int rowVersion)
      onSubmitUpdate;
  final Function(String passphrase) onUnlockKey;

  const ItemFormBottomSheet({
    super.key,
    required this.categories,
    this.initialCategory,
    this.itemToEdit,
    required this.isKeyUnlocked,
    required this.onSubmitCreate,
    required this.onSubmitUpdate,
    required this.onUnlockKey,
  });

  @override
  State<ItemFormBottomSheet> createState() => _ItemFormBottomSheetState();
}

class _ItemFormBottomSheetState extends State<ItemFormBottomSheet> {
  late String _selectedCategoryId;
  late TextEditingController _nameController;
  late TextEditingController _hintController;
  late TextEditingController _notesController;
  late TextEditingController _passphraseController;
  ItemPriority _selectedPriority = ItemPriority.important;
  String? _error;

  @override
  void initState() {
    super.initState();
    _selectedCategoryId = widget.itemToEdit?.categoryId ??
        widget.initialCategory?.categoryId ??
        (widget.categories.isNotEmpty ? widget.categories.first.categoryId : '');

    _nameController = TextEditingController(text: widget.itemToEdit?.name ?? '');
    _hintController = TextEditingController(text: widget.itemToEdit?.documentLocationHint ?? '');
    _notesController = TextEditingController();
    _passphraseController = TextEditingController();

    if (widget.itemToEdit != null) {
      _selectedPriority = widget.itemToEdit!.priority;
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _hintController.dispose();
    _notesController.dispose();
    _passphraseController.dispose();
    super.dispose();
  }

  void _handleSubmit() {
    final name = _nameController.text.trim();
    if (name.isEmpty) {
      setState(() => _error = 'Tên hạng mục không được để trống.');
      return;
    }

    final notes = _notesController.text.trim();
    if (notes.isNotEmpty && !widget.isKeyUnlocked) {
      final pass = _passphraseController.text.trim();
      if (pass.isEmpty) {
        setState(() => _error = 'Vui lòng nhập Passphrase để mã hóa ghi chú bí mật.');
        return;
      }
      widget.onUnlockKey(pass);
    }

    final hint = _hintController.text.trim();

    if (widget.itemToEdit != null) {
      widget.onSubmitUpdate(
        widget.itemToEdit!.id,
        name,
        _selectedPriority,
        hint.isEmpty ? null : hint,
        notes.isEmpty ? null : notes,
        widget.itemToEdit!.rowVersion,
      );
    } else {
      widget.onSubmitCreate(
        _selectedCategoryId,
        name,
        _selectedPriority,
        hint.isEmpty ? null : hint,
        notes.isEmpty ? null : notes,
      );
    }

    Navigator.of(context).pop();
  }

  @override
  Widget build(BuildContext context) {
    final isEditing = widget.itemToEdit != null;

    return Container(
      padding: EdgeInsets.only(
        left: 20,
        right: 20,
        top: 20,
        bottom: MediaQuery.of(context).viewInsets.bottom + 20,
      ),
      decoration: const BoxDecoration(
        color: Color(0xFF0F172A),
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      child: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  isEditing ? 'Chỉnh Sửa Hạng Mục' : 'Thêm Hạng Mục Tiếp Quản',
                  style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.white),
                ),
                IconButton(
                  icon: const Icon(Icons.close, color: Colors.white60),
                  onPressed: () => Navigator.of(context).pop(),
                ),
              ],
            ),
            if (_error != null)
              Container(
                margin: const EdgeInsets.only(bottom: 12),
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: Colors.red.shade900.withOpacity(0.3),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.red.shade700),
                ),
                child: Text(_error!, style: const TextStyle(color: Colors.redAccent, fontSize: 12)),
              ),
            if (!isEditing && widget.categories.isNotEmpty)
              DropdownButtonFormField<String>(
                value: _selectedCategoryId,
                dropdownColor: const Color(0xFF1E293B),
                style: const TextStyle(color: Colors.white, fontSize: 13),
                decoration: const InputDecoration(
                  labelText: 'Danh mục',
                  labelStyle: TextStyle(color: Colors.white70, fontSize: 12),
                ),
                items: widget.categories.map<DropdownMenuItem<String>>((c) {
                  return DropdownMenuItem<String>(value: c.categoryId, child: Text(c.name));
                }).toList(),
                onChanged: (val) {
                  if (val != null) setState(() => _selectedCategoryId = val);
                },
              ),
            const SizedBox(height: 10),
            TextField(
              controller: _nameController,
              style: const TextStyle(color: Colors.white, fontSize: 13),
              decoration: const InputDecoration(
                labelText: 'Tên tài sản / nghĩa vụ *',
                labelStyle: TextStyle(color: Colors.white70, fontSize: 12),
                hintText: 'VD: Sổ tiết kiệm Vietcombank, Hợp đồng thuê nhà...',
                hintStyle: TextStyle(color: Colors.white30, fontSize: 12),
              ),
            ),
            const SizedBox(height: 12),
            const Text('Mức độ ưu tiên tiếp quản', style: TextStyle(color: Colors.white70, fontSize: 12)),
            const SizedBox(height: 6),
            Row(
              children: ItemPriority.values.map((p) {
                final isSelected = _selectedPriority == p;
                return Expanded(
                  child: Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 3),
                    child: ChoiceChip(
                      label: Text(
                        p.toShortString(),
                        style: TextStyle(
                          fontSize: 10,
                          fontWeight: FontWeight.bold,
                          color: isSelected ? Colors.white : Colors.white60,
                        ),
                      ),
                      selected: isSelected,
                      selectedColor: Colors.teal,
                      backgroundColor: const Color(0xFF1E293B),
                      onSelected: (_) => setState(() => _selectedPriority = p),
                    ),
                  ),
                );
              }).toList(),
            ),
            const SizedBox(height: 10),
            TextField(
              controller: _hintController,
              style: const TextStyle(color: Colors.white, fontSize: 13),
              decoration: const InputDecoration(
                labelText: 'Vị trí hồ sơ / cách thức truy cập',
                labelStyle: TextStyle(color: Colors.white70, fontSize: 12),
                hintText: 'VD: Két sắt tầng 2, Google Drive cá nhân...',
                hintStyle: TextStyle(color: Colors.white30, fontSize: 12),
              ),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: _notesController,
              maxLines: 2,
              style: const TextStyle(color: Colors.white, fontSize: 13),
              decoration: const InputDecoration(
                labelText: 'Ghi chú bí mật (Mã hóa AES-256 đầu cuối)',
                labelStyle: TextStyle(color: Colors.tealAccent, fontSize: 12),
                hintText: 'Nội dung giải mã chỉ hiển thị khi có Master Key...',
                hintStyle: TextStyle(color: Colors.white30, fontSize: 12),
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.teal,
                padding: const EdgeInsets.symmetric(vertical: 12),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              ),
              onPressed: _handleSubmit,
              child: Text(
                isEditing ? 'Lưu Thay Đổi' : 'Tạo Hạng Mục',
                style: const TextStyle(fontWeight: FontWeight.bold, color: Colors.white),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
