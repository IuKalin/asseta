import 'package:flutter/material.dart';
import '../../domain/entities/action_step_entity.dart';

class ReorderableStepList extends StatelessWidget {
  final List<ActionStepEntity> steps;
  final Function(String stepId, bool isCompleted)? onToggleStep;
  final Function(List<String> reorderedIds)? onReorder;
  final Function(String stepId)? onDeleteStep;
  final bool readOnly;

  const ReorderableStepList({
    Key? key,
    required this.steps,
    this.onToggleStep,
    this.onReorder,
    this.onDeleteStep,
    this.readOnly = false,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    if (steps.isEmpty) {
      return Container(
        padding: const EdgeInsets.all(20),
        decoration: BoxDecoration(
          color: const Color(0xFF0F172A),
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: const Color(0xFF1E293B)),
        ),
        child: const Center(
          child: Text(
            'Chưa có bước hành động nào.',
            style: TextStyle(color: Color(0xFF64748B), fontSize: 13),
          ),
        ),
      );
    }

    final sortedSteps = List<ActionStepEntity>.from(steps)
      ..sort((a, b) => a.stepOrder.compareTo(b.stepOrder));

    return ReorderableListView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      itemCount: sortedSteps.length,
      buildDefaultDragHandles: !readOnly,
      onReorder: (oldIndex, newIndex) {
        if (readOnly || onReorder == null) return;
        if (newIndex > oldIndex) {
          newIndex -= 1;
        }
        final item = sortedSteps.removeAt(oldIndex);
        sortedSteps.insert(newIndex, item);
        onReorder!(sortedSteps.map((s) => s.id).toList());
      },
      itemBuilder: (context, index) {
        final step = sortedSteps[index];

        return Container(
          key: ValueKey(step.id),
          margin: const EdgeInsets.only(bottom: 8),
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          decoration: BoxDecoration(
            color: step.isCompleted ? const Color(0x2210B981) : const Color(0xFF0F172A),
            borderRadius: BorderRadius.circular(10),
            border: Border.all(
              color: step.isCompleted ? const Color(0x4410B981) : const Color(0xFF1E293B),
            ),
          ),
          child: Row(
            children: [
              Checkbox(
                value: step.isCompleted,
                activeColor: const Color(0xFF10B981),
                checkColor: Colors.white,
                onChanged: readOnly || onToggleStep == null
                    ? null
                    : (val) {
                        if (val != null) {
                          onToggleStep!(step.id, val);
                        }
                      },
              ),
              Container(
                width: 24,
                height: 24,
                alignment: Alignment.center,
                decoration: BoxDecoration(
                  color: const Color(0xFF1E293B),
                  shape: BoxShape.circle,
                ),
                child: Text(
                  '${step.stepOrder}',
                  style: const TextStyle(
                    color: Colors.white70,
                    fontSize: 11,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      step.instruction,
                      style: TextStyle(
                        color: step.isCompleted ? const Color(0xFF94A3B8) : Colors.white,
                        fontSize: 13,
                        decoration: step.isCompleted ? TextDecoration.lineThrough : null,
                      ),
                    ),
                    if (step.estimatedDuration != null && step.estimatedDuration!.isNotEmpty) ...[
                      const SizedBox(height: 2),
                      Text(
                        '⏱️ ${step.estimatedDuration}',
                        style: const TextStyle(
                          color: Color(0xFF64748B),
                          fontSize: 11,
                        ),
                      ),
                    ],
                  ],
                ),
              ),
              if (!readOnly && onDeleteStep != null)
                IconButton(
                  icon: const Icon(Icons.close, size: 16, color: Color(0xFF64748B)),
                  onPressed: () => onDeleteStep!(step.id),
                  tooltip: 'Xóa bước này',
                ),
              if (!readOnly)
                const Icon(Icons.drag_handle, color: Color(0xFF475569), size: 20),
            ],
          ),
        );
      },
    );
  }
}
