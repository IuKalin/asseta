import '../../domain/entities/trusted_person_entity.dart';

class ScopedPermissionModel extends ScopedPermissionEntity {
  const ScopedPermissionModel({
    required super.id,
    required super.trustedPersonId,
    required super.permissionType,
    super.targetCategoryId,
    super.categoryNameVi,
    super.targetActionCardId,
    super.actionCardTitle,
    super.canView = true,
  });

  factory ScopedPermissionModel.fromJson(Map<String, dynamic> json) {
    return ScopedPermissionModel(
      id: json['id'] as String,
      trustedPersonId: json['trustedPersonId'] as String,
      permissionType: json['permissionType'] as String? ?? 'Category',
      targetCategoryId: json['targetCategoryId'] as String?,
      categoryNameVi: json['categoryNameVi'] as String?,
      targetActionCardId: json['targetActionCardId'] as String?,
      actionCardTitle: json['actionCardTitle'] as String?,
      canView: json['canView'] as bool? ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'trustedPersonId': trustedPersonId,
      'permissionType': permissionType,
      'targetCategoryId': targetCategoryId,
      'categoryNameVi': categoryNameVi,
      'targetActionCardId': targetActionCardId,
      'actionCardTitle': actionCardTitle,
      'canView': canView,
    };
  }
}

class TrustedPersonModel extends TrustedPersonEntity {
  const TrustedPersonModel({
    required super.id,
    required super.ownerId,
    super.delegateUserId,
    required super.fullName,
    required super.email,
    required super.phoneNumber,
    required super.relationship,
    super.roleDescription,
    required super.trustLevel,
    required super.status,
    super.activePairingCode,
    super.pairingExpiresAt,
    required super.rowVersion,
    required super.createdAtUtc,
    required super.permissions,
  });

  factory TrustedPersonModel.fromJson(Map<String, dynamic> json) {
    return TrustedPersonModel(
      id: json['id'] as String,
      ownerId: json['ownerId'] as String,
      delegateUserId: json['delegateUserId'] as String?,
      fullName: json['fullName'] as String,
      email: json['email'] as String,
      phoneNumber: json['phoneNumber'] as String,
      relationship: json['relationship'] as String,
      roleDescription: json['roleDescription'] as String?,
      trustLevel: json['trustLevel'] as int? ?? 1,
      status: TrustedPersonStatusX.fromString(json['status'] as String? ?? 'Invited'),
      activePairingCode: json['activePairingCode'] as String?,
      pairingExpiresAt: json['pairingExpiresAt'] != null
          ? DateTime.parse(json['pairingExpiresAt'] as String)
          : null,
      rowVersion: json['rowVersion'] as int? ?? 1,
      createdAtUtc: json['createdAtUtc'] != null
          ? DateTime.parse(json['createdAtUtc'] as String)
          : DateTime.now(),
      permissions: (json['permissions'] as List<dynamic>?)
              ?.map((e) => ScopedPermissionModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          [],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'ownerId': ownerId,
      'delegateUserId': delegateUserId,
      'fullName': fullName,
      'email': email,
      'phoneNumber': phoneNumber,
      'relationship': relationship,
      'roleDescription': roleDescription,
      'trustLevel': trustLevel,
      'status': status.toShortString(),
      'activePairingCode': activePairingCode,
      'pairingExpiresAt': pairingExpiresAt?.toIso8601String(),
      'rowVersion': rowVersion,
      'createdAtUtc': createdAtUtc.toIso8601String(),
      'permissions': permissions
          .map((p) => (p as ScopedPermissionModel).toJson())
          .toList(),
    };
  }
}

class ClaimPairingResultModel extends ClaimPairingResultEntity {
  const ClaimPairingResultModel({
    required super.trustedPersonId,
    required super.ownerId,
    required super.ownerDisplayName,
    required super.assignedRole,
    required super.status,
    required super.pairedAtUtc,
  });

  factory ClaimPairingResultModel.fromJson(Map<String, dynamic> json) {
    return ClaimPairingResultModel(
      trustedPersonId: json['trustedPersonId'] as String,
      ownerId: json['ownerId'] as String,
      ownerDisplayName: json['ownerDisplayName'] as String? ?? 'Chủ tài sản',
      assignedRole: json['assignedRole'] as String? ?? '',
      status: json['status'] as String? ?? 'Active',
      pairedAtUtc: json['pairedAtUtc'] != null
          ? DateTime.parse(json['pairedAtUtc'] as String)
          : DateTime.now(),
    );
  }
}
