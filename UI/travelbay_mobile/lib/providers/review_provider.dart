import '../models/enums.dart';
import '../models/review.dart';
import '../models/search_result.dart';
import 'base_provider.dart';

/// Reviews: the public list shows only approved ones; a user also sees their own
/// (pending or rejected). New reviews start as pending until an admin moderates them.
class ReviewProvider extends BaseProvider<Review> {
  ReviewProvider() : super('Reviews');

  @override
  Review fromJson(Map<String, dynamic> json) => Review.fromJson(json);

  Future<SearchResult<Review>> approvedFor(int destinationId, {int page = 1, int pageSize = 5}) =>
      get(filter: {
        'destinationId': destinationId,
        'status': ReviewStatus.approved.index,
        'includeTotalCount': true,
        'page': page,
        'pageSize': pageSize,
      });

  /// The signed-in user's reviews of one destination, newest first.
  Future<List<Review>> mineFor(int destinationId) async {
    final result = await get(filter: {
      'destinationId': destinationId,
      'onlyMine': true,
      'pageSize': 10,
    });
    return result.items;
  }
}
