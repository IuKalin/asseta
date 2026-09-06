import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/core/network/idempotency_interceptor.dart';

void main() {
  group('IdempotencyInterceptor', () {
    late IdempotencyInterceptor interceptor;

    setUp(() {
      interceptor = IdempotencyInterceptor();
    });

    test('adds X-Correlation-Id and Idempotency-Key to POST request', () {
      final options = RequestOptions(method: 'POST', path: '/continuity-items');
      final handler = RequestInterceptorHandler();

      interceptor.onRequest(options, handler);

      expect(options.headers['X-Correlation-Id'], isNotNull);
      expect(options.headers['Idempotency-Key'], isNotNull);
      expect(options.headers['Idempotency-Key'].toString().length, equals(36)); // UUIDv4 length
    });

    test('adds X-Correlation-Id but NOT Idempotency-Key to GET request', () {
      final options = RequestOptions(method: 'GET', path: '/continuity-map');
      final handler = RequestInterceptorHandler();

      interceptor.onRequest(options, handler);

      expect(options.headers['X-Correlation-Id'], isNotNull);
      expect(options.headers.containsKey('Idempotency-Key'), isFalse);
    });

    test('preserves existing Idempotency-Key if already supplied', () {
      const existingKey = 'custom-idempotency-key-1234';
      final options = RequestOptions(
        method: 'PUT',
        path: '/continuity-items/1',
        headers: {'Idempotency-Key': existingKey},
      );
      final handler = RequestInterceptorHandler();

      interceptor.onRequest(options, handler);

      expect(options.headers['Idempotency-Key'], equals(existingKey));
    });
  });
}
