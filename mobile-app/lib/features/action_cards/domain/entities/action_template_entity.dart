import 'package:equatable/equatable.dart';
import 'action_card_enums.dart';

class TemplateStepItem extends Equatable {
  final int stepOrder;
  final String instruction;
  final String? estimatedDuration;

  const TemplateStepItem({
    required this.stepOrder,
    required this.instruction,
    this.estimatedDuration,
  });

  factory TemplateStepItem.fromJson(Map<String, dynamic> json) {
    return TemplateStepItem(
      stepOrder: json['stepOrder'] as int,
      instruction: json['instruction'] as String,
      estimatedDuration: json['estimatedDuration'] as String?,
    );
  }

  @override
  List<Object?> get props => [stepOrder, instruction, estimatedDuration];
}

class ActionTemplateEntity extends Equatable {
  final String id;
  final String templateCode;
  final String categoryCode;
  final String titleVi;
  final String titleEn;
  final UrgencyStage defaultUrgency;
  final String defaultPriority;
  final List<TemplateStepItem> suggestedSteps;
  final List<String> suggestedRoles;

  const ActionTemplateEntity({
    required this.id,
    required this.templateCode,
    required this.categoryCode,
    required this.titleVi,
    required this.titleEn,
    required this.defaultUrgency,
    required this.defaultPriority,
    required this.suggestedSteps,
    required this.suggestedRoles,
  });

  factory ActionTemplateEntity.fromJson(Map<String, dynamic> json) {
    final stepsRaw = json['suggestedSteps'] as List<dynamic>? ?? [];
    final rolesRaw = json['suggestedRoles'] as List<dynamic>? ?? [];

    return ActionTemplateEntity(
      id: json['id'] as String,
      templateCode: json['templateCode'] as String,
      categoryCode: json['categoryCode'] as String,
      titleVi: json['titleVi'] as String,
      titleEn: json['titleEn'] as String,
      defaultUrgency: UrgencyStageX.fromString(json['defaultUrgency'] as String? ?? 'FIRST_72_HOURS'),
      defaultPriority: json['defaultPriority'] as String? ?? 'CRITICAL',
      suggestedSteps: stepsRaw.map((s) => TemplateStepItem.fromJson(s as Map<String, dynamic>)).toList(),
      suggestedRoles: rolesRaw.map((r) => r.toString()).toList(),
    );
  }

  @override
  List<Object?> get props => [
        id,
        templateCode,
        categoryCode,
        titleVi,
        titleEn,
        defaultUrgency,
        defaultPriority,
        suggestedSteps,
        suggestedRoles,
      ];
}
