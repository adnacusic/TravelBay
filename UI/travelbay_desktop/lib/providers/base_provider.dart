import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import '../models/search_result.dart';
import '../utils/api_client_exception.dart';
import '../utils/app_config.dart';
import 'auth_provider.dart';

/// Generic REST access for one API resource: paged list, get by id, insert, update, delete.
/// Every call carries the Bearer token; an expired token is refreshed once and the call retried.
abstract class BaseProvider<T> with ChangeNotifier {
  BaseProvider(this.endpoint);

  final String endpoint;

  T fromJson(Map<String, dynamic> json);

  Future<SearchResult<T>> get({Map<String, dynamic>? filter}) async {
    final response = await send(
      (headers) => http.get(buildUri(endpoint, filter), headers: headers),
    );
    final data = jsonDecode(response.body) as Map<String, dynamic>;
    final items = (data['items'] as List)
        .map((e) => fromJson(e as Map<String, dynamic>))
        .toList();
    return SearchResult(items: items, totalCount: data['totalCount'] as int?);
  }

  Future<T> getById(int id) async {
    final response = await send(
      (headers) => http.get(buildUri('$endpoint/$id'), headers: headers),
    );
    return fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }

  Future<T> insert(Map<String, dynamic> request) async {
    final response = await send(
      (headers) => http.post(
        buildUri(endpoint),
        headers: headers,
        body: jsonEncode(request),
      ),
    );
    return fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }

  Future<T> update(int id, Map<String, dynamic> request) async {
    final response = await send(
      (headers) => http.put(
        buildUri('$endpoint/$id'),
        headers: headers,
        body: jsonEncode(request),
      ),
    );
    return fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }

  Future<void> remove(int id) async {
    await send(
      (headers) => http.delete(buildUri('$endpoint/$id'), headers: headers),
    );
  }

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
