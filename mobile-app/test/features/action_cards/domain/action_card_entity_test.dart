import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_card_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_card_enums.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_step_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_contact_entity.dart';
import 'package:asseta_mobile/features/action_cards/domain/entities/action_template_entity.dart';

void main() {
  group('ActionCard Entities Serialization & Props', () {
    test('ActionStepEntity should correctly serialize to and from JSON', () {
      final step = ActionStepEntity(
        id: 's1',
        actionCardId: 'card1',
        stepOrder: 1,
        instruction: 'Liên hệ cán bộ tín dụng',
        estimatedDuration: '30 phút',
        isCompleted: true,
      );

      final json = step.toJson();
      final restored = ActionStepEntity.fromJson(json);

      expect(restored.id, 's1');
      expect(restored.stepOrder, 1);
      expect(restored.instruction, 'Liên hệ cán bộ tín dụng');
      expect(restored.estimatedDuration, '30 phút');
      expect(restored.isCompleted, true);
    });

    test('ActionContactEntity should correctly serialize to and from JSON', () {
      final contact = ActionContactEntity(
        id: 'c1',
        actionCardId: 'card1',
        contactName: 'Nguyễn Văn B',
        relationshipOrRole: 'Luật sư gia đình',
        phoneNumber: '0901234567',
        email: 'lawyer@example.com',
        contactNotes: 'Gọi trong giờ hành chính',
      );

      final json = contact.toJson();
      final restored = ActionContactEntity.fromJson(json);

      expect(restored.contactName, 'Nguyễn Văn B');
      expect(restored.relationshipOrRole, 'Luật sư gia đình');
      expect(restored.phoneNumber, '0901234567');
      expect(restored.email, 'lawyer@example.com');
    });

    test('ActionCardEntity should correctly deserialize full envelope JSON', () {
      final json = {
        'id': 'card-100',
        'ownerId': 'owner-1',
        'categoryId': 'cat-1',
        'categoryCode': 'FINANCIAL',
        'categoryNameVi': 'Tài chính',
        'title': 'Xử lý khoản vay ngân hàng',
        'summary': 'Khoản vay thế chấp mua nhà',
        'urgency': 'FIRST_72_HOURS',
        'priority': 'CRITICAL',
        'assignedTrustedPersonId': 'trustee-1',
        'documentLocationHint': 'Két sắt',
        'digitalStorageLink': 'https://storage.asseta.vn/loan',
        'hasConfidentialInstructions': true,
        'cipherInstructionsBlob': 'blob123',
        'cipherNonce': 'nonce123',
        'cipherAuthTag': 'tag123',
        'isCompleted': false,
        'rowVersion': 2,
        'createdAtUtc': '2026-09-05T12:00:00.000Z',
        'steps': [
          {
            'id': 'step-1',
            'actionCardId': 'card-100',
            'stepOrder': 1,
            'instruction': 'Kiểm tra dư nợ gốc',
            'estimatedDuration': '15 phút',
            'isCompleted': false,
          }
        ],
        'contacts': [
          {
            'id': 'cnt-1',
            'actionCardId': 'card-100',
            'contactName': 'Hoàng Văn C',
            'relationshipOrRole': 'Cán bộ tín dụng',
          }
        ],
      };

      final card = ActionCardEntity.fromJson(json);

      expect(card.id, 'card-100');
      expect(card.urgency, UrgencyStage.first72Hours);
      expect(card.priority, 'CRITICAL');
      expect(card.hasConfidentialInstructions, true);
      expect(card.steps.length, 1);
      expect(card.steps.first.instruction, 'Kiểm tra dư nợ gốc');
      expect(card.contacts.length, 1);
      expect(card.contacts.first.contactName, 'Hoàng Văn C');
    });

    test('ActionTemplateEntity should parse suggested steps and roles', () {
      final json = {
        'id': 'tpl-1',
        'templateCode': 'TPL_BANK_LOAN',
        'categoryCode': 'FINANCIAL',
        'titleVi': 'Xử lý nợ ngân hàng',
        'titleEn': 'Handle Bank Loan',
        'defaultUrgency': 'FIRST_72_HOURS',
        'defaultPriority': 'CRITICAL',
        'suggestedSteps': [
          {'stepOrder': 1, 'instruction': 'Bước 1', 'estimatedDuration': '10 phút'}
        ],
        'suggestedRoles': ['Kế toán', 'Cán bộ tín dụng'],
      };

      final tpl = ActionTemplateEntity.fromJson(json);

      expect(tpl.templateCode, 'TPL_BANK_LOAN');
      expect(tpl.defaultUrgency, UrgencyStage.first72Hours);
      expect(tpl.suggestedSteps.length, 1);
      expect(tpl.suggestedRoles.length, 2);
    });
  });
}
