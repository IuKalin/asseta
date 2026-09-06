import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/entities/continuity_item_entity.dart';
import '../../domain/entities/continuity_map_entity.dart';
import '../../domain/repositories/continuity_repository.dart';
import '../datasources/continuity_local_datasource.dart';
import '../datasources/continuity_remote_datasource.dart';

class ContinuityRepositoryImpl implements ContinuityRepository {
  final ContinuityRemoteDataSource remoteDataSource;
  final ContinuityLocalDataSource localDataSource;

  ContinuityRepositoryImpl({
    required this.remoteDataSource,
    required this.localDataSource,
  });

  @override
  Future<Either<Failure, ContinuityMapEntity>> getContinuityMap([String? ownerId]) async {
    try {
      final remoteMap = await remoteDataSource.getContinuityMap(ownerId);
      await localDataSource.cacheMap(remoteMap);
      return Right(remoteMap);
    } catch (e) {
      final cached = await localDataSource.getCachedMap();
      if (cached != null) {
        return Right(cached);
      }
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ContinuityItemEntity>> createContinuityItem({
    required String categoryId,
    required String name,
    required ItemPriority priority,
    String? documentLocationHint,
    String? assignedTrustedPersonId,
    String? cipherNotesBlob,
    String? cipherNonce,
    String? cipherAuthTag,
  }) async {
    try {
      final item = await remoteDataSource.createContinuityItem(
        categoryId: categoryId,
        name: name,
        priority: priority,
        documentLocationHint: documentLocationHint,
        assignedTrustedPersonId: assignedTrustedPersonId,
        cipherNotesBlob: cipherNotesBlob,
        cipherNonce: cipherNonce,
        cipherAuthTag: cipherAuthTag,
      );
      return Right(item);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, ContinuityItemEntity>> updateContinuityItem({
    required String id,
    required String name,
    required ItemPriority priority,
    String? documentLocationHint,
    String? assignedTrustedPersonId,
    String? cipherNotesBlob,
    String? cipherNonce,
    String? cipherAuthTag,
    required int rowVersion,
  }) async {
    try {
      final item = await remoteDataSource.updateContinuityItem(
        id: id,
        name: name,
        priority: priority,
        documentLocationHint: documentLocationHint,
        assignedTrustedPersonId: assignedTrustedPersonId,
        cipherNotesBlob: cipherNotesBlob,
        cipherNonce: cipherNonce,
        cipherAuthTag: cipherAuthTag,
        rowVersion: rowVersion,
      );
      return Right(item);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> deleteContinuityItem(String id) async {
    try {
      await remoteDataSource.deleteContinuityItem(id);
      return const Right(null);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
