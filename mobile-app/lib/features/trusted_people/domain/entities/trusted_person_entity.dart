import 'package:equatable/equatable.dart';

enum TrustedPersonStatus { invited, active, suspended, revoked }

extension TrustedPersonStatusX on TrustedPersonStatus {
  String toShortString() {
    switch (this) {
      case TrustedPersonStatus.invited:
        return 'Invited';
      case TrustedPersonStatus.active:
        return 'Active';
      case TrustedPersonStatus.suspended:
        return 'Suspended';
      case TrustedPersonStatus.revoked:
        return 'Revoked';
    }
  }

  String get displayNameVi {
    switch (this) {
      case TrustedPersonStatus.invited:
        return 'Đang chờ ghép đôi';
      case TrustedPersonStatus.active:
        return 'Đã liên kết';
      case TrustedPersonStatus.suspended:
        return 'Tạm ngưng';
      case TrustedPersonStatus.revoked:
        return 'Đã thu hồi';
    }
  }

  static TrustedPersonStatus fromString(String val) {
    switch (val.toUpperCase()) {
      case 'ACTIVE':
        return TrustedPersonStatus.active;
      case 'SUSPENDED':
        return TrustedPersonStatus.suspended;
      case 'REVOKED':
        return TrustedPersonStatus.revoked;
      case 'INVITED':
      default:
        return TrustedPersonStatus.invited;
    }
  }
}

class ScopedPermissionEntity extends Equatable {
  final String id;
  final String trustedPersonId;
  final String permissionType;
  final String? targetCategoryId;
  final String? categoryNameVi;
  final String? targetActionCardId;
  final String? actionCardTitle;
  final bool canView;

  const ScopedPermissionEntity({
    required this.id,
    required this.trustedPersonId,
    required this.permissionType,
    this.targetCategoryId,
    this.categoryNameVi,
    this.targetActionCardId,
    this.actionCardTitle,
    this.canView = true,
  });

  @override
  List<Object?> get props => [
        id,
        trustedPersonId,
        permissionType,
        targetCategoryId,
        categoryNameVi,
        targetActionCardId,
        actionCardTitle,
        canView,
      ];
}

class TrustedPersonEntity extends Equatable {
  final String id;
  final String ownerId;
  final String? delegateUserId;
  final String fullName;
  final String email;
  final String phoneNumber;
  final String relationship;
  final String? roleDescription;
  final int trustLevel;
  final TrustedPersonStatus status;
  final String? activePairingCode;
  final DateTime? pairingExpiresAt;
  final int rowVersion;
  final DateTime createdAtUtc;
  final List<ScopedPermissionEntity> permissions;

  const TrustedPersonEntity({
    required this.id,
    required this.ownerId,
    this.delegateUserId,
    required this.fullName,
    required this.email,
    required this.phoneNumber,
    required this.relationship,
    this.roleDescription,
    required this.trustLevel,
    required this.status,
    this.activePairingCode,
    this.pairingExpiresAt,
    required this.rowVersion,
    required this.createdAtUtc,
    required this.permissions,
  });

  String get trustLevelLabel {
    switch (trustLevel) {
      case 1:
        return 'Level 1: Notice Only';
      case 2:
        return 'Level 2: Scoped Delegate';
      case 3:
        return 'Level 3: Primary Delegate';
      default:
        return 'Level $trustLevel';
    }
  }

  @override
  List<Object?> get props => [
        id,
        ownerId,
        delegateUserId,
        fullName,
        email,
        phoneNumber,
        relationship,
        roleDescription,
        trustLevel,
        status,
        activePairingCode,
        pairingExpiresAt,
        rowVersion,
        createdAtUtc,
        permissions,
      ];
}

class ClaimPairingResultEntity extends Equatable {
  final String trustedPersonId;
  final String ownerId;
  final String ownerDisplayName;
  final String assignedRole;
  final String status;
  final DateTime pairedAtUtc;

  const ClaimPairingResultEntity({
    required this.trustedPersonId,
    required this.ownerId,
    required this.ownerDisplayName,
    required this.assignedRole,
    required this.status,
    required this.pairedAtUtc,
  });

  @override
  List<Object?> get props => [
        trustedPersonId,
        ownerId,
        ownerDisplayName,
        assignedRole,
        status,
        pairedAtUtc,
      ];
}
