import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/search_result.dart';
import 'api_provider.dart';

/// Generic REST access for one API resource: paged list, get by id, insert, update, delete.
abstract class BaseProvider<T> extends ApiProvider {
  BaseProvider(super.endpoint);

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
}
