import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/features/safe_activation/domain/entities/safe_activation_entity.dart';
import 'package:asseta_mobile/features/safe_activation/data/models/safe_activation_model.dart';

void main() {
  group('SafeActivation Model & JSON Serialization', () {
    test('HeartbeatStatusX string conversions', () {
      expect(HeartbeatStatusX.fromString('ACTIVE'), HeartbeatStatus.active);
      expect(HeartbeatStatusX.fromString('WARNING'), HeartbeatStatus.warning);
      expect(HeartbeatStatusX.fromString('PENDING_GRACE_PERIOD'), HeartbeatStatus.pendingGracePeriod);
      expect(HeartbeatStatusX.fromString('PENDINGGRACEPERIOD'), HeartbeatStatus.pendingGracePeriod);
      expect(HeartbeatStatusX.fromString('ACTIVATED'), HeartbeatStatus.activated);

      expect(HeartbeatStatus.active.toShortString(), 'ACTIVE');
      expect(HeartbeatStatus.pendingGracePeriod.toShortString(), 'PENDING_GRACE_PERIOD');
      expect(HeartbeatStatus.active.displayNameVi, 'Bình thường');
      expect(HeartbeatStatus.pendingGracePeriod.displayNameVi, 'Đang đệm chờ kích hoạt');
    });

    test('ActivationStatusModel parses JSON correctly', () {
      final json = {
        'ownerId': 'owner-123',
        'checkInIntervalDays': 30,
        'gracePeriodHours': 48,
        'minConfirmationsRequired': 2,
        'lastCheckInAtUtc': '2026-09-01T10:00:00.000Z',
        'nextCheckInDueUtc': '2026-10-01T10:00:00.000Z',
        'isDueSoon': false,
        'isOverdue': false,
        'heartbeatStatus': 'ACTIVE',
        'isEmergencyActive': false,
        'activeRequest': null,
      };

      final model = ActivationStatusModel.fromJson(json);

      expect(model.ownerId, 'owner-123');
      expect(model.checkInIntervalDays, 30);
      expect(model.gracePeriodHours, 48);
      expect(model.minConfirmationsRequired, 2);
      expect(model.heartbeatStatus, HeartbeatStatus.active);
      expect(model.isEmergencyActive, false);
      expect(model.activeRequest, isNull);
    });

    test('ActivationRequestModel parses JSON with confirmations correctly', () {
      final json = {
        'id': 'req-999',
        'ownerId': 'owner-123',
        'triggerSource': 'TrustedPersonRequest',
        'initiatedByTrustedPersonId': 'tp-1',
        'reason': 'Mất liên lạc khẩn cấp',
        'status': 'PendingGracePeriod',
        'gracePeriodExpiresAtUtc': '2026-09-07T10:00:00.000Z',
        'remainingSeconds': 172800,
        'confirmationsCount': 1,
        'minConfirmationsRequired': 2,
        'rowVersion': 1,
        'confirmations': [
          {
            'id': 'conf-1',
            'trustedPersonId': 'tp-1',
            'trustedPersonName': 'Người ủy thác 1',
            'isConfirmed': true,
            'note': 'Khẩn cấp',
            'confirmedAtUtc': '2026-09-05T10:00:00.000Z',
          }
        ],
      };

      final model = ActivationRequestModel.fromJson(json);

      expect(model.id, 'req-999');
      expect(model.triggerSource, 'TrustedPersonRequest');
      expect(model.status, 'PendingGracePeriod');
      expect(model.isPendingGracePeriod, isTrue);
      expect(model.remainingSeconds, 172800);
      expect(model.confirmationsCount, 1);
      expect(model.confirmations.length, 1);
      expect(model.confirmations.first.trustedPersonName, 'Người ủy thác 1');
    });

    test('ActivationConfigModel toJson produces expected payload', () {
      const model = ActivationConfigModel(
        checkInIntervalDays: 60,
        gracePeriodHours: 72,
        minConfirmationsRequired: 3,
      );

      final json = model.toJson();
      expect(json['checkInIntervalDays'], 60);
      expect(json['gracePeriodHours'], 72);
      expect(json['minConfirmationsRequired'], 3);
    });
  });
}
