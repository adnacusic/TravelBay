import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import '../utils/api_client_exception.dart';
import '../utils/app_config.dart';
import '../utils/jwt_claims.dart';

/// Holds the session tokens. They are static so every provider can attach the
/// Bearer token and refresh it without depending on a BuildContext.
class AuthProvider extends ChangeNotifier {
  static String? _accessToken;
  static String? _refreshToken;
  static JwtClaims? _claims;
  static String? _displayNameOverride;
  static Future<bool>? _pendingRefresh;

  /// Set once in main.dart; called when the session cannot be renewed any more.
  static VoidCallback? onSessionExpired;

  static String? get accessToken => _accessToken;

  bool get isAuthenticated => _accessToken != null;
  String get displayName => _displayNameOverride ?? _claims?.fullName ?? '';

  /// Used to recognise the signed-in admin in lists (e.g. no self-deactivation).
  int? get currentUserId => _claims?.userId;

  /// The JWT keeps the old name until it is renewed; show the saved one right away.
  void updateDisplayName(String firstName, String lastName) {
    _displayNameOverride = '$firstName $lastName'.trim();
    notifyListeners();
  }

  static const _jsonHeaders = {'Content-Type': 'application/json'};

  /// Server messages for login failures, shown in the app language.
  static const _loginMessages = {
    'Invalid username or password.': 'Pogrešno korisničko ime ili lozinka.',
    'User account is deactivated.':
        'Ovaj korisnički račun je deaktiviran. Obratite se administratoru.',
  };

  Future<void> login(String username, String password) async {
    final http.Response response;
    try {
      response = await http.post(
        Uri.parse('${AppConfig.baseUrl}Access/Login'),
        headers: _jsonHeaders,
        body: jsonEncode({'username': username, 'password': password}),
      );
    } on http.ClientException {
      throw ApiClientException(apiUnreachableMessage);
    }

    if (response.statusCode != 200) {
      final error = ApiErrorParser.parse(
        response.body,
        fallback: 'Prijava nije uspjela. Pokušajte ponovo.',
      );
      throw ApiClientException(_loginMessages[error.message] ?? error.message);
    }

    final data = jsonDecode(response.body) as Map<String, dynamic>;
    final accessToken = data['accesstoken'] as String;
    final claims = JwtClaims.fromToken(accessToken);

    if (!claims.isAdmin) {
      throw ApiClientException(
        'Desktop aplikacija je namijenjena samo administratorima. '
        'Ovaj račun koristite u mobilnoj aplikaciji.',
      );
    }

    _accessToken = accessToken;
    _refreshToken = data['refreshtoken'] as String;
    _claims = claims;
    notifyListeners();
  }

  void logout() {
    _clearSession();
    notifyListeners();
  }

  /// Renews the access token with the refresh token. Concurrent callers share
  /// one request, because the API rotates (invalidates) the refresh token on use.
  static Future<bool> refreshAccessToken() {
    return _pendingRefresh ??=
        _requestNewTokens().whenComplete(() => _pendingRefresh = null);
  }

  static Future<bool> _requestNewTokens() async {
    final refreshToken = _refreshToken;
    if (refreshToken == null) {
      return false;
    }

    try {
      final response = await http.post(
        Uri.parse('${AppConfig.baseUrl}Access/LoginWithRefreshToken'),
        headers: _jsonHeaders,
        body: jsonEncode({'refreshToken': refreshToken}),
      );
      if (response.statusCode != 200) {
        return false;
      }

      final data = jsonDecode(response.body) as Map<String, dynamic>;
      _accessToken = data['accesstoken'] as String;
      _refreshToken = data['refreshtoken'] as String;
      _claims = JwtClaims.fromToken(_accessToken!);
      return true;
    } on http.ClientException {
      return false;
    }
  }

  static void expireSession() {
    _clearSession();
    onSessionExpired?.call();
  }

  static void _clearSession() {
    _accessToken = null;
    _refreshToken = null;
    _claims = null;
    _displayNameOverride = null;
  }

  static String get apiUnreachableMessage =>
      'Server nije dostupan (${AppConfig.baseUrl}). '
      'Provjerite da li je API pokrenut (docker compose up -d).';
}
