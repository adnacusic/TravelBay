import '../models/review.dart';
import 'base_provider.dart';

/// Status changes go only through the API state machine (Approve / Reject).
class ReviewProvider extends BaseProvider<Review> {
  ReviewProvider() : super('Reviews');

  @override
  Review fromJson(Map<String, dynamic> json) => Review.fromJson(json);

  Future<Review> approve(int id) async =>
      fromJson((await sendAction('POST', '$endpoint/$id/Approve'))!);

  Future<Review> reject(int id, String reason) async =>
      fromJson((await sendAction('POST', '$endpoint/$id/Reject', {'reason': reason}))!);
}
