import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/entities/continuity_item_entity.dart';
import '../bloc/continuity_map_bloc.dart';
import '../bloc/continuity_map_event.dart';

class AssessmentPage extends StatefulWidget {
  const AssessmentPage({super.key});

  @override
  State<AssessmentPage> createState() => _AssessmentPageState();
}

class _AssessmentPageState extends State<AssessmentPage> {
  final List<Map<String, dynamic>> _questions = [
    {
      'id': 'Q1',
      'category': 'FINANCIAL',
      'name': 'Tài khoản ngân hàng & Tiết kiệm',
      'prompt': 'Bạn có tài khoản ngân hàng chính hoặc sổ tiết kiệm cần người thân nắm bắt?',
      'defaultPriority': ItemPriority.critical,
    },
    {
      'id': 'Q2',
      'category': 'FINANCIAL',
      'name': 'Khoản vay & Nghĩa vụ trả nợ',
      'prompt': 'Bạn có khoản vay thế chấp hoặc trả góp cần thanh toán định kỳ?',
      'defaultPriority': ItemPriority.critical,
    },
    {
      'id': 'Q3',
      'category': 'PROPERTY',
      'name': 'Nhà ở & Bất động sản sở hữu',
      'prompt': 'Bạn có sở hữu nhà đất, chung cư hoặc hợp đồng thuê dài hạn?',
      'defaultPriority': ItemPriority.critical,
    },
    {
      'id': 'Q4',
      'category': 'PROPERTY',
      'name': 'Phương tiện đi lại (Ô tô / Xe máy)',
      'prompt': 'Bạn có xe ô tô hoặc phương tiện chính đứng tên cá nhân?',
      'defaultPriority': ItemPriority.important,
    },
    {
      'id': 'Q5',
      'category': 'INSURANCE',
      'name': 'Hợp đồng bảo hiểm nhân thọ',
      'prompt': 'Bạn có tham gia hợp đồng bảo hiểm nhân thọ bảo vệ gia đình?',
      'defaultPriority': ItemPriority.critical,
    },
    {
      'id': 'Q6',
      'category': 'INSURANCE',
      'name': 'Thẻ bảo hiểm sức khỏe cao cấp',
      'prompt': 'Bạn có thẻ bảo hiểm sức khỏe tư nhân bảo lãnh viện phí?',
      'defaultPriority': ItemPriority.important,
    },
    {
      'id': 'Q7',
      'category': 'BUSINESS',
      'name': 'Cổ phần & Vốn góp công ty',
      'prompt': 'Bạn có cổ phần doanh nghiệp hoặc tư cách đại diện pháp luật?',
      'defaultPriority': ItemPriority.critical,
    },
    {
      'id': 'Q8',
      'category': 'BUSINESS',
      'name': 'Thỏa thuận hợp tác then chốt',
      'prompt': 'Có hợp đồng đối tác quan trọng nào cần bàn giao người kế nhiệm?',
      'defaultPriority': ItemPriority.important,
    },
    {
      'id': 'Q9',
      'category': 'DOCUMENTS',
      'name': 'Giấy tờ tùy thân & Hộ chiếu gốc',
      'prompt': 'Vị trí cất giữ hộ chiếu, CCCD và sổ hộ khẩu đã được sắp xếp rõ ràng?',
      'defaultPriority': ItemPriority.critical,
    },
    {
      'id': 'Q10',
      'category': 'DOCUMENTS',
      'name': 'Di chúc / Thỏa thuận phân chia',
      'prompt': 'Bạn đã từng lập văn bản di chúc hoặc nguyện vọng tiếp quản tài sản?',
      'defaultPriority': ItemPriority.important,
    },
    {
      'id': 'Q11',
      'category': 'FAMILY',
      'name': 'Nghĩa vụ chu cấp & Phụng dưỡng',
      'prompt': 'Bạn có nghĩa vụ chu cấp học phí con cái hoặc phụng dưỡng cha mẹ hàng tháng?',
      'defaultPriority': ItemPriority.critical,
    },
    {
      'id': 'Q12',
      'category': 'FAMILY',
      'name': 'Liên lạc khẩn cấp gia đình',
      'prompt': 'Đã có danh sách số điện thoại người thân và luật sư đáng tin cậy?',
      'defaultPriority': ItemPriority.important,
    },
  ];

  int _currentIndex = 0;
  final Map<String, bool> _answers = {};
  final Map<String, String> _hints = {};

  @override
  void initState() {
    super.initState();
    for (var q in _questions) {
      _answers[q['id']] = true;
      _hints[q['id']] = '';
    }
  }

