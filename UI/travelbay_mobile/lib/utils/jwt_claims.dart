import 'dart:convert';

/// Claim names issued by the API (TravelBay.Model.Constants.ClaimNames).
class ClaimNames {
  ClaimNames._();

  static const id = 'Id';
  static const firstName = 'FirstName';
  static const lastName = 'LastName';
}

/// Reads the JWT payload. The signature is verified by the API on every call;
/// the client only needs the claims for display.
class JwtClaims {
  JwtClaims._(this._payload);

  final Map<String, dynamic> _payload;

  factory JwtClaims.fromToken(String token) {
    final parts = token.split('.');
    if (parts.length != 3) {
      throw const FormatException('Neispravan pristupni token.');
    }
    final normalized = base64Url.normalize(parts[1]);
    final payload = jsonDecode(utf8.decode(base64Url.decode(normalized)));
    return JwtClaims._(Map<String, dynamic>.from(payload as Map));
  }

  int? get userId => int.tryParse(_payload[ClaimNames.id]?.toString() ?? '');

  String get firstName => _payload[ClaimNames.firstName]?.toString() ?? '';
  String get lastName => _payload[ClaimNames.lastName]?.toString() ?? '';
  String get fullName => '$firstName $lastName'.trim();
}
