import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../entities/asset_entity.dart';

abstract class ContinuityMapRepository {
  Future<Either<Failure, List<AssetEntity>>> getAssets();
}
