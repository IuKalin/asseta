import 'package:equatable/equatable.dart';

class AssetEntity extends Equatable {
  final String id;
  final String name;
  final String description;
  final int type;
  final double estimatedValue;

  const AssetEntity({
    required this.id,
    required this.name,
    required this.description,
    required this.type,
    required this.estimatedValue,
  });

  @override
  List<Object?> get props => [id, name, description, type, estimatedValue];
}
