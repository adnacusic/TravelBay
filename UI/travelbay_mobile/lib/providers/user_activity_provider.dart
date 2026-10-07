import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/search_result.dart';
import '../models/user_activity.dart';
import 'api_provider.dart';

/// Saved destinations of the signed-in user.
class SavedDestinationProvider extends ApiProvider {
  SavedDestinationProvider() : super('SavedDestinations');

  Future<SavedDestination> save(int destinationId) async => SavedDestination.fromJson(
        (await sendAction('POST', endpoint, {'destinationId': destinationId}))!,
      );

  Future<void> remove(int savedDestinationId) async {
    await send(
      (headers) => http.delete(buildUri('$endpoint/$savedDestinationId'), headers: headers),
    );
  }
}

/// Preferred categories of the signed-in user; the recommender reads them.
class UserPreferenceProvider extends ApiProvider {
  UserPreferenceProvider() : super('UserPreferences');

  Future<List<UserPreference>> getAll() async {
    final response = await send((headers) => http.get(buildUri(endpoint), headers: headers));
    return (jsonDecode(response.body) as List)
        .map((e) => UserPreference.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Replaces the whole set of preferred categories.
  Future<void> setCategories(List<int> categoryIds) =>
      sendAction('PUT', endpoint, {'categoryIds': categoryIds});
}

/// Destination views of the signed-in user; every opened detail is recorded.
class ViewHistoryProvider extends ApiProvider {
  ViewHistoryProvider() : super('ViewHistories');

  Future<void> record(int destinationId) =>
      sendAction('POST', endpoint, {'destinationId': destinationId});

  Future<SearchResult<ViewHistory>> get({int page = 1, int pageSize = 20}) async {
    final data = await getJson(
      endpoint,
      {'page': page, 'pageSize': pageSize, 'includeTotalCount': true},
    );
    return SearchResult(
      items: (data['items'] as List)
          .map((e) => ViewHistory.fromJson(e as Map<String, dynamic>))
          .toList(),
      totalCount: data['totalCount'] as int?,
    );
  }
}
