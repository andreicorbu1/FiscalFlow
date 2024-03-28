import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class Storage {
  static final _storage = new FlutterSecureStorage();

  static Future<void> storeToken(String token) async {
    await _storage.write(key: 'token', value: token);
  }

  static Future<String?> getToken() async {
    return await _storage.read(key: 'token');
  }

  static Future<void> storeRefreshToken(String token) async {
    await _storage.write(key: 'refresh-token', value: token);
  }

  static Future<String?> getRefreshToken() async {
    return await _storage.read(key: 'refresh-token');
  }
}
