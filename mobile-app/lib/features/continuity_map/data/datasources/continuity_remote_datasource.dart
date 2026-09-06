import 'package:dio/dio.dart';
import '../../domain/entities/continuity_item_entity.dart';
import '../../domain/entities/continuity_map_entity.dart';

abstract class ContinuityRemoteDataSource {
  Future<ContinuityMapEntity> getContinuityMap([String? ownerId]);
  Future<ContinuityItemEntity> createContinuityItem({
    required String categoryId,
    required String name,
    required ItemPriority priority,
    String? documentLocationHint,
    String? assignedTrustedPersonId,
    String? cipherNotesBlob,
    String? cipherNonce,
    String? cipherAuthTag,
  });
  Future<ContinuityItemEntity> updateContinuityItem({
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
  Future<void> deleteContinuityItem(String id);
}

class ContinuityRemoteDataSourceImpl implements ContinuityRemoteDataSource {
  final Dio dio;

  ContinuityRemoteDataSourceImpl({required this.dio});

  @override
  Future<ContinuityMapEntity> getContinuityMap([String? ownerId]) async {
    final response = await dio.get(
      '/continuity-map',
      queryParameters: ownerId != null ? {'ownerId': ownerId} : null,
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return _parseContinuityMap(data);
  }

  @override
  Future<ContinuityItemEntity> createContinuityItem({
    required String categoryId,
    required String name,
    required ItemPriority priority,
    String? documentLocationHint,
    String? assignedTrustedPersonId,
    String? cipherNotesBlob,
    String? cipherNonce,
    String? cipherAuthTag,
  }) async {
    final response = await dio.post(
      '/continuity-items',
      data: {
        'categoryId': categoryId,
        'name': name,
        'priority': priority.toShortString(),
        'documentLocationHint': documentLocationHint,
        'assignedTrustedPersonId': assignedTrustedPersonId,
        'cipherNotesBlob': cipherNotesBlob,
        'cipherNonce': cipherNonce,
        'cipherAuthTag': cipherAuthTag,
      },
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return _parseItem(data);
  }

  @override
  Future<ContinuityItemEntity> updateContinuityItem({
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
    final response = await dio.put(
      '/continuity-items/$id',
      data: {
        'name': name,
        'priority': priority.toShortString(),
        'documentLocationHint': documentLocationHint,
        'assignedTrustedPersonId': assignedTrustedPersonId,
        'cipherNotesBlob': cipherNotesBlob,
        'cipherNonce': cipherNonce,
        'cipherAuthTag': cipherAuthTag,
        'rowVersion': rowVersion,
      },
    );

    final data = response.data['data'] as Map<String, dynamic>;
    return _parseItem(data);
  }

  @override
  Future<void> deleteContinuityItem(String id) async {
    await dio.delete('/continuity-items/$id');
  }

  ContinuityMapEntity _parseContinuityMap(Map<String, dynamic> json) {
    final rawCategories = json['categories'] as List<dynamic>? ?? [];
    final categories = rawCategories.map((c) {
      final catMap = c as Map<String, dynamic>;
      final rawItems = catMap['items'] as List<dynamic>? ?? [];
      final items = rawItems.map((i) => _parseItem(i as Map<String, dynamic>)).toList();

      return ContinuityCategoryEntity(
        categoryId: catMap['categoryId'] ?? '',
        code: catMap['code'] ?? '',
        name: catMap['name'] ?? '',
        icon: catMap['icon'] ?? '',
        readinessScore: catMap['readinessScore'] ?? 0,
        items: items,
      );
    }).toList();

    final rawGaps = json['gaps'] as List<dynamic>? ?? [];
    final gaps = rawGaps.map((g) {
      final gapMap = g as Map<String, dynamic>;
      final missing = (gapMap['missingFields'] as List<dynamic>? ?? [])
          .map((m) => m.toString())
          .toList();
      return ContinuityGapEntity(
        itemId: gapMap['itemId'] ?? '',
        itemName: gapMap['itemName'] ?? '',
        categoryCode: gapMap['categoryCode'] ?? '',
        priority: gapMap['priority'] ?? '',
        missingFields: missing,
      );
    }).toList();

    return ContinuityMapEntity(
      overallReadinessScore: json['overallReadinessScore'] ?? 0,
      totalItems: json['totalItems'] ?? 0,
      totalGaps: json['totalGaps'] ?? 0,
      categories: categories,
      gaps: gaps,
    );
  }

  ContinuityItemEntity _parseItem(Map<String, dynamic> json) {
    return ContinuityItemEntity(
      id: json['id'] ?? '',
      ownerId: json['ownerId'] ?? '',
      categoryId: json['categoryId'] ?? '',
      categoryCode: json['categoryCode'] ?? '',
      name: json['name'] ?? '',
      priority: ItemPriorityX.fromString(json['priority'] ?? 'IMPORTANT'),
      documentLocationHint: json['documentLocationHint'],
      assignedTrustedPersonId: json['assignedTrustedPersonId'],
      actionCardId: json['actionCardId'],
      hasConfidentialNotes: json['hasConfidentialNotes'] ?? false,
      cipherNotesBlob: json['cipherNotesBlob'],
      cipherNonce: json['cipherNonce'],
      cipherAuthTag: json['cipherAuthTag'],
      isCompleted: json['isCompleted'] ?? false,
      rowVersion: json['rowVersion'] ?? 1,
      sortOrder: json['sortOrder'] ?? 0,
      createdAtUtc: DateTime.tryParse(json['createdAtUtc'] ?? '') ?? DateTime.now(),
    );
  }
}
