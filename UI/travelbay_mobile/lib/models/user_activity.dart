import 'package:json_annotation/json_annotation.dart';

import 'utc_date_time_converter.dart';

part 'user_activity.g.dart';

/// A destination the user saved ("Sačuvano").
@JsonSerializable()
@UtcDateTimeConverter()
class SavedDestination {
  SavedDestination({
    required this.id,
    required this.destinationId,
    this.destinationName = '',
    this.cityName = '',
    this.imageUrl,
    required this.savedAt,
  });

  final int id;
  final int destinationId;
  final String destinationName;
  final String cityName;

  /// Cover image of the destination; null when it has none.
  final String? imageUrl;
  final DateTime savedAt;

  factory SavedDestination.fromJson(Map<String, dynamic> json) =>
      _$SavedDestinationFromJson(json);

  Map<String, dynamic> toJson() => _$SavedDestinationToJson(this);
}

/// A category the user prefers; it feeds the recommender.
@JsonSerializable()
class UserPreference {
  UserPreference({required this.categoryId, this.categoryName = ''});

  final int categoryId;
  final String categoryName;

  factory UserPreference.fromJson(Map<String, dynamic> json) =>
      _$UserPreferenceFromJson(json);

  Map<String, dynamic> toJson() => _$UserPreferenceToJson(this);
}

/// One opening of a destination's details; it feeds the recommender.
@JsonSerializable()
@UtcDateTimeConverter()
class ViewHistory {
  ViewHistory({
    required this.id,
    required this.destinationId,
    this.destinationName = '',
    this.cityName = '',
    this.imageUrl,
    required this.viewedAt,
  });

  final int id;
  final int destinationId;
  final String destinationName;
  final String cityName;

  /// Cover image of the destination; null when it has none.
  final String? imageUrl;
  final DateTime viewedAt;

  factory ViewHistory.fromJson(Map<String, dynamic> json) =>
      _$ViewHistoryFromJson(json);

  Map<String, dynamic> toJson() => _$ViewHistoryToJson(this);
}

/// Counters shown on the profile (Users/Me/Activity).
@JsonSerializable()
class UserActivity {
  UserActivity({
    this.reviewCount = 0,
    this.pendingReviewCount = 0,
    this.approvedReviewCount = 0,
    this.rejectedReviewCount = 0,
    this.tripPlanCount = 0,
    this.completedTripPlanCount = 0,
    this.collectionCount = 0,
    this.savedDestinationCount = 0,
    this.viewCount = 0,
  });

  final int reviewCount;
  final int pendingReviewCount;
  final int approvedReviewCount;
  final int rejectedReviewCount;
  final int tripPlanCount;
  final int completedTripPlanCount;
  final int collectionCount;
  final int savedDestinationCount;
  final int viewCount;

  factory UserActivity.fromJson(Map<String, dynamic> json) =>
      _$UserActivityFromJson(json);

  Map<String, dynamic> toJson() => _$UserActivityToJson(this);
}
