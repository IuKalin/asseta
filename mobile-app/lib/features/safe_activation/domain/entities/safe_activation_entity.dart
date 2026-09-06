import 'package:equatable/equatable.dart';

enum HeartbeatStatus { active, warning, pendingGracePeriod, activated }

extension HeartbeatStatusX on HeartbeatStatus {
  String toShortString() {
    switch (this) {
      case HeartbeatStatus.active:
        return 'ACTIVE';
      case HeartbeatStatus.warning:
        return 'WARNING';
      case HeartbeatStatus.pendingGracePeriod:
        return 'PENDING_GRACE_PERIOD';
      case HeartbeatStatus.activated:
        return 'ACTIVATED';
    }
  }

  String get displayNameVi {
    switch (this) {
      case HeartbeatStatus.active:
        return 'Bình thường';
      case HeartbeatStatus.warning:
        return 'Sắp đến hạn điểm danh';
      case HeartbeatStatus.pendingGracePeriod:
        return 'Đang đệm chờ kích hoạt';
      case HeartbeatStatus.activated:
        return 'Đã kích hoạt khẩn cấp';
    }
  }

  static HeartbeatStatus fromString(String val) {
    switch (val.toUpperCase()) {
      case 'WARNING':
        return HeartbeatStatus.warning;
      case 'PENDING_GRACE_PERIOD':
      case 'PENDINGGRACEPERIOD':
        return HeartbeatStatus.pendingGracePeriod;
      case 'ACTIVATED':
        return HeartbeatStatus.activated;
      case 'ACTIVE':
      default:
        return HeartbeatStatus.active;
    }
  }
}

class ActivationConfirmationEntity extends Equatable {
  final String id;
  final String trustedPersonId;
  final String? trustedPersonName;
  final bool isConfirmed;
  final String? note;
  final DateTime confirmedAtUtc;

  const ActivationConfirmationEntity({
    required this.id,
    required this.trustedPersonId,
    this.trustedPersonName,
    required this.isConfirmed,
    this.note,
    required this.confirmedAtUtc,
  });

  @override
  List<Object?> get props => [id, trustedPersonId, isConfirmed, note, confirmedAtUtc];
}

class ActivationRequestEntity extends Equatable {
  final String id;
  final String ownerId;
  final String triggerSource;
  final String? initiatedByTrustedPersonId;
  final String? reason;
  final String status;
  final DateTime gracePeriodExpiresAtUtc;
  final int remainingSeconds;
  final int confirmationsCount;
  final int minConfirmationsRequired;
  final int rowVersion;
  final List<ActivationConfirmationEntity> confirmations;

  const ActivationRequestEntity({
    required this.id,
    required this.ownerId,
    required this.triggerSource,
    this.initiatedByTrustedPersonId,
    this.reason,
    required this.status,
    required this.gracePeriodExpiresAtUtc,
    required this.remainingSeconds,
    required this.confirmationsCount,
    required this.minConfirmationsRequired,
    required this.rowVersion,
    required this.confirmations,
  });

  bool get isPendingGracePeriod => status.toUpperCase() == 'PENDINGGRACEPERIOD' || status.toUpperCase() == 'PENDING_GRACE_PERIOD';

  @override
  List<Object?> get props => [
        id,
        ownerId,
        triggerSource,
        initiatedByTrustedPersonId,
        reason,
        status,
        gracePeriodExpiresAtUtc,
        remainingSeconds,
        confirmationsCount,
        minConfirmationsRequired,
        rowVersion,
        confirmations,
      ];
}

class ActivationStatusEntity extends Equatable {
  final String ownerId;
  final int checkInIntervalDays;
  final int gracePeriodHours;
  final int minConfirmationsRequired;
  final DateTime? lastCheckInAtUtc;
  final DateTime? nextCheckInDueUtc;
  final bool isDueSoon;
  final bool isOverdue;
  final HeartbeatStatus heartbeatStatus;
  final bool isEmergencyActive;
  final ActivationRequestEntity? activeRequest;

  const ActivationStatusEntity({
    required this.ownerId,
    required this.checkInIntervalDays,
    required this.gracePeriodHours,
    required this.minConfirmationsRequired,
    this.lastCheckInAtUtc,
    this.nextCheckInDueUtc,
    required this.isDueSoon,
    required this.isOverdue,
    required this.heartbeatStatus,
    required this.isEmergencyActive,
    this.activeRequest,
  });

  @override
  List<Object?> get props => [
        ownerId,
        checkInIntervalDays,
        gracePeriodHours,
        minConfirmationsRequired,
        lastCheckInAtUtc,
        nextCheckInDueUtc,
        isDueSoon,
        isOverdue,
        heartbeatStatus,
        isEmergencyActive,
        activeRequest,
      ];
}

class ActivationConfigEntity extends Equatable {
  final int checkInIntervalDays;
  final int gracePeriodHours;
  final int minConfirmationsRequired;

  const ActivationConfigEntity({
    required this.checkInIntervalDays,
    required this.gracePeriodHours,
    required this.minConfirmationsRequired,
  });

  @override
  List<Object?> get props => [
        checkInIntervalDays,
        gracePeriodHours,
        minConfirmationsRequired,
      ];
}
