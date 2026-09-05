import '../../domain/entities/asset_entity.dart';

class AssetModel extends AssetEntity {
  const AssetModel({
    required super.id,
    required super.name,
    required super.description,
    required super.type,
    required super.estimatedValue,
  });

  factory AssetModel.fromJson(Map<String, dynamic> json) {
    return AssetModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      type: json['type'] ?? 1,
      estimatedValue: (json['estimatedValue'] as num?)?.toDouble() ?? 0.0,
    );
  }
}
