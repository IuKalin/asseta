import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../entities/continuity_item_entity.dart';
import '../entities/continuity_map_entity.dart';

abstract class ContinuityRepository {
  Future<Either<Failure, ContinuityMapEntity>> getContinuityMap([String? ownerId]);
  Future<Either<Failure, ContinuityItemEntity>> createContinuityItem({
    required String categoryId,
    required String name,
    required ItemPriority priority,
    String? documentLocationHint,
    String? assignedTrustedPersonId,
    String? cipherNotesBlob,
    String? cipherNonce,
    String? cipherAuthTag,
  });
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
  });
  Future<Either<Failure, void>> deleteContinuityItem(String id);
}
