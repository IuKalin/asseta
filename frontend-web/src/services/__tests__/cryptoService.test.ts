import { describe, it, expect } from 'vitest';
import { CryptoService } from '../cryptoService';

describe('CryptoService (Web Crypto AES-256-GCM & PBKDF2)', () => {
  it('should generate a 16-byte random salt', () => {
    const salt = CryptoService.generateSalt();
    expect(salt).toBeInstanceOf(Uint8Array);
    expect(salt.length).toBe(16);
  });

  it('should derive a valid AES-GCM 256-bit CryptoKey from passphrase and salt', async () => {
    const passphrase = 'MySuperSecretPassphrase@2026';
    const salt = CryptoService.generateSalt();

    const key = await CryptoService.deriveMasterKey(passphrase, salt);
    expect(key).toBeDefined();
    expect(key.algorithm.name).toBe('AES-GCM');
    expect((key.algorithm as any).length).toBe(256);
  });

  it('should encrypt and decrypt plaintext with UTF-8 accents correctly', async () => {
    const passphrase = 'PersonalPassphrase123!';
    const salt = CryptoService.generateSalt();
    const key = await CryptoService.deriveMasterKey(passphrase, salt);

    const secretText = 'Hồ sơ thế chấp gửi anh Hoàng kế toán giữ bản gốc tại ngăn kéo tầng 2';

    // 1. Encrypt
    const encrypted = await CryptoService.encrypt(key, secretText);

    expect(encrypted.cipherNotesBlob).toBeDefined();
    expect(encrypted.cipherNonce).toBeDefined();
    expect(encrypted.cipherAuthTag).toBeDefined();

    // Verify ciphertext does not contain plaintext
    expect(encrypted.cipherNotesBlob).not.toContain(secretText);

    // 2. Decrypt
    const decrypted = await CryptoService.decrypt(
      key,
      encrypted.cipherNotesBlob,
      encrypted.cipherNonce,
      encrypted.cipherAuthTag
    );

    expect(decrypted).toBe(secretText);
  });

  it('should fail decryption if an incorrect key is provided', async () => {
    const salt = CryptoService.generateSalt();
    const correctKey = await CryptoService.deriveMasterKey('CorrectPassword123', salt);
    const wrongKey = await CryptoService.deriveMasterKey('WrongPassword456', salt);

    const secretText = 'Thông tin tuyệt mật cần bảo vệ';
    const encrypted = await CryptoService.encrypt(correctKey, secretText);

    await expect(
      CryptoService.decrypt(
        wrongKey,
        encrypted.cipherNotesBlob,
        encrypted.cipherNonce,
        encrypted.cipherAuthTag
      )
    ).rejects.toThrow();
  });

  it('should fail decryption if auth tag is tampered with', async () => {
    const salt = CryptoService.generateSalt();
    const key = await CryptoService.deriveMasterKey('Passphrase123', salt);

    const secretText = 'Dữ liệu không được phép can thiệp';
    const encrypted = await CryptoService.encrypt(key, secretText);

    // Tamper with auth tag
    const tamperedTag = btoa('invalid_tag_bytes_123');

    await expect(
      CryptoService.decrypt(
        key,
        encrypted.cipherNotesBlob,
        encrypted.cipherNonce,
        tamperedTag
      )
    ).rejects.toThrow();
  });
});
