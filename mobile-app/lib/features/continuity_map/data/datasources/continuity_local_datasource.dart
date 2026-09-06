import '../../domain/entities/continuity_map_entity.dart';

abstract class ContinuityLocalDataSource {
  Future<ContinuityMapEntity?> getCachedMap();
  Future<void> cacheMap(ContinuityMapEntity map);
  Future<void> clearCache();
}

class InMemoryContinuityLocalDataSource implements ContinuityLocalDataSource {
  ContinuityMapEntity? _cachedMap;

  @override
  Future<ContinuityMapEntity?> getCachedMap() async {
    return _cachedMap;
  }

  @override
  Future<void> cacheMap(ContinuityMapEntity map) async {
    _cachedMap = map;
  }

  @override
  Future<void> clearCache() async {
    _cachedMap = null;
  }
}
