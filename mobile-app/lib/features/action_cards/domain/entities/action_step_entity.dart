import 'package:equatable/equatable.dart';

class ActionStepEntity extends Equatable {
  final String id;
  final String actionCardId;
  final int stepOrder;
  final String instruction;
  final String? estimatedDuration;
  final bool isCompleted;

  const ActionStepEntity({
    required this.id,
    required this.actionCardId,
    required this.stepOrder,
    required this.instruction,
    this.estimatedDuration,
    required this.isCompleted,
  });

  ActionStepEntity copyWith({
    String? id,
    String? actionCardId,
    int? stepOrder,
    String? instruction,
    String? estimatedDuration,
    bool? isCompleted,
  }) {
    return ActionStepEntity(
      id: id ?? this.id,
      actionCardId: actionCardId ?? this.actionCardId,
      stepOrder: stepOrder ?? this.stepOrder,
      instruction: instruction ?? this.instruction,
      estimatedDuration: estimatedDuration ?? this.estimatedDuration,
      isCompleted: isCompleted ?? this.isCompleted,
    );
  }

  factory ActionStepEntity.fromJson(Map<String, dynamic> json) {
    return ActionStepEntity(
      id: json['id'] as String,
      actionCardId: json['actionCardId'] as String,
      stepOrder: json['stepOrder'] as int,
      instruction: json['instruction'] as String,
      estimatedDuration: json['estimatedDuration'] as String?,
      isCompleted: json['isCompleted'] as bool? ?? false,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'actionCardId': actionCardId,
      'stepOrder': stepOrder,
      'instruction': instruction,
      'estimatedDuration': estimatedDuration,
      'isCompleted': isCompleted,
    };
  }

  @override
  List<Object?> get props => [
        id,
        actionCardId,
        stepOrder,
        instruction,
        estimatedDuration,
        isCompleted,
      ];
}
