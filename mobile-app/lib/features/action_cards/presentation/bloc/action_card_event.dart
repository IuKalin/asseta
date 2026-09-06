import 'package:equatable/equatable.dart';
import '../../domain/entities/action_card_enums.dart';

abstract class ActionCardEvent extends Equatable {
  const ActionCardEvent();

  @override
  List<Object?> get props => [];
}

class LoadActionCardsEvent extends ActionCardEvent {
  final UrgencyStage? urgency;
  final String? search;

  const LoadActionCardsEvent({this.urgency, this.search});

  @override
  List<Object?> get props => [urgency, search];
}

class FilterUrgencyStageEvent extends ActionCardEvent {
  final UrgencyStage? urgency;

  const FilterUrgencyStageEvent(this.urgency);

  @override
  List<Object?> get props => [urgency];
}

class SearchActionCardsEvent extends ActionCardEvent {
  final String query;

  const SearchActionCardsEvent(this.query);

  @override
  List<Object?> get props => [query];
}

class CreateActionCardEvent extends ActionCardEvent {
  final String categoryId;
  final String title;
  final UrgencyStage urgency;
  final String priority;
  final String? summary;
  final String? assignedTrustedPersonId;
  final String? documentLocationHint;
  final String? digitalStorageLink;
  final String? cipherInstructionsBlob;
  final String? cipherNonce;
  final String? cipherAuthTag;

  const CreateActionCardEvent({
    required this.categoryId,
    required this.title,
    required this.urgency,
    required this.priority,
    this.summary,
    this.assignedTrustedPersonId,
    this.documentLocationHint,
    this.digitalStorageLink,
    this.cipherInstructionsBlob,
    this.cipherNonce,
    this.cipherAuthTag,
  });

  @override
  List<Object?> get props => [
        categoryId,
        title,
        urgency,
        priority,
        summary,
        assignedTrustedPersonId,
        documentLocationHint,
        digitalStorageLink,
        cipherInstructionsBlob,
        cipherNonce,
        cipherAuthTag,
      ];
}

class CreateActionCardFromItemEvent extends ActionCardEvent {
  final String continuityItemId;
  final String? templateCode;
  final UrgencyStage? urgency;
  final String? priority;
  final String? summary;

  const CreateActionCardFromItemEvent({
    required this.continuityItemId,
    this.templateCode,
    this.urgency,
    this.priority,
    this.summary,
  });

  @override
  List<Object?> get props => [continuityItemId, templateCode, urgency, priority, summary];
}

class UpdateActionCardEvent extends ActionCardEvent {
  final String id;
  final String title;
  final UrgencyStage urgency;
  final String priority;
  final int rowVersion;
  final String? summary;
  final String? assignedTrustedPersonId;
  final String? documentLocationHint;
  final String? digitalStorageLink;
  final String? cipherInstructionsBlob;
  final String? cipherNonce;
  final String? cipherAuthTag;

  const UpdateActionCardEvent({
    required this.id,
    required this.title,
    required this.urgency,
    required this.priority,
    required this.rowVersion,
    this.summary,
    this.assignedTrustedPersonId,
    this.documentLocationHint,
    this.digitalStorageLink,
    this.cipherInstructionsBlob,
    this.cipherNonce,
    this.cipherAuthTag,
  });

  @override
  List<Object?> get props => [
        id,
        title,
        urgency,
        priority,
        rowVersion,
        summary,
        assignedTrustedPersonId,
        documentLocationHint,
        digitalStorageLink,
        cipherInstructionsBlob,
        cipherNonce,
        cipherAuthTag,
      ];
}

class DeleteActionCardEvent extends ActionCardEvent {
  final String cardId;

  const DeleteActionCardEvent(this.cardId);

  @override
  List<Object?> get props => [cardId];
}

class ToggleStepCompletionEvent extends ActionCardEvent {
  final String cardId;
  final String stepId;
  final String instruction;
  final String? estimatedDuration;
  final bool isCompleted;

  const ToggleStepCompletionEvent({
    required this.cardId,
    required this.stepId,
    required this.instruction,
    this.estimatedDuration,
    required this.isCompleted,
  });

  @override
  List<Object?> get props => [cardId, stepId, instruction, estimatedDuration, isCompleted];
}

class AddStepEvent extends ActionCardEvent {
  final String cardId;
  final String instruction;
  final String? estimatedDuration;

  const AddStepEvent({
    required this.cardId,
    required this.instruction,
    this.estimatedDuration,
  });

  @override
  List<Object?> get props => [cardId, instruction, estimatedDuration];
}

class DeleteStepEvent extends ActionCardEvent {
  final String cardId;
  final String stepId;

  const DeleteStepEvent({
    required this.cardId,
    required this.stepId,
  });

  @override
  List<Object?> get props => [cardId, stepId];
}

class ReorderStepsEvent extends ActionCardEvent {
  final String cardId;
  final List<String> orderedStepIds;

  const ReorderStepsEvent({
    required this.cardId,
    required this.orderedStepIds,
  });

  @override
  List<Object?> get props => [cardId, orderedStepIds];
}

class AddContactEvent extends ActionCardEvent {
  final String cardId;
  final String contactName;
  final String relationshipOrRole;
  final String? phoneNumber;
  final String? email;
  final String? contactNotes;

  const AddContactEvent({
    required this.cardId,
    required this.contactName,
    required this.relationshipOrRole,
    this.phoneNumber,
    this.email,
    this.contactNotes,
  });

  @override
  List<Object?> get props => [
        cardId,
        contactName,
        relationshipOrRole,
        phoneNumber,
        email,
        contactNotes,
      ];
}

class DeleteContactEvent extends ActionCardEvent {
  final String cardId;
  final String contactId;

  const DeleteContactEvent({
    required this.cardId,
    required this.contactId,
  });

  @override
  List<Object?> get props => [cardId, contactId];
}
