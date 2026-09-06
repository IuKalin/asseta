import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../domain/entities/trusted_person_entity.dart';
import '../../domain/repositories/trusted_people_repository.dart';

// Events
abstract class TrustedPeopleEvent extends Equatable {
  const TrustedPeopleEvent();
  @override
  List<Object?> get props => [];
}

class LoadTrustedPeopleEvent extends TrustedPeopleEvent {
  const LoadTrustedPeopleEvent();
}

class CreateTrustedPersonEvent extends TrustedPeopleEvent {
  final String fullName;
  final String email;
  final String phoneNumber;
  final String relationship;
  final int trustLevel;
  final String? roleDescription;

  const CreateTrustedPersonEvent({
    required this.fullName,
    required this.email,
    required this.phoneNumber,
    required this.relationship,
    required this.trustLevel,
    this.roleDescription,
  });

  @override
  List<Object?> get props => [fullName, email, phoneNumber, relationship, trustLevel, roleDescription];
}

class ClaimPairingCodeEvent extends TrustedPeopleEvent {
  final String pairingCode;
  const ClaimPairingCodeEvent(this.pairingCode);

  @override
  List<Object?> get props => [pairingCode];
}

class RevokeTrustedPersonEvent extends TrustedPeopleEvent {
  final String id;
  const RevokeTrustedPersonEvent(this.id);

  @override
  List<Object?> get props => [id];
}

class RegeneratePairingCodeEvent extends TrustedPeopleEvent {
  final String id;
  const RegeneratePairingCodeEvent(this.id);

  @override
  List<Object?> get props => [id];
}

// States
abstract class TrustedPeopleState extends Equatable {
  const TrustedPeopleState();
  @override
  List<Object?> get props => [];
}

class TrustedPeopleInitialState extends TrustedPeopleState {}

class TrustedPeopleLoadingState extends TrustedPeopleState {}

class TrustedPeopleLoadedState extends TrustedPeopleState {
  final List<TrustedPersonEntity> people;
  final String? newlyCreatedPairingCode;

  const TrustedPeopleLoadedState({
    required this.people,
    this.newlyCreatedPairingCode,
  });

  @override
  List<Object?> get props => [people, newlyCreatedPairingCode];
}

class PairingClaimSuccessState extends TrustedPeopleState {
  final ClaimPairingResultEntity result;
  const PairingClaimSuccessState(this.result);

  @override
  List<Object?> get props => [result];
}

class TrustedPeopleErrorState extends TrustedPeopleState {
  final String message;
  const TrustedPeopleErrorState(this.message);

  @override
  List<Object?> get props => [message];
}

// BLoC
class TrustedPeopleBloc extends Bloc<TrustedPeopleEvent, TrustedPeopleState> {
  final TrustedPeopleRepository repository;

  TrustedPeopleBloc({required this.repository}) : super(TrustedPeopleInitialState()) {
    on<LoadTrustedPeopleEvent>(_onLoadTrustedPeople);
    on<CreateTrustedPersonEvent>(_onCreateTrustedPerson);
    on<ClaimPairingCodeEvent>(_onClaimPairingCode);
    on<RevokeTrustedPersonEvent>(_onRevokeTrustedPerson);
    on<RegeneratePairingCodeEvent>(_onRegeneratePairingCode);
  }

  Future<void> _onLoadTrustedPeople(
    LoadTrustedPeopleEvent event,
    Emitter<TrustedPeopleState> emit,
  ) async {
    emit(TrustedPeopleLoadingState());
    final result = await repository.getTrustedPeople();
    result.fold(
      (failure) => emit(TrustedPeopleErrorState(failure.message)),
      (people) => emit(TrustedPeopleLoadedState(people: people)),
    );
  }

  Future<void> _onCreateTrustedPerson(
    CreateTrustedPersonEvent event,
    Emitter<TrustedPeopleState> emit,
  ) async {
    emit(TrustedPeopleLoadingState());
    final result = await repository.createTrustedPerson(
      fullName: event.fullName,
      email: event.email,
      phoneNumber: event.phoneNumber,
      relationship: event.relationship,
      trustLevel: event.trustLevel,
      roleDescription: event.roleDescription,
    );
    result.fold(
      (failure) => emit(TrustedPeopleErrorState(failure.message)),
      (created) async {
        final currentList = await repository.getTrustedPeople();
        currentList.fold(
          (_) => emit(TrustedPeopleLoadedState(people: [created], newlyCreatedPairingCode: created.activePairingCode)),
          (all) => emit(TrustedPeopleLoadedState(people: all, newlyCreatedPairingCode: created.activePairingCode)),
        );
      },
    );
  }

  Future<void> _onClaimPairingCode(
    ClaimPairingCodeEvent event,
    Emitter<TrustedPeopleState> emit,
  ) async {
    emit(TrustedPeopleLoadingState());
    final result = await repository.claimPairingCode(event.pairingCode);
    result.fold(
      (failure) => emit(TrustedPeopleErrorState(failure.message)),
      (claimResult) => emit(PairingClaimSuccessState(claimResult)),
    );
  }

  Future<void> _onRevokeTrustedPerson(
    RevokeTrustedPersonEvent event,
    Emitter<TrustedPeopleState> emit,
  ) async {
    final result = await repository.revokeTrustedPerson(event.id);
    result.fold(
      (failure) => emit(TrustedPeopleErrorState(failure.message)),
      (_) => add(const LoadTrustedPeopleEvent()),
    );
  }

  Future<void> _onRegeneratePairingCode(
    RegeneratePairingCodeEvent event,
    Emitter<TrustedPeopleState> emit,
  ) async {
    final result = await repository.regeneratePairingCode(event.id);
    result.fold(
      (failure) => emit(TrustedPeopleErrorState(failure.message)),
      (updated) => add(const LoadTrustedPeopleEvent()),
    );
  }
}
