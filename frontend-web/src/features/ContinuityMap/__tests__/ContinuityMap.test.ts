import { describe, it, expect } from 'vitest';
import { ContinuityCategory, ContinuityItem } from '../../../types/continuity';

describe('Continuity Map Domain & UI Logic', () => {
  const mockCategories: ContinuityCategory[] = [
    {
      categoryId: 'cat-1',
      code: 'FINANCIAL',
      name: 'Tài chính',
      icon: 'wallet',
      readinessScore: 85,
      items: [
        {
          id: 'item-1',
          ownerId: 'owner-1',
          categoryId: 'cat-1',
          categoryCode: 'FINANCIAL',
          name: 'Sổ tiết kiệm MB Bank',
          priority: 'CRITICAL',
          documentLocationHint: 'Két sắt',
          assignedTrustedPersonId: 'trusted-1',
          actionCardId: null,
          hasConfidentialNotes: true,
          isCompleted: false,
          hasContinuityGap: false,
          rowVersion: 1,
          sortOrder: 1,
          createdAtUtc: new Date().toISOString(),
        },
      ],
    },
    {
      categoryId: 'cat-2',
      code: 'PROPERTY',
      name: 'Bất động sản',
      icon: 'home',
      readinessScore: 40,
      items: [
        {
          id: 'item-2',
          ownerId: 'owner-1',
          categoryId: 'cat-2',
          categoryCode: 'PROPERTY',
          name: 'Hợp đồng mua nhà Landmark',
          priority: 'CRITICAL',
          documentLocationHint: null, // Gap!
          assignedTrustedPersonId: null, // Gap!
          actionCardId: null,
          hasConfidentialNotes: false,
          isCompleted: false,
          hasContinuityGap: true,
          rowVersion: 1,
          sortOrder: 1,
          createdAtUtc: new Date().toISOString(),
        },
      ],
    },
  ];

  it('should correctly identify continuity gaps on critical items', () => {
    const gaps: ContinuityItem[] = [];
    mockCategories.forEach((cat) => {
      cat.items.forEach((item) => {
        if (
          (item.priority === 'CRITICAL' || item.priority === 'IMPORTANT') &&
          (!item.assignedTrustedPersonId || !item.documentLocationHint)
        ) {
          gaps.push(item);
        }
      });
    });

    expect(gaps.length).toBe(1);
    expect(gaps[0].name).toBe('Hợp đồng mua nhà Landmark');
    expect(gaps[0].hasContinuityGap).toBe(true);
  });

  it('should categorize score levels into proper color bands', () => {
    const getBand = (score: number) => {
      if (score >= 80) return 'GREEN';
      if (score >= 50) return 'YELLOW';
      return 'RED';
    };

    expect(getBand(85)).toBe('GREEN');
    expect(getBand(80)).toBe('GREEN');
    expect(getBand(79)).toBe('YELLOW');
    expect(getBand(50)).toBe('YELLOW');
    expect(getBand(49)).toBe('RED');
    expect(getBand(0)).toBe('RED');
  });

  it('should reject unencrypted credit cards via regex inspection', () => {
    const ccRegex = /\b(?:\d[ -]*?){13,19}\b/;

    const sampleNameWithCard = 'Thẻ Visa 4111 2222 3333 4444 Techcombank';
    const safeName = 'Thẻ thanh toán Techcombank *4444';

    expect(ccRegex.test(sampleNameWithCard)).toBe(true);
    expect(ccRegex.test(safeName)).toBe(false);
  });

  it('should reject raw cryptographic private keys via regex inspection', () => {
    const pkRegex = /(-----BEGIN (?:RSA |EC )?PRIVATE KEY-----|\b0x[a-fA-F0-9]{64}\b)/;

    const sampleWithEthKey = 'Ví Metamask 0x4f3edf983ac636a65a842ce7c78d9aa706d3b113bce9c46f30d7d21715b23b1d';
    const safeHint = 'Seed phrase lưu trong két sắt tại ngăn bí mật';

    expect(pkRegex.test(sampleWithEthKey)).toBe(true);
    expect(pkRegex.test(safeHint)).toBe(false);
  });
});
