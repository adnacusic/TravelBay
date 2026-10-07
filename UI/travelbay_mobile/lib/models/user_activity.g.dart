// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_activity.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SavedDestination _$SavedDestinationFromJson(Map<String, dynamic> json) =>
    SavedDestination(
      id: (json['id'] as num).toInt(),
      destinationId: (json['destinationId'] as num).toInt(),
      destinationName: json['destinationName'] as String? ?? '',
      cityName: json['cityName'] as String? ?? '',
      imageUrl: json['imageUrl'] as String?,
      savedAt: const UtcDateTimeConverter().fromJson(json['savedAt'] as String),
    );

Map<String, dynamic> _$SavedDestinationToJson(SavedDestination instance) =>
    <String, dynamic>{
      'id': instance.id,
      'destinationId': instance.destinationId,
      'destinationName': instance.destinationName,
      'cityName': instance.cityName,
      'imageUrl': instance.imageUrl,
      'savedAt': const UtcDateTimeConverter().toJson(instance.savedAt),
    };

UserPreference _$UserPreferenceFromJson(Map<String, dynamic> json) =>
    UserPreference(
      categoryId: (json['categoryId'] as num).toInt(),
      categoryName: json['categoryName'] as String? ?? '',
    );

Map<String, dynamic> _$UserPreferenceToJson(UserPreference instance) =>
    <String, dynamic>{
      'categoryId': instance.categoryId,
      'categoryName': instance.categoryName,
    };

ViewHistory _$ViewHistoryFromJson(Map<String, dynamic> json) => ViewHistory(
  id: (json['id'] as num).toInt(),
  destinationId: (json['destinationId'] as num).toInt(),
  destinationName: json['destinationName'] as String? ?? '',
  cityName: json['cityName'] as String? ?? '',
  imageUrl: json['imageUrl'] as String?,
  viewedAt: const UtcDateTimeConverter().fromJson(json['viewedAt'] as String),
);

Map<String, dynamic> _$ViewHistoryToJson(ViewHistory instance) =>
    <String, dynamic>{
      'id': instance.id,
      'destinationId': instance.destinationId,
      'destinationName': instance.destinationName,
      'cityName': instance.cityName,
      'imageUrl': instance.imageUrl,
      'viewedAt': const UtcDateTimeConverter().toJson(instance.viewedAt),
    };

UserActivity _$UserActivityFromJson(Map<String, dynamic> json) => UserActivity(
  reviewCount: (json['reviewCount'] as num?)?.toInt() ?? 0,
  pendingReviewCount: (json['pendingReviewCount'] as num?)?.toInt() ?? 0,
  approvedReviewCount: (json['approvedReviewCount'] as num?)?.toInt() ?? 0,
  rejectedReviewCount: (json['rejectedReviewCount'] as num?)?.toInt() ?? 0,
  tripPlanCount: (json['tripPlanCount'] as num?)?.toInt() ?? 0,
  completedTripPlanCount:
      (json['completedTripPlanCount'] as num?)?.toInt() ?? 0,
  collectionCount: (json['collectionCount'] as num?)?.toInt() ?? 0,
  savedDestinationCount: (json['savedDestinationCount'] as num?)?.toInt() ?? 0,
  viewCount: (json['viewCount'] as num?)?.toInt() ?? 0,
);

Map<String, dynamic> _$UserActivityToJson(UserActivity instance) =>
    <String, dynamic>{
      'reviewCount': instance.reviewCount,
      'pendingReviewCount': instance.pendingReviewCount,
      'approvedReviewCount': instance.approvedReviewCount,
      'rejectedReviewCount': instance.rejectedReviewCount,
      'tripPlanCount': instance.tripPlanCount,
      'completedTripPlanCount': instance.completedTripPlanCount,
      'collectionCount': instance.collectionCount,
      'savedDestinationCount': instance.savedDestinationCount,
      'viewCount': instance.viewCount,
    };
