import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/entities/asset_entity.dart';
import '../../domain/repositories/continuity_map_repository.dart';
import '../models/asset_model.dart';

class ContinuityMapRepositoryImpl implements ContinuityMapRepository {
  @override
  Future<Either<Failure, List<AssetEntity>>> getAssets() async {
    try {
      // Mock data aligned with .sdd/specs/feat-01-continuity-map/
      final assets = [
        const AssetModel(
          id: '1',
          name: 'Real Estate Villa',
          description: 'Primary family residence',
          type: 2,
          estimatedValue: 500000.0,
        ),
        const AssetModel(
          id: '2',
          name: 'Global Equity Portfolio',
          description: 'Stock & ETF investments',
          type: 1,
          estimatedValue: 250000.0,
        ),
        const AssetModel(
          id: '3',
          name: 'Cold Storage Vault',
          description: 'Digital crypto assets (BTC)',
          type: 3,
          estimatedValue: 120000.0,
        ),
      ];
      return Right(assets);
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
