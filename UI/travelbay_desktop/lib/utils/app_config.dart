/// API address comes only from `--dart-define=baseUrl=...`; the default is the local docker-compose API.
class AppConfig {
  AppConfig._();

  static const String _rawBaseUrl = String.fromEnvironment(
    'baseUrl',
    defaultValue: 'http://localhost:8080/',
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
