export interface EncryptedPayload {
  cipherNotesBlob: string;
  cipherNonce: string;
  cipherAuthTag: string;
}

export class CryptoService {
  private static readonly PBKDF2_ITERATIONS = 100000;
  private static readonly KEY_LENGTH_BITS = 256;
  private static readonly TAG_LENGTH_BITS = 128; // 16 bytes
  private static readonly NONCE_LENGTH_BYTES = 12; // 96 bits

  /**
   * Generates a random 16-byte salt for key derivation.
   */
  public static generateSalt(): Uint8Array {
    return crypto.getRandomValues(new Uint8Array(16));
  }

  /**
   * Derives a cryptographic CryptoKey (AES-256-GCM) from a passphrase and salt using PBKDF2.
   */
  public static async deriveMasterKey(passphrase: string, salt: Uint8Array): Promise<CryptoKey> {
    const encoder = new TextEncoder();
    const passphraseKey = await crypto.subtle.importKey(
      'raw',
      encoder.encode(passphrase),
      { name: 'PBKDF2' },
      false,
      ['deriveKey']
    );

    return crypto.subtle.deriveKey(
      {
        name: 'PBKDF2',
        salt: salt as unknown as BufferSource,
        iterations: this.PBKDF2_ITERATIONS,
        hash: 'SHA-256'
      },
      passphraseKey,
      { name: 'AES-GCM', length: this.KEY_LENGTH_BITS },
      false,
      ['encrypt', 'decrypt']
    );
  }

  /**
   * Encrypts plaintext string using AES-256-GCM and returns separated Base64 blob, nonce, and authTag.
   */
  public static async encrypt(key: CryptoKey, plaintext: string): Promise<EncryptedPayload> {
    const nonce = crypto.getRandomValues(new Uint8Array(this.NONCE_LENGTH_BYTES));
    const encoder = new TextEncoder();
    const encodedData = encoder.encode(plaintext);

    // Web Crypto produces [ciphertext + 16-byte tag]
    const encryptedBuffer = await crypto.subtle.encrypt(
      {
        name: 'AES-GCM',
        iv: nonce,
        tagLength: this.TAG_LENGTH_BITS
      },
      key,
      encodedData
    );

    const fullCipherBytes = new Uint8Array(encryptedBuffer);
    const tagByteLength = this.TAG_LENGTH_BITS / 8;
    const cipherLength = fullCipherBytes.length - tagByteLength;

    const cipherBytes = fullCipherBytes.subarray(0, cipherLength);
    const tagBytes = fullCipherBytes.subarray(cipherLength);

    return {
      cipherNotesBlob: this.bytesToBase64(cipherBytes),
      cipherNonce: this.bytesToBase64(nonce),
      cipherAuthTag: this.bytesToBase64(tagBytes)
    };
  }

  /**
   * Decrypts Base64 payload back to UTF-8 plaintext using AES-256-GCM.
   */
  public static async decrypt(
    key: CryptoKey,
    cipherBlobBase64: string,
    nonceBase64: string,
    authTagBase64: string
  ): Promise<string> {
    const cipherBytes = this.base64ToBytes(cipherBlobBase64);
    const nonceBytes = this.base64ToBytes(nonceBase64);
    const tagBytes = this.base64ToBytes(authTagBase64);

    // Combine cipherBytes and tagBytes for Web Crypto API
    const combinedBytes = new Uint8Array(cipherBytes.length + tagBytes.length);
    combinedBytes.set(cipherBytes, 0);
    combinedBytes.set(tagBytes, cipherBytes.length);

    const decryptedBuffer = await crypto.subtle.decrypt(
      {
        name: 'AES-GCM',
        iv: nonceBytes as unknown as BufferSource,
        tagLength: this.TAG_LENGTH_BITS
      },
      key,
      combinedBytes as unknown as BufferSource
    );

    const decoder = new TextDecoder();
    return decoder.decode(decryptedBuffer);
  }

  /**
   * Helper to decrypt an EncryptedPayload object directly.
   */
  public static async decryptPayload(key: CryptoKey, payload: EncryptedPayload): Promise<string> {
    return this.decrypt(key, payload.cipherNotesBlob, payload.cipherNonce, payload.cipherAuthTag);
  }

  public static bytesToBase64(bytes: Uint8Array): string {
    let binary = '';
    const len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
      binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary);
  }

  public static base64ToBytes(base64: string): Uint8Array {
    const binaryString = atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
  }

  public static hexToBytes(hex: string): Uint8Array {
    const cleanHex = hex.trim();
    const matches = cleanHex.match(/.{1,2}/g) || [];
    return new Uint8Array(matches.map((byte) => parseInt(byte, 16)));
  }

  public static bytesToHex(bytes: Uint8Array): string {
    return Array.from(bytes)
      .map((b) => b.toString(16).padStart(2, '0'))
      .join('');
  }

  public static async computeMasterKeyVerifier(masterKey: string): Promise<string> {
    const encoder = new TextEncoder();
    const data = encoder.encode(masterKey.trim().toUpperCase());
    const hashBuffer = await crypto.subtle.digest('SHA-256', data);
    return this.bytesToHex(new Uint8Array(hashBuffer)).toLowerCase();
  }
}
