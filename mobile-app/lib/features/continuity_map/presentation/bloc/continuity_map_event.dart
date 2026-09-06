import 'package:equatable/equatable.dart';
import '../../domain/entities/continuity_item_entity.dart';

abstract class ContinuityMapEvent extends Equatable {
  const ContinuityMapEvent();

  @override
  List<Object?> get props => [];
}

class LoadContinuityMapEvent extends ContinuityMapEvent {
  final String? ownerId;
  const LoadContinuityMapEvent({this.ownerId});

  @override
  List<Object?> get props => [ownerId];
}

class RefreshContinuityMapEvent extends ContinuityMapEvent {
  const RefreshContinuityMapEvent();
}

class CreateContinuityItemEvent extends ContinuityMapEvent {
  final String categoryId;
  final String name;
  final ItemPriority priority;
  final String? documentLocationHint;
  final String? assignedTrustedPersonId;
  final String? plainNotes;

  const CreateContinuityItemEvent({
    required this.categoryId,
    required this.name,
    required this.priority,
    this.documentLocationHint,
    this.assignedTrustedPersonId,
    this.plainNotes,
  });

  @override
  List<Object?> get props => [
        categoryId,
        name,
        priority,
        documentLocationHint,
        assignedTrustedPersonId,
        plainNotes,
      ];
}

class UpdateContinuityItemEvent extends ContinuityMapEvent {
  final String id;
  final String name;
  final ItemPriority priority;
  final String? documentLocationHint;
  final String? assignedTrustedPersonId;
  final String? plainNotes;
  final int rowVersion;

  const UpdateContinuityItemEvent({
    required this.id,
    required this.name,
    required this.priority,
    this.documentLocationHint,
    this.assignedTrustedPersonId,
    this.plainNotes,
    required this.rowVersion,
  });

  @override
  List<Object?> get props => [
        id,
        name,
        priority,
        documentLocationHint,
        assignedTrustedPersonId,
        plainNotes,
        rowVersion,
      ];
}

class DeleteContinuityItemEvent extends ContinuityMapEvent {
  final String id;
  const DeleteContinuityItemEvent(this.id);

  @override
  List<Object?> get props => [id];
}

class UnlockMasterKeyEvent extends ContinuityMapEvent {
  final String passphrase;
  const UnlockMasterKeyEvent(this.passphrase);

  @override
  List<Object?> get props => [passphrase];
}

class LockMasterKeyEvent extends ContinuityMapEvent {
  const LockMasterKeyEvent();
}
