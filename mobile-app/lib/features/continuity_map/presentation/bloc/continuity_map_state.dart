import 'package:cryptography/cryptography.dart';
import 'package:equatable/equatable.dart';
import '../../domain/entities/continuity_map_entity.dart';

abstract class ContinuityMapState extends Equatable {
  const ContinuityMapState();

  @override
  List<Object?> get props => [];
}

class ContinuityMapInitial extends ContinuityMapState {
  const ContinuityMapInitial();
}

class ContinuityMapLoading extends ContinuityMapState {
  const ContinuityMapLoading();
}

class ContinuityMapLoaded extends ContinuityMapState {
  final ContinuityMapEntity map;
  final bool isKeyUnlocked;
  final SecretKey? masterKey;
  final String? notificationMessage;

  const ContinuityMapLoaded({
    required this.map,
    this.isKeyUnlocked = false,
    this.masterKey,
    this.notificationMessage,
  });

  ContinuityMapLoaded copyWith({
    ContinuityMapEntity? map,
    bool? isKeyUnlocked,
    SecretKey? masterKey,
    String? notificationMessage,
  }) {
    return ContinuityMapLoaded(
      map: map ?? this.map,
      isKeyUnlocked: isKeyUnlocked ?? this.isKeyUnlocked,
      masterKey: masterKey ?? this.masterKey,
      notificationMessage: notificationMessage,
    );
  }

  @override
  List<Object?> get props => [map, isKeyUnlocked, masterKey, notificationMessage];
}

class ContinuityMapError extends ContinuityMapState {
  final String message;
  const ContinuityMapError(this.message);

  @override
  List<Object?> get props => [message];
}
