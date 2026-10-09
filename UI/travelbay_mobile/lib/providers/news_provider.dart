import '../models/news.dart';
import '../models/search_result.dart';
import 'base_provider.dart';

/// News written by the admin on the desktop; the API returns the newest publication first.
class NewsProvider extends BaseProvider<News> {
  NewsProvider() : super('News');

  @override
  News fromJson(Map<String, dynamic> json) => News.fromJson(json);

  Future<SearchResult<News>> latest({int page = 1, int pageSize = 10}) =>
      get(filter: {'page': page, 'pageSize': pageSize, 'includeTotalCount': true});
}
