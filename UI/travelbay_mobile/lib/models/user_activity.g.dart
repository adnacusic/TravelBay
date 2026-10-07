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
      savedAt: const UtcDateTimeConverter().fromJson(json['savedAt'] as String),
    );

Map<String, dynamic> _$SavedDestinationToJson(SavedDestination instance) =>
    <String, dynamic>{
      'id': instance.id,
      'destinationId': instance.destinationId,
      'destinationName': instance.destinationName,
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
  viewedAt: const UtcDateTimeConverter().fromJson(json['viewedAt'] as String),
);

Map<String, dynamic> _$ViewHistoryToJson(ViewHistory instance) =>
    <String, dynamic>{
      'id': instance.id,
      'destinationId': instance.destinationId,
      'destinationName': instance.destinationName,
      'viewedAt': const UtcDateTimeConverter().toJson(instance.viewedAt),
    };
