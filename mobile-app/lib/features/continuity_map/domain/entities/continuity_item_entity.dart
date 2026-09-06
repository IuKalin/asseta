import 'package:equatable/equatable.dart';

enum ItemPriority { critical, important, low }

extension ItemPriorityX on ItemPriority {
  String toShortString() {
    switch (this) {
      case ItemPriority.critical:
        return 'CRITICAL';
      case ItemPriority.important:
        return 'IMPORTANT';
      case ItemPriority.low:
        return 'LOW';
    }
  }

  static ItemPriority fromString(String val) {
    switch (val.toUpperCase()) {
      case 'CRITICAL':
        return ItemPriority.critical;
      case 'LOW':
        return ItemPriority.low;
      case 'IMPORTANT':
      default:
        return ItemPriority.important;
    }
  }
}

class ContinuityItemEntity extends Equatable {
  final String id;
  final String ownerId;
  final String categoryId;
  final String categoryCode;
  final String name;
  final ItemPriority priority;
  final String? documentLocationHint;
  final String? assignedTrustedPersonId;
  final String? actionCardId;
  final bool hasConfidentialNotes;
  final String? cipherNotesBlob;
  final String? cipherNonce;
  final String? cipherAuthTag;
  final bool isCompleted;
  final int rowVersion;
  final int sortOrder;
  final DateTime createdAtUtc;

  const ContinuityItemEntity({
    required this.id,
    required this.ownerId,
    required this.categoryId,
    required this.categoryCode,
    required this.name,
    required this.priority,
    this.documentLocationHint,
    this.assignedTrustedPersonId,
    this.actionCardId,
    required this.hasConfidentialNotes,
    this.cipherNotesBlob,
    this.cipherNonce,
    this.cipherAuthTag,
    required this.isCompleted,
    required this.rowVersion,
    required this.sortOrder,
    required this.createdAtUtc,
  });

  bool get hasContinuityGap {
    if (priority == ItemPriority.critical || priority == ItemPriority.important) {
      final hasPerson = assignedTrustedPersonId != null && assignedTrustedPersonId!.isNotEmpty;
      final hasDoc = documentLocationHint != null && documentLocationHint!.trim().isNotEmpty;
      return !hasPerson || !hasDoc;
    }
    return false;
  }

  @override
  List<Object?> get props => [
        id,
        ownerId,
        categoryId,
        categoryCode,
        name,
        priority,
        documentLocationHint,
        assignedTrustedPersonId,
        actionCardId,
        hasConfidentialNotes,
        cipherNotesBlob,
        cipherNonce,
        cipherAuthTag,
        isCompleted,
        rowVersion,
        sortOrder,
        createdAtUtc,
      ];
}
