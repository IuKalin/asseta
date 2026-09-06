import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/crypto/crypto_service.dart';
import '../../domain/repositories/continuity_repository.dart';
import 'continuity_map_event.dart';
import 'continuity_map_state.dart';

class ContinuityMapBloc extends Bloc<ContinuityMapEvent, ContinuityMapState> {
  final ContinuityRepository repository;
  final MobileCryptoService cryptoService;

  ContinuityMapBloc({
    required this.repository,
    required this.cryptoService,
  }) : super(const ContinuityMapInitial()) {
    on<LoadContinuityMapEvent>(_onLoadContinuityMap);
    on<RefreshContinuityMapEvent>(_onRefreshContinuityMap);
    on<CreateContinuityItemEvent>(_onCreateItem);
    on<UpdateContinuityItemEvent>(_onUpdateItem);
    on<DeleteContinuityItemEvent>(_onDeleteItem);
    on<UnlockMasterKeyEvent>(_onUnlockMasterKey);
    on<LockMasterKeyEvent>(_onLockMasterKey);
  }

  Future<void> _onLoadContinuityMap(
    LoadContinuityMapEvent event,
    Emitter<ContinuityMapState> emit,
  ) async {
    emit(const ContinuityMapLoading());
    final result = await repository.getContinuityMap(event.ownerId);
    result.fold(
      (failure) => emit(ContinuityMapError(failure.message)),
      (map) => emit(ContinuityMapLoaded(map: map)),
    );
  }

  Future<void> _onRefreshContinuityMap(
    RefreshContinuityMapEvent event,
    Emitter<ContinuityMapState> emit,
  ) async {
    final currentState = state;
    final result = await repository.getContinuityMap();
    result.fold(
      (failure) => emit(ContinuityMapError(failure.message)),
      (map) {
        if (currentState is ContinuityMapLoaded) {
          emit(currentState.copyWith(map: map));
        } else {
          emit(ContinuityMapLoaded(map: map));
        }
      },
    );
  }

  Future<void> _onCreateItem(
    CreateContinuityItemEvent event,
    Emitter<ContinuityMapState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ContinuityMapLoaded) return;

    String? cipherBlob;
    String? nonce;
    String? authTag;

    if (event.plainNotes != null && event.plainNotes!.trim().isNotEmpty) {
      if (!currentState.isKeyUnlocked || currentState.masterKey == null) {
        emit(currentState.copyWith(
          notificationMessage: 'Vui lòng mở khóa Master Key để mã hóa ghi chú.',
        ));
        return;
      }

      final encrypted = await cryptoService.encrypt(
        currentState.masterKey!,
        event.plainNotes!,
      );
      cipherBlob = encrypted.cipherNotesBlob;
      nonce = encrypted.cipherNonce;
      authTag = encrypted.cipherAuthTag;
    }

    final result = await repository.createContinuityItem(
      categoryId: event.categoryId,
      name: event.name,
      priority: event.priority,
      documentLocationHint: event.documentLocationHint,
      assignedTrustedPersonId: event.assignedTrustedPersonId,
      cipherNotesBlob: cipherBlob,
      cipherNonce: nonce,
      cipherAuthTag: authTag,
    );

    await result.fold(
      (failure) async => emit(ContinuityMapError(failure.message)),
      (created) async {
        final refresh = await repository.getContinuityMap();
        refresh.fold(
          (failure) => emit(ContinuityMapError(failure.message)),
          (newMap) => emit(currentState.copyWith(
            map: newMap,
            notificationMessage: 'Đã thêm hạng mục thành công.',
          )),
        );
      },
    );
  }

  Future<void> _onUpdateItem(
    UpdateContinuityItemEvent event,
    Emitter<ContinuityMapState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ContinuityMapLoaded) return;

    String? cipherBlob;
    String? nonce;
    String? authTag;

    if (event.plainNotes != null && event.plainNotes!.trim().isNotEmpty) {
      if (!currentState.isKeyUnlocked || currentState.masterKey == null) {
        emit(currentState.copyWith(
          notificationMessage: 'Vui lòng mở khóa Master Key để mã hóa ghi chú.',
        ));
        return;
      }

      final encrypted = await cryptoService.encrypt(
        currentState.masterKey!,
        event.plainNotes!,
      );
      cipherBlob = encrypted.cipherNotesBlob;
      nonce = encrypted.cipherNonce;
      authTag = encrypted.cipherAuthTag;
    }

    final result = await repository.updateContinuityItem(
      id: event.id,
      name: event.name,
      priority: event.priority,
      documentLocationHint: event.documentLocationHint,
      assignedTrustedPersonId: event.assignedTrustedPersonId,
      cipherNotesBlob: cipherBlob,
      cipherNonce: nonce,
      cipherAuthTag: authTag,
      rowVersion: event.rowVersion,
    );

    await result.fold(
      (failure) async => emit(ContinuityMapError(failure.message)),
      (updated) async {
        final refresh = await repository.getContinuityMap();
        refresh.fold(
          (failure) => emit(ContinuityMapError(failure.message)),
          (newMap) => emit(currentState.copyWith(
            map: newMap,
            notificationMessage: 'Đã cập nhật hạng mục thành công.',
          )),
        );
      },
    );
  }

  Future<void> _onDeleteItem(
    DeleteContinuityItemEvent event,
    Emitter<ContinuityMapState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ContinuityMapLoaded) return;

    final result = await repository.deleteContinuityItem(event.id);
    await result.fold(
      (failure) async => emit(ContinuityMapError(failure.message)),
      (_) async {
        final refresh = await repository.getContinuityMap();
        refresh.fold(
          (failure) => emit(ContinuityMapError(failure.message)),
          (newMap) => emit(currentState.copyWith(
            map: newMap,
            notificationMessage: 'Đã xóa hạng mục thành công.',
          )),
        );
      },
    );
  }

  Future<void> _onUnlockMasterKey(
    UnlockMasterKeyEvent event,
    Emitter<ContinuityMapState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ContinuityMapLoaded) return;

    try {
      final salt = cryptoService.generateSalt();
      final key = await cryptoService.deriveMasterKey(event.passphrase, salt);
      emit(currentState.copyWith(
        isKeyUnlocked: true,
        masterKey: key,
        notificationMessage: 'Master Key đã mở khóa thành công.',
      ));
    } catch (e) {
      emit(currentState.copyWith(
        notificationMessage: 'Không thể mở khóa: ${e.toString()}',
      ));
    }
  }

  void _onLockMasterKey(
    LockMasterKeyEvent event,
    Emitter<ContinuityMapState> emit,
  ) {
    final currentState = state;
    if (currentState is! ContinuityMapLoaded) return;

    emit(ContinuityMapLoaded(
      map: currentState.map,
      isKeyUnlocked: false,
      masterKey: null,
      notificationMessage: 'Đã khóa Master Key an toàn.',
    ));
  }
}
