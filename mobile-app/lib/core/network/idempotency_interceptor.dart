import 'package:dio/dio.dart';
import 'package:uuid/uuid.dart';

class IdempotencyInterceptor extends Interceptor {
  final Uuid _uuid = const Uuid();

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    if (!options.headers.containsKey('X-Correlation-Id')) {
      options.headers['X-Correlation-Id'] = _uuid.v4();
    }

    final method = options.method.toUpperCase();
    if (method == 'POST' || method == 'PUT' || method == 'DELETE') {
      if (!options.headers.containsKey('Idempotency-Key')) {
        options.headers['Idempotency-Key'] = _uuid.v4();
      }
    }

    super.onRequest(options, handler);
  }
}
