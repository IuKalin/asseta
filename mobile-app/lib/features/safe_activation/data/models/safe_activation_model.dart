import '../../domain/entities/safe_activation_entity.dart';

class ActivationConfirmationModel extends ActivationConfirmationEntity {
  const ActivationConfirmationModel({
    required super.id,
    required super.trustedPersonId,
    super.trustedPersonName,
    required super.isConfirmed,
    super.note,
    required super.confirmedAtUtc,
  });

  factory ActivationConfirmationModel.fromJson(Map<String, dynamic> json) {
    return ActivationConfirmationModel(
      id: json['id'] as String,
      trustedPersonId: json['trustedPersonId'] as String,
      trustedPersonName: json['trustedPersonName'] as String?,
      isConfirmed: json['isConfirmed'] as bool? ?? false,
      note: json['note'] as String?,
      confirmedAtUtc: DateTime.parse(json['confirmedAtUtc'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'trustedPersonId': trustedPersonId,
      'trustedPersonName': trustedPersonName,
      'isConfirmed': isConfirmed,
      'note': note,
      'confirmedAtUtc': confirmedAtUtc.toIso8601String(),
    };
  }
}

class ActivationRequestModel extends ActivationRequestEntity {
  const ActivationRequestModel({
    required super.id,
    required super.ownerId,
    required super.triggerSource,
    super.initiatedByTrustedPersonId,
    super.reason,
    required super.status,
    required super.gracePeriodExpiresAtUtc,
    required super.remainingSeconds,
    required super.confirmationsCount,
    required super.minConfirmationsRequired,
    required super.rowVersion,
    required super.confirmations,
  });

  factory ActivationRequestModel.fromJson(Map<String, dynamic> json) {
    final rawConfirmations = json['confirmations'] as List<dynamic>? ?? [];
    return ActivationRequestModel(
      id: json['id'] as String,
      ownerId: json['ownerId'] as String,
      triggerSource: json['triggerSource'] as String? ?? 'TrustedPersonRequest',
      initiatedByTrustedPersonId: json['initiatedByTrustedPersonId'] as String?,
      reason: json['reason'] as String?,
      status: json['status'] as String? ?? 'PendingGracePeriod',
      gracePeriodExpiresAtUtc: DateTime.parse(json['gracePeriodExpiresAtUtc'] as String),
      remainingSeconds: (json['remainingSeconds'] as num?)?.toInt() ?? 0,
      confirmationsCount: (json['confirmationsCount'] as num?)?.toInt() ?? 0,
      minConfirmationsRequired: (json['minConfirmationsRequired'] as num?)?.toInt() ?? 1,
      rowVersion: (json['rowVersion'] as num?)?.toInt() ?? 1,
      confirmations: rawConfirmations
          .map((c) => ActivationConfirmationModel.fromJson(c as Map<String, dynamic>))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'ownerId': ownerId,
      'triggerSource': triggerSource,
      'initiatedByTrustedPersonId': initiatedByTrustedPersonId,
      'reason': reason,
      'status': status,
      'gracePeriodExpiresAtUtc': gracePeriodExpiresAtUtc.toIso8601String(),
      'remainingSeconds': remainingSeconds,
      'confirmationsCount': confirmationsCount,
      'minConfirmationsRequired': minConfirmationsRequired,
      'rowVersion': rowVersion,
      'confirmations': confirmations
          .map((c) => (c as ActivationConfirmationModel).toJson())
          .toList(),
    };
  }
}

class ActivationStatusModel extends ActivationStatusEntity {
  const ActivationStatusModel({
    required super.ownerId,
    required super.checkInIntervalDays,
    required super.gracePeriodHours,
    required super.minConfirmationsRequired,
    super.lastCheckInAtUtc,
    super.nextCheckInDueUtc,
    required super.isDueSoon,
    required super.isOverdue,
    required super.heartbeatStatus,
    required super.isEmergencyActive,
    super.activeRequest,
  });

  factory ActivationStatusModel.fromJson(Map<String, dynamic> json) {
    return ActivationStatusModel(
      ownerId: json['ownerId'] as String,
      checkInIntervalDays: (json['checkInIntervalDays'] as num?)?.toInt() ?? 30,
      gracePeriodHours: (json['gracePeriodHours'] as num?)?.toInt() ?? 48,
      minConfirmationsRequired: (json['minConfirmationsRequired'] as num?)?.toInt() ?? 1,
      lastCheckInAtUtc: json['lastCheckInAtUtc'] != null
          ? DateTime.parse(json['lastCheckInAtUtc'] as String)
          : null,
      nextCheckInDueUtc: json['nextCheckInDueUtc'] != null
          ? DateTime.parse(json['nextCheckInDueUtc'] as String)
          : null,
      isDueSoon: json['isDueSoon'] as bool? ?? false,
      isOverdue: json['isOverdue'] as bool? ?? false,
      heartbeatStatus: HeartbeatStatusX.fromString(json['heartbeatStatus'] as String? ?? 'ACTIVE'),
      isEmergencyActive: json['isEmergencyActive'] as bool? ?? false,
      activeRequest: json['activeRequest'] != null
          ? ActivationRequestModel.fromJson(json['activeRequest'] as Map<String, dynamic>)
          : null,
    );
  }
}

class ActivationConfigModel extends ActivationConfigEntity {
  const ActivationConfigModel({
    required super.checkInIntervalDays,
    required super.gracePeriodHours,
    required super.minConfirmationsRequired,
  });

  factory ActivationConfigModel.fromJson(Map<String, dynamic> json) {
    return ActivationConfigModel(
      checkInIntervalDays: (json['checkInIntervalDays'] as num?)?.toInt() ?? 30,
      gracePeriodHours: (json['gracePeriodHours'] as num?)?.toInt() ?? 48,
      minConfirmationsRequired: (json['minConfirmationsRequired'] as num?)?.toInt() ?? 1,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'checkInIntervalDays': checkInIntervalDays,
      'gracePeriodHours': gracePeriodHours,
      'minConfirmationsRequired': minConfirmationsRequired,
    };
  }
}
