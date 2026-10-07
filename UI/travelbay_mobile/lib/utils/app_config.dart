/// API address comes only from `--dart-define=baseUrl=...`. The default is the docker-compose API
/// as seen from the Android emulator (10.0.2.2 is the host machine; localhost would be the emulator).
class AppConfig {
  AppConfig._();

  static const String _rawBaseUrl = String.fromEnvironment(
    'baseUrl',
    defaultValue: 'http://10.0.2.2:8080/',
  );

  static String get baseUrl =>
      _rawBaseUrl.endsWith('/') ? _rawBaseUrl : '$_rawBaseUrl/';

  /// Uploaded images are stored as API-relative paths, external ones as absolute URLs.
  static String resolveImageUrl(String imageUrl) {
    if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
      return imageUrl;
    }
    return '$baseUrl$imageUrl';
  }
}
