import 'package:json_annotation/json_annotation.dart';

part 'user_stats.g.dart';

@JsonSerializable()
class UserStats {
  UserStats({
    required this.totalUsers,
    required this.activeUsers,
    required this.inactiveUsers,
    required this.administrators,
    required this.newUsers,
    required this.newUsersPeriodDays,
  });

  final int totalUsers;
  final int activeUsers;
  final int inactiveUsers;
  final int administrators;
  final int newUsers;
  final int newUsersPeriodDays;

  factory UserStats.fromJson(Map<String, dynamic> json) =>
      _$UserStatsFromJson(json);

  Map<String, dynamic> toJson() => _$UserStatsToJson(this);
}

/// What one user has done in the app.
@JsonSerializable()
class UserActivity {
  UserActivity({
    required this.reviewCount,
    required this.pendingReviewCount,
    required this.approvedReviewCount,
    required this.rejectedReviewCount,
    required this.tripPlanCount,
    required this.collectionCount,
    required this.savedDestinationCount,
    required this.viewCount,
  });

  final int reviewCount;
  final int pendingReviewCount;
  final int approvedReviewCount;
  final int rejectedReviewCount;
  final int tripPlanCount;
  final int collectionCount;
  final int savedDestinationCount;
  final int viewCount;

  factory UserActivity.fromJson(Map<String, dynamic> json) =>
      _$UserActivityFromJson(json);

  Map<String, dynamic> toJson() => _$UserActivityToJson(this);
}
