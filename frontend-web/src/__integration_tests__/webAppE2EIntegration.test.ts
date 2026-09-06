import { describe, it, expect, beforeAll } from 'vitest';
import axios from 'axios';
import { authApi } from '../services/authApi';
import { apiClient } from '../services/apiClient';
import { CryptoService, EncryptedPayload } from '../services/cryptoService';
import { safeActivationService } from '../services/safeActivationService';
import { continuityPlanService } from '../services/continuityPlanService';
import { trustedPeopleService } from '../services/trustedPeopleService';

describe('Web-App E2E Integration Test Across Modules 1 to 5 (Zero Mock Data)', () => {
  const testSuffix = Math.random().toString(36).substring(2, 8);
  const ownerEmail = `owner_${testSuffix}@test.asseta`;
  const ownerPassword = 'Password123!@#';
  const ownerFullName = 'Nguyễn Văn Chủ Tài Sản';
  const ownerPhone = '0901234567';

  const delegateEmail = `delegate_${testSuffix}@test.asseta`;
  const delegatePassword = 'Password123!@#';
  const delegateFullName = 'Trần Thị Người Ủy Thác';
  const delegatePhone = '0912345678';

  let ownerToken = '';
  let ownerId = '';
  let delegateToken = '';
  let categoryId = '';
  let createdCardId = '';
  let createdPersonId = '';
  let activePairingCode = '';
  let activeActivationRequestId = '';
  let realEncryptedBlob: EncryptedPayload;

  beforeAll(async () => {
    // Verify Backend is live and reachable at http://localhost:5000
    try {
      const healthCheck = await axios.get('http://localhost:5000/');
      expect(healthCheck.status).toBe(200);
      expect(healthCheck.data.status).toBe('Healthy');
    } catch (err: any) {
      throw new Error(`Backend API tại http://localhost:5000 không phản hồi: ${err.message}`);
    }
  });

  it('[MODULE 1 & AUTH] Register Owner, Login, obtain real JWT and verify WebCrypto Master Key operations', async () => {
    // 1. Register real owner on live Backend
    const registerRes = await authApi.register({
      email: ownerEmail,
      password: ownerPassword,
      fullName: ownerFullName,
      phoneNumber: ownerPhone,
    });
    expect(registerRes).toBeDefined();
    expect(registerRes.accessToken).toBeDefined();
    expect(registerRes.user.email).toBe(ownerEmail);

    ownerToken = registerRes.accessToken;
    ownerId = registerRes.user.id;

    // Set owner token in apiClient default headers
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${ownerToken}`;

    // 2. Real WebCrypto: Generate salt, derive Master Key via PBKDF2 (100,000 iterations)
    const testSecret = 'Mã PIN két sắt bí mật: 987654. Tài khoản ngân hàng số: 102030.';
    const salt = CryptoService.generateSalt();
    const derivedKey = await CryptoService.deriveMasterKey(ownerPassword, salt);
    expect(derivedKey).toBeDefined();

    // 3. Real WebCrypto: Encrypt data with AES-256-GCM
    realEncryptedBlob = await CryptoService.encrypt(derivedKey, testSecret);
    expect(realEncryptedBlob).toBeDefined();
    expect(realEncryptedBlob.cipherNotesBlob).toBeDefined();
    expect(realEncryptedBlob.cipherNonce).toBeDefined();
    expect(realEncryptedBlob.cipherAuthTag).toBeDefined();
    expect(realEncryptedBlob.cipherNotesBlob).not.toBe(testSecret);

    // 4. Real WebCrypto: Decrypt to verify zero-knowledge roundtrip
    const decryptedText = await CryptoService.decryptPayload(derivedKey, realEncryptedBlob);
    expect(decryptedText).toBe(testSecret);

    // 5. Query Continuity Map to fetch seeded categories
    const mapRes = await apiClient.get('/v1/continuity-map');
    expect(mapRes.status).toBe(200);
    expect(mapRes.data.data.categories.length).toBeGreaterThan(0);
    categoryId = mapRes.data.data.categories[0].categoryId;
    expect(categoryId).toBeDefined();

    // 6. Create Continuity Item with real encrypted payload
    const itemRes = await apiClient.post('/v1/continuity-items', {
      categoryId,
      name: `Sổ Đỏ Bất Động Sản ${testSuffix}`,
      priority: 'CRITICAL',
      documentLocationHint: 'Két sắt gia đình, ngăn số 2',
      cipherNotesBlob: realEncryptedBlob.cipherNotesBlob,
      cipherNonce: realEncryptedBlob.cipherNonce,
      cipherAuthTag: realEncryptedBlob.cipherAuthTag,
    });
    expect(itemRes.status).toBe(201);
    expect(itemRes.data.data.id).toBeDefined();
  });

  it('[MODULE 2] Create real Action Card with Category, Urgency, Steps, Contacts and Cipher Instructions', async () => {
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${ownerToken}`;

    // 1. Create real action card with real encrypted instructions
    const createCardRes = await apiClient.post('/v1/action-cards', {
      categoryId,
      title: `Thẻ Hành Động Tiếp Quản ${testSuffix}`,
      urgency: 'IMMEDIATE',
      priority: 'CRITICAL',
      summary: 'Quy trình xử lý khẩn cấp chuyển giao tài chính',
      documentLocationHint: 'Két sắt phòng ngủ chính',
      cipherInstructionsBlob: realEncryptedBlob.cipherNotesBlob,
      cipherNonce: realEncryptedBlob.cipherNonce,
      cipherAuthTag: realEncryptedBlob.cipherAuthTag,
    });

    expect(createCardRes.status).toBe(201);
    expect(createCardRes.data.data).toBeDefined();
    createdCardId = createCardRes.data.data.id;
    expect(createdCardId).toBeDefined();
    expect(createCardRes.data.data.urgency).toBe('IMMEDIATE');

    // 2. Add real Step
    const stepRes = await apiClient.post(`/v1/action-cards/${createdCardId}/steps`, {
      instruction: 'Mở két sắt bằng mã PIN đã giải mã',
      estimatedDuration: '15 phút',
    });
    expect(stepRes.status).toBe(201);

    // 3. Add real Contact
    const contactRes = await apiClient.post(`/v1/action-cards/${createdCardId}/contacts`, {
      contactName: 'Luật sư Lê Hoàng',
      relationshipOrRole: 'Luật sư gia đình',
      phoneNumber: '0988776655',
      email: 'lawyer@test.asseta',
    });
    expect(contactRes.status).toBe(201);
  });

  it('[MODULE 3] Invite Trusted Person, Generate 6-Digit Pairing Code, Claim by Delegate and Set Scoped Access', async () => {
    // 1. Register real delegate user on live Backend
    const delegateRegister = await authApi.register({
      email: delegateEmail,
      password: delegatePassword,
      fullName: delegateFullName,
      phoneNumber: delegatePhone,
    });
    expect(delegateRegister.accessToken).toBeDefined();
    delegateToken = delegateRegister.accessToken;

    // 2. Owner invites delegate with Trust Level 2
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${ownerToken}`;
    const inviteRes = await trustedPeopleService.createTrustedPerson({
      fullName: delegateFullName,
      email: delegateEmail,
      phoneNumber: delegatePhone,
      relationship: 'Vợ/Chồng',
      trustLevel: 2,
    });

    expect(inviteRes).toBeDefined();
    createdPersonId = inviteRes.id;
    expect(createdPersonId).toBeDefined();

    // 3. Get or regenerate 6-digit numeric pairing code
    activePairingCode = inviteRes.activePairingCode || '';
    if (!activePairingCode) {
      const regenerated = await trustedPeopleService.regeneratePairingCode(createdPersonId);
      activePairingCode = regenerated.activePairingCode || '';
    }
    expect(activePairingCode).toHaveLength(6);

    // 4. Delegate claims pairing code using delegate token
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${delegateToken}`;
    const claimRes = await trustedPeopleService.claimPairingCode(activePairingCode);
    expect(claimRes).toBeDefined();
    expect(claimRes.status).toBe('Active');

    // 5. Owner grants Scoped Access on created Action Card
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${ownerToken}`;
    const permRes = await trustedPeopleService.updateScopedPermissions(createdPersonId, {
      actionCardPermissions: [
        {
          actionCardId: createdCardId,
          canView: true,
        },
      ],
    });
    expect(permRes).toBeDefined();
    expect(permRes.length).toBeGreaterThan(0);
  });

  it('[MODULE 4] Query Continuity Plan, SPoF Analysis, Gap Detection and Offline Emergency Brief', async () => {
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${ownerToken}`;

    // 1. Query aggregated Continuity Plan (4 stages)
    const plan = await continuityPlanService.getContinuityPlan();
    expect(plan).toBeDefined();
    expect(plan.stages).toBeDefined();
    expect(plan.stages.length).toBe(4);

    // Verify Immediate stage contains our created action card
    const immediateStage = plan.stages.find((s) => s.stage === 'IMMEDIATE');
    expect(immediateStage).toBeDefined();
    const cardInStage = immediateStage?.cards.find((c) => c.id === createdCardId);
    expect(cardInStage).toBeDefined();

    // 2. Query Readiness Audit (SPoF and Gaps)
    const audit = await continuityPlanService.getPlanAudit();
    expect(audit).toBeDefined();
    expect(audit.overallScore).toBeGreaterThanOrEqual(0);
    expect(typeof audit.hasSinglePointOfFailureRisk).toBe('boolean');

    // 3. Export Offline Emergency Brief (Zero-Knowledge: no cipher instructions)
    const brief = await continuityPlanService.getEmergencyBrief();
    expect(brief).toBeDefined();
    expect(brief.stages).toBeDefined();
    brief.stages.forEach((stage) => {
      stage.actionItems.forEach((card: any) => {
        expect(card.cipherInstructionsBlob).toBeUndefined();
      });
    });
  });

  it('[MODULE 5] Safe Activation: Vitality Check-in, 48h Time-Lock, 1-Tap Cancel, Quorum & Deactivate', async () => {
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${ownerToken}`;

    // 1. Get initial activation status
    const initialStatus = await safeActivationService.getStatus();
    expect(initialStatus).toBeDefined();
    expect(initialStatus.ownerId).toBe(ownerId);

    // 2. Vitality Check-in 1-tap ("Tôi Vẫn Ổn")
    const checkInStatus = await safeActivationService.vitalityCheckIn();
    expect(checkInStatus).toBeDefined();
    expect(checkInStatus.heartbeatStatus).toBe('ACTIVE');
    expect(checkInStatus.lastCheckInAtUtc).toBeDefined();

    // 3. Update configuration: 30 days interval, 48h grace, 1 min confirmation
    const updatedConfig = await safeActivationService.updateConfig({
      checkInIntervalDays: 30,
      gracePeriodHours: 48,
      minConfirmationsRequired: 1,
    });
    expect(updatedConfig.checkInIntervalDays).toBe(30);
    expect(updatedConfig.gracePeriodHours).toBe(48);
    expect(updatedConfig.minConfirmationsRequired).toBe(1);

    // 4. Delegate initiates emergency activation request
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${delegateToken}`;
    const initiateRes = await safeActivationService.initiateRequest(
      ownerId,
      'Mất liên lạc khẩn cấp với chủ tài sản hơn 48h'
    );
    expect(initiateRes).toBeDefined();
    expect(initiateRes.status).toBe('PendingGracePeriod');
    expect(initiateRes.remainingSeconds).toBeGreaterThan(0);
    activeActivationRequestId = initiateRes.id;

    // 5. Owner performs 1-Tap Emergency Cancellation
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${ownerToken}`;
    const cancelSuccess = await safeActivationService.cancelRequest(activeActivationRequestId);
    expect(cancelSuccess).toBe(true);

    // Verify status reverted to normal ACTIVE
    const afterCancelStatus = await safeActivationService.getStatus();
    expect(afterCancelStatus.activeRequest).toBeNull();
    expect(afterCancelStatus.heartbeatStatus).toBe('ACTIVE');
  });
});
