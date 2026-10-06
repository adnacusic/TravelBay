// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_stats.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UserStats _$UserStatsFromJson(Map<String, dynamic> json) => UserStats(
  totalUsers: (json['totalUsers'] as num).toInt(),
  activeUsers: (json['activeUsers'] as num).toInt(),
  inactiveUsers: (json['inactiveUsers'] as num).toInt(),
  administrators: (json['administrators'] as num).toInt(),
  newUsers: (json['newUsers'] as num).toInt(),
  newUsersPeriodDays: (json['newUsersPeriodDays'] as num).toInt(),
);

Map<String, dynamic> _$UserStatsToJson(UserStats instance) => <String, dynamic>{
  'totalUsers': instance.totalUsers,
  'activeUsers': instance.activeUsers,
  'inactiveUsers': instance.inactiveUsers,
  'administrators': instance.administrators,
  'newUsers': instance.newUsers,
  'newUsersPeriodDays': instance.newUsersPeriodDays,
};

UserActivity _$UserActivityFromJson(Map<String, dynamic> json) => UserActivity(
  reviewCount: (json['reviewCount'] as num).toInt(),
  pendingReviewCount: (json['pendingReviewCount'] as num).toInt(),
  approvedReviewCount: (json['approvedReviewCount'] as num).toInt(),
  rejectedReviewCount: (json['rejectedReviewCount'] as num).toInt(),
  tripPlanCount: (json['tripPlanCount'] as num).toInt(),
  collectionCount: (json['collectionCount'] as num).toInt(),
  savedDestinationCount: (json['savedDestinationCount'] as num).toInt(),
  viewCount: (json['viewCount'] as num).toInt(),
);

Map<String, dynamic> _$UserActivityToJson(UserActivity instance) =>
    <String, dynamic>{
      'reviewCount': instance.reviewCount,
      'pendingReviewCount': instance.pendingReviewCount,
      'approvedReviewCount': instance.approvedReviewCount,
      'rejectedReviewCount': instance.rejectedReviewCount,
      'tripPlanCount': instance.tripPlanCount,
      'collectionCount': instance.collectionCount,
      'savedDestinationCount': instance.savedDestinationCount,
      'viewCount': instance.viewCount,
    };
