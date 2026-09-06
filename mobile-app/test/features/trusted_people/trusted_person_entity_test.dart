import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/features/trusted_people/domain/entities/trusted_person_entity.dart';
import 'package:asseta_mobile/features/trusted_people/data/models/trusted_person_model.dart';

void main() {
  group('TrustedPerson Entities & Models', () {
    test('TrustedPersonModel should correctly parse from JSON and serialize back', () {
      final json = {
        'id': 'tp-1',
        'ownerId': 'owner-1',
        'delegateUserId': 'user-1',
        'fullName': 'Nguyễn Văn B',
        'email': 'b@example.com',
        'phoneNumber': '0901234567',
        'relationship': 'Luật sư',
        'roleDescription': 'Pháp lý',
        'trustLevel': 2,
        'status': 'Active',
        'activePairingCode': 'AS7K9P',
        'pairingExpiresAt': '2026-09-07T14:30:00.000Z',
        'rowVersion': 1,
        'createdAtUtc': '2026-09-05T14:30:00.000Z',
        'permissions': [
          {
            'id': 'perm-1',
            'trustedPersonId': 'tp-1',
            'permissionType': 'Category',
            'targetCategoryId': 'cat-1',
            'categoryNameVi': 'Tài chính',
            'targetActionCardId': null,
            'actionCardTitle': null,
            'canView': true,
          }
        ],
      };

      final model = TrustedPersonModel.fromJson(json);

      expect(model.id, 'tp-1');
      expect(model.fullName, 'Nguyễn Văn B');
      expect(model.status, TrustedPersonStatus.active);
      expect(model.trustLevel, 2);
      expect(model.trustLevelLabel, 'Level 2: Scoped Delegate');
      expect(model.permissions.length, 1);
      expect(model.permissions[0].categoryNameVi, 'Tài chính');

      final serialized = model.toJson();
      expect(serialized['id'], 'tp-1');
      expect(serialized['status'], 'Active');
    });

    test('TrustedPersonStatusX should correctly map string values', () {
      expect(TrustedPersonStatusX.fromString('INVITED'), TrustedPersonStatus.invited);
      expect(TrustedPersonStatusX.fromString('ACTIVE'), TrustedPersonStatus.active);
      expect(TrustedPersonStatusX.fromString('SUSPENDED'), TrustedPersonStatus.suspended);
      expect(TrustedPersonStatusX.fromString('REVOKED'), TrustedPersonStatus.revoked);
      expect(TrustedPersonStatus.invited.displayNameVi, 'Đang chờ ghép đôi');
      expect(TrustedPersonStatus.active.displayNameVi, 'Đã liên kết');
    });
  });
}
