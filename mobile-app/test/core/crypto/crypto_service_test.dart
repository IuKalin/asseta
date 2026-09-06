import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/core/crypto/crypto_service.dart';

void main() {
  group('MobileCryptoService (Dart PBKDF2 & AES-256-GCM)', () {
    late MobileCryptoService cryptoService;

    setUp(() {
      cryptoService = MobileCryptoService();
    });

    test('generateSalt returns 16 random bytes', () {
      final salt = cryptoService.generateSalt();
      expect(salt.length, equals(16));
    });

    test('deriveMasterKey, encrypt and decrypt with UTF-8 Vietnamese characters', () async {
      const passphrase = 'PersonalSafePassphrase#2026';
      final salt = cryptoService.generateSalt();

      final key = await cryptoService.deriveMasterKey(passphrase, salt);

      const secretText = 'Sổ đỏ nhà đất tại Quận 2 gửi anh Tuấn luật sư giữ bản gốc.';

      // 1. Encrypt
      final encrypted = await cryptoService.encrypt(key, secretText);

      expect(encrypted.cipherNotesBlob, isNotEmpty);
      expect(encrypted.cipherNonce, isNotEmpty);
      expect(encrypted.cipherAuthTag, isNotEmpty);

      // Zero-knowledge check
      expect(encrypted.cipherNotesBlob.contains(secretText), isFalse);

      // 2. Decrypt
      final decrypted = await cryptoService.decrypt(
        key: key,
        cipherNotesBlob: encrypted.cipherNotesBlob,
        cipherNonce: encrypted.cipherNonce,
        cipherAuthTag: encrypted.cipherAuthTag,
      );

      expect(decrypted, equals(secretText));
    });

    test('decrypt with incorrect key fails', () async {
      final salt = cryptoService.generateSalt();
      final key1 = await cryptoService.deriveMasterKey('CorrectPassword123', salt);
      final key2 = await cryptoService.deriveMasterKey('WrongPassword456', salt);

      const secretText = 'Nội dung tuyệt mật không được rò rỉ';
      final encrypted = await cryptoService.encrypt(key1, secretText);

      expect(
        () async => await cryptoService.decrypt(
          key: key2,
          cipherNotesBlob: encrypted.cipherNotesBlob,
          cipherNonce: encrypted.cipherNonce,
          cipherAuthTag: encrypted.cipherAuthTag,
        ),
        throwsA(isA<Exception>()),
      );
    });
  });
}
