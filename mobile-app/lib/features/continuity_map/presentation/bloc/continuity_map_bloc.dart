import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../domain/entities/asset_entity.dart';
import '../../domain/repositories/continuity_map_repository.dart';

// Events
abstract class ContinuityMapEvent extends Equatable {
  const ContinuityMapEvent();
  @override
  List<Object?> get props => [];
}

class LoadContinuityMapEvent extends ContinuityMapEvent {}

// States
abstract class ContinuityMapState extends Equatable {
  const ContinuityMapState();
  @override
  List<Object?> get props => [];
}

class ContinuityMapInitial extends ContinuityMapState {}
class ContinuityMapLoading extends ContinuityMapState {}
class ContinuityMapLoaded extends ContinuityMapState {
  final List<AssetEntity> assets;
  const ContinuityMapLoaded(this.assets);
  @override
  List<Object?> get props => [assets];
}
class ContinuityMapError extends ContinuityMapState {
  final String message;
  const ContinuityMapError(this.message);
  @override
  List<Object?> get props => [message];
}

// BLoC
class ContinuityMapBloc extends Bloc<ContinuityMapEvent, ContinuityMapState> {
  final ContinuityMapRepository repository;

  ContinuityMapBloc({required this.repository}) : super(ContinuityMapInitial()) {
    on<LoadContinuityMapEvent>((event, emit) async {
      emit(ContinuityMapLoading());
      final result = await repository.getAssets();
      result.fold(
        (failure) => emit(ContinuityMapError(failure.message)),
        (assets) => emit(ContinuityMapLoaded(assets)),
      );
    });
  }
}
