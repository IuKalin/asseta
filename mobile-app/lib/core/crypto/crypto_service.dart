import 'dart:convert';
import 'dart:math';
import 'dart:typed_data';
import 'package:cryptography/cryptography.dart';

class EncryptedPayload {
  final String cipherNotesBlob;
  final String cipherNonce;
  final String cipherAuthTag;

  const EncryptedPayload({
    required this.cipherNotesBlob,
    required this.cipherNonce,
    required this.cipherAuthTag,
  });
}

class MobileCryptoService {
  final AesGcm _aesGcm = AesGcm.with256bits();
  final Pbkdf2 _pbkdf2 = Pbkdf2(
    macAlgorithm: Hmac.sha256(),
    iterations: 100000,
    bits: 256,
  );

  Uint8List generateSalt([int length = 16]) {
    final random = Random.secure();
    return Uint8List.fromList(List<int>.generate(length, (_) => random.nextInt(256)));
  }

  Future<SecretKey> deriveMasterKey(String passphrase, List<int> salt) async {
    final secretKey = await _pbkdf2.deriveKey(
      secretKey: SecretKey(utf8.encode(passphrase)),
      nonce: salt,
    );
    return secretKey;
  }

  Future<EncryptedPayload> encrypt(SecretKey key, String plaintext) async {
    final clearTextBytes = utf8.encode(plaintext);
    final secretBox = await _aesGcm.encrypt(
      clearTextBytes,
      secretKey: key,
    );

    return EncryptedPayload(
      cipherNotesBlob: base64Encode(secretBox.cipherText),
      cipherNonce: base64Encode(secretBox.nonce),
      cipherAuthTag: base64Encode(secretBox.mac.bytes),
    );
  }

  Future<String> decrypt({
    required SecretKey key,
    required String cipherNotesBlob,
    required String cipherNonce,
    required String cipherAuthTag,
  }) async {
    final cipherBytes = base64Decode(cipherNotesBlob);
    final nonceBytes = base64Decode(cipherNonce);
    final tagBytes = base64Decode(cipherAuthTag);

    final secretBox = SecretBox(
      cipherBytes,
      nonce: nonceBytes,
      mac: Mac(tagBytes),
    );

    final decryptedBytes = await _aesGcm.decrypt(
      secretBox,
      secretKey: key,
    );

    return utf8.decode(decryptedBytes);
  }
}
