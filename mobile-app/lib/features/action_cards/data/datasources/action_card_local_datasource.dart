import '../../domain/entities/action_card_entity.dart';

abstract class ActionCardLocalDataSource {
  Future<List<ActionCardEntity>?> getCachedCards();
  Future<void> cacheCards(List<ActionCardEntity> cards);
  Future<void> clearCache();
}

class InMemoryActionCardLocalDataSource implements ActionCardLocalDataSource {
  List<ActionCardEntity>? _cachedCards;

  @override
  Future<List<ActionCardEntity>?> getCachedCards() async {
    return _cachedCards;
  }

  @override
  Future<void> cacheCards(List<ActionCardEntity> cards) async {
    _cachedCards = List.unmodifiable(cards);
  }

  @override
  Future<void> clearCache() async {
    _cachedCards = null;
  }
}
