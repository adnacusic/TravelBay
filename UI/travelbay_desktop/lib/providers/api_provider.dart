import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import '../utils/api_client_exception.dart';
import '../utils/app_config.dart';
import 'auth_provider.dart';

/// HTTP access to one API area (e.g. `Users`, `Reports`). Every call carries the
/// Bearer token; an expired token is refreshed once and the call retried, and API
/// errors are turned into [ApiClientException] with the server's message.
abstract class ApiProvider with ChangeNotifier {
  ApiProvider(this.endpoint);

  final String endpoint;

  /// GET on a sub-path of the resource (e.g. `Users/Stats`), returning the decoded JSON body.
  @protected
  Future<Map<String, dynamic>> getJson(String path) async {
    final response = await send(
      (headers) => http.get(buildUri(path), headers: headers),
    );
    return jsonDecode(response.body) as Map<String, dynamic>;
  }

  /// POST/PUT for actions outside plain CRUD (e.g. `Reviews/5/Approve`); returns the JSON body, if any.
  @protected
  Future<Map<String, dynamic>?> sendAction(
    String method,
    String path, [
    Map<String, dynamic>? body,
  ]) async {
    final response = await send((headers) {
      final uri = buildUri(path);
      final encoded = body == null ? null : jsonEncode(body);
      return method == 'PUT'
          ? http.put(uri, headers: headers, body: encoded)
          : http.post(uri, headers: headers, body: encoded);
    });
    if (response.body.trim().isEmpty) {
      return null;
    }
    final decoded = jsonDecode(response.body);
    return decoded is Map<String, dynamic> ? decoded : null;
  }

  /// Query values that are null or empty are left out, so they do not filter.
  @protected
  Uri buildUri(String path, [Map<String, dynamic>? query]) {
    final parameters = <String, String>{};
    query?.forEach((key, value) {
      if (value != null && value.toString().isNotEmpty) {
        parameters[key] = value.toString();
      }
    });
    return Uri.parse('${AppConfig.baseUrl}$path').replace(
      queryParameters: parameters.isEmpty ? null : parameters,
    );
  }

  @protected
  Future<http.Response> send(
    Future<http.Response> Function(Map<String, String> headers) request,
  ) async {
    try {
      var response = await request(_headers());

      if (response.statusCode == 401 &&
          await AuthProvider.refreshAccessToken()) {
        response = await request(_headers());
      }

      _ensureSuccess(response);
      return response;
    } on http.ClientException {
      throw ApiClientException(AuthProvider.apiUnreachableMessage);
    }
  }

  Map<String, String> _headers() => {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ${AuthProvider.accessToken ?? ''}',
      };

  void _ensureSuccess(http.Response response) {
    final status = response.statusCode;
    if (status >= 200 && status < 300) {
      return;
    }

    if (status == 401) {
      AuthProvider.expireSession();
      throw SessionExpiredException();
    }
    if (status == 403) {
      throw ApiClientException('Nemate ovlasti za ovu akciju.');
    }
    if (status >= 500) {
      throw ApiClientException(
        'Greška na serveru. Pokušajte ponovo za nekoliko trenutaka.',
      );
    }

    throw ApiErrorParser.parse(
      response.body,
      fallback: status == 404
          ? 'Traženi zapis ne postoji ili je obrisan.'
          : 'Zahtjev nije moguće izvršiti.',
    );
  }
}