  void _handleComplete() {
    final bloc = context.read<ContinuityMapBloc>();

    // Dispatches created items based on answered questions
    for (var q in _questions) {
      final qId = q['id'] as String;
      if (_answers[qId] == true) {
        bloc.add(CreateContinuityItemEvent(
          categoryId: q['category'] as String,
          name: q['name'] as String,
          priority: q['defaultPriority'] as ItemPriority,
          documentLocationHint: _hints[qId]?.trim().isNotEmpty == true ? _hints[qId] : 'Đang cập nhật vị trí hồ sơ',
        ));
      }
    }

    Navigator.of(context).pop();
  }

  @override
  Widget build(BuildContext context) {
    final q = _questions[_currentIndex];
    final qId = q['id'] as String;
    final hasItem = _answers[qId] ?? true;
    final progress = ((_currentIndex + 1) / _questions.length);

    return Scaffold(
      backgroundColor: const Color(0xFF090D16),
      appBar: AppBar(
        backgroundColor: const Color(0xFF0F172A),
        title: const Text('Khảo Sát Tiếp Quản Nhanh', style: TextStyle(fontSize: 15, color: Colors.white)),
      ),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Progress Bar
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Câu ${_currentIndex + 1}/${_questions.length}',
                    style: const TextStyle(fontSize: 12, color: Colors.white70)),
                Text('${(progress * 100).toInt()}%',
                    style: const TextStyle(fontSize: 12, color: Colors.tealAccent)),
              ],
            ),
            const SizedBox(height: 8),
            LinearProgressIndicator(
              value: progress,
              color: Colors.tealAccent,
              backgroundColor: const Color(0xFF1E293B),
            ),
            const SizedBox(height: 24),

            // Question Card
            Expanded(
              child: Container(
                padding: const EdgeInsets.all(20),
                decoration: BoxDecoration(
                  color: const Color(0xFF0F172A),
                  borderRadius: BorderRadius.circular(20),
                  border: Border.all(color: const Color(0xFF1E293B)),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: Colors.teal.withOpacity(0.2),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        q['category'] as String,
                        style: const TextStyle(fontSize: 10, fontWeight: FontWeight.bold, color: Colors.tealAccent),
                      ),
                    ),
                    const SizedBox(height: 12),
                    Text(
                      q['prompt'] as String,
                      style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.white, height: 1.4),
                    ),
                    const SizedBox(height: 20),

                    // Yes / No options
                    Row(
                      children: [
                        Expanded(
                          child: ElevatedButton(
                            style: ElevatedButton.styleFrom(
                              backgroundColor: hasItem ? Colors.teal : const Color(0xFF1E293B),
                              padding: const EdgeInsets.symmetric(vertical: 12),
                              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                            ),
                            onPressed: () => setState(() => _answers[qId] = true),
                            child: Text('Có', style: TextStyle(color: hasItem ? Colors.white : Colors.white60)),
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: ElevatedButton(
                            style: ElevatedButton.styleFrom(
                              backgroundColor: !hasItem ? const Color(0xFF475569) : const Color(0xFF1E293B),
                              padding: const EdgeInsets.symmetric(vertical: 12),
                              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                            ),
                            onPressed: () => setState(() => _answers[qId] = false),
                            child: Text('Không có', style: TextStyle(color: !hasItem ? Colors.white : Colors.white60)),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),

                    if (hasItem)
                      TextField(
                        key: ValueKey(qId),
                        style: const TextStyle(color: Colors.white, fontSize: 13),
                        decoration: const InputDecoration(
                          labelText: 'Gợi ý vị trí hồ sơ / cách thức truy cập',
                          labelStyle: TextStyle(color: Colors.white70, fontSize: 12),
                          hintText: 'VD: Két sắt gia đình, Thư mục trên mây...',
                          hintStyle: TextStyle(color: Colors.white30, fontSize: 12),
                        ),
                        onChanged: (val) => _hints[qId] = val,
                      ),
                  ],
                ),
              ),
            ),

            // Navigation buttons
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                if (_currentIndex > 0)
                  TextButton(
                    onPressed: () => setState(() => _currentIndex--),
                    child: const Text('Quay lại', style: TextStyle(color: Colors.white60)),
                  )
                else
                  const SizedBox.shrink(),
                if (_currentIndex < _questions.length - 1)
                  ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.teal,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                    ),
                    onPressed: () => setState(() => _currentIndex++),
                    child: const Text('Tiếp theo', style: TextStyle(color: Colors.white)),
                  )
                else
                  ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.tealAccent.shade700,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                    ),
                    onPressed: _handleComplete,
                    child: const Text('Hoàn tất khảo sát', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
