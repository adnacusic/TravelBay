import 'package:json_annotation/json_annotation.dart';

import 'enums.dart';
import 'utc_date_time_converter.dart';

part 'review.g.dart';

@JsonSerializable()
@UtcDateTimeConverter()
class Review {
  Review({
    required this.id,
    required this.destinationId,
    required this.userId,
    this.reviewerDisplayName = '',
    required this.rating,
    this.comment,
    required this.status,
    required this.createdAt,
    this.moderatedByUserId,
    this.moderatedAt,
    this.moderationReason,
  });

  final int id;
  final int destinationId;
  final int userId;
  final String reviewerDisplayName;
  final int rating;
  final String? comment;
  final ReviewStatus status;
  final DateTime createdAt;
  final int? moderatedByUserId;
  final DateTime? moderatedAt;
  final String? moderationReason;

  factory Review.fromJson(Map<String, dynamic> json) => _$ReviewFromJson(json);

  Map<String, dynamic> toJson() => _$ReviewToJson(this);
}
