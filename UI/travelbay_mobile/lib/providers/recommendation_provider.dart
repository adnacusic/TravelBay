import '../models/recommendation.dart';
import '../models/search_result.dart';
import 'api_provider.dart';

/// Personal recommendations (content-based recommender on the API).
class RecommendationProvider extends ApiProvider {
  RecommendationProvider() : super('Recommendations');

  Future<SearchResult<Recommendation>> get({int page = 1, int pageSize = 10}) async {
    final data = await getJson(
      endpoint,
      {'page': page, 'pageSize': pageSize, 'includeTotalCount': true},
    );
    return SearchResult(
      items: (data['items'] as List)
          .map((e) => Recommendation.fromJson(e as Map<String, dynamic>))
          .toList(),
      totalCount: data['totalCount'] as int?,
    );
  }
}
