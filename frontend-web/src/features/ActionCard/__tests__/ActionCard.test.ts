import { describe, it, expect } from 'vitest';
import { ActionCard } from '../../../types/actionCard';
import { CryptoService } from '../../../services/cryptoService';

describe('ActionCard Business Logic & Zero-Knowledge Verification', () => {
  const sampleCards: ActionCard[] = [
    {
      id: 'card-1',
      ownerId: 'owner-1',
      categoryId: 'cat-1',
      categoryCode: 'FINANCIAL',
      categoryNameVi: 'Tài chính',
      title: 'Xử lý nợ ngân hàng',
      urgency: 'IMMEDIATE',
      priority: 'CRITICAL',
      hasConfidentialInstructions: true,
      cipherInstructionsBlob: 'blob1',
      cipherNonce: 'nonce1',
      cipherAuthTag: 'tag1',
      isCompleted: true,
      rowVersion: 1,
      createdAtUtc: new Date().toISOString(),
      steps: [
        { id: 's1', actionCardId: 'card-1', stepOrder: 1, instruction: 'Gọi điện', isCompleted: true },
      ],
      contacts: [],
    },
    {
      id: 'card-2',
      ownerId: 'owner-1',
      categoryId: 'cat-2',
      categoryCode: 'PROPERTY',
      categoryNameVi: 'Bất động sản',
      title: 'Quản lý nhà cho thuê',
      urgency: 'FIRST_72_HOURS',
      priority: 'IMPORTANT',
      hasConfidentialInstructions: false,
      isCompleted: false,
      rowVersion: 1,
      createdAtUtc: new Date().toISOString(),
      steps: [],
      contacts: [],
    },
    {
      id: 'card-3',
      ownerId: 'owner-1',
      categoryId: 'cat-3',
      categoryCode: 'DOCUMENTS',
      categoryNameVi: 'Hồ sơ',
      title: 'Mở két sắt gia đình',
      urgency: 'FIRST_7_DAYS',
      priority: 'IMPORTANT',
      hasConfidentialInstructions: false,
      isCompleted: false,
      rowVersion: 1,
      createdAtUtc: new Date().toISOString(),
      steps: [],
      contacts: [],
    },
  ];

  it('should correctly partition cards into 4 urgency stages', () => {
    const immediate = sampleCards.filter((c) => c.urgency === 'IMMEDIATE');
    const first72h = sampleCards.filter((c) => c.urgency === 'FIRST_72_HOURS');
    const first7d = sampleCards.filter((c) => c.urgency === 'FIRST_7_DAYS');
    const longerTerm = sampleCards.filter((c) => c.urgency === 'LONGER_TERM');

    expect(immediate.length).toBe(1);
    expect(first72h.length).toBe(1);
    expect(first7d.length).toBe(1);
    expect(longerTerm.length).toBe(0);
  });

  it('should encrypt and decrypt confidential instructions using Web Crypto AES-256-GCM', async () => {
    const masterPassphrase = 'AssetaMasterVaultKey#2026';
    const cardId = 'a1111111-1111-1111-1111-111111111111';
    const salt = new TextEncoder().encode(cardId.substring(0, 16).padEnd(16, '0'));

    const key = await CryptoService.deriveMasterKey(masterPassphrase, salt);
    const confidentialNote = 'Mã két sắt phòng làm việc là 889911. Hợp đồng bảo hiểm AIA nằm ở ngăn kéo thứ hai có chìa khóa riêng.';

    const encrypted = await CryptoService.encrypt(key, confidentialNote);

    // Verify ciphertext does not leak plaintext
    expect(encrypted.cipherNotesBlob).not.toContain('889911');
    expect(encrypted.cipherNotesBlob).not.toContain('AIA');

    // Decrypt
    const decrypted = await CryptoService.decrypt(
      key,
      encrypted.cipherNotesBlob,
      encrypted.cipherNonce,
      encrypted.cipherAuthTag
    );

    expect(decrypted).toBe(confidentialNote);
  });

  it('should detect credit card pattern in unencrypted fields', () => {
    const ccRegex = /\b(?:\d[ -]*?){13,19}\b/;
    const dangerousInput = 'Thẻ Visa thanh toán số 4111 2222 3333 4444 hạn dùng 12/28';
    const safeInput = 'Thẻ Sacombank liên kết tài khoản lương';

    expect(ccRegex.test(dangerousInput)).toBe(true);
    expect(ccRegex.test(safeInput)).toBe(false);
  });
});
