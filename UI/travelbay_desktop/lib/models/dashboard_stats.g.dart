// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'dashboard_stats.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DashboardStats _$DashboardStatsFromJson(
  Map<String, dynamic> json,
) => DashboardStats(
  totalDestinations: (json['totalDestinations'] as num).toInt(),
  totalUsers: (json['totalUsers'] as num).toInt(),
  destinationsWithImages: (json['destinationsWithImages'] as num).toInt(),
  destinationsWithoutImages: (json['destinationsWithoutImages'] as num).toInt(),
  destinationsWithKeywords: (json['destinationsWithKeywords'] as num).toInt(),
  destinationsWithoutKeywords: (json['destinationsWithoutKeywords'] as num)
      .toInt(),
  processedDestinations: (json['processedDestinations'] as num).toInt(),
  processedPercent: (json['processedPercent'] as num).toDouble(),
  recentDestinations:
      (json['recentDestinations'] as List<dynamic>?)
          ?.map((e) => RecentDestination.fromJson(e as Map<String, dynamic>))
          .toList() ??
      const [],
);

Map<String, dynamic> _$DashboardStatsToJson(DashboardStats instance) =>
    <String, dynamic>{
      'totalDestinations': instance.totalDestinations,
      'totalUsers': instance.totalUsers,
      'destinationsWithImages': instance.destinationsWithImages,
      'destinationsWithoutImages': instance.destinationsWithoutImages,
      'destinationsWithKeywords': instance.destinationsWithKeywords,
      'destinationsWithoutKeywords': instance.destinationsWithoutKeywords,
      'processedDestinations': instance.processedDestinations,
      'processedPercent': instance.processedPercent,
      'recentDestinations': instance.recentDestinations
          .map((e) => e.toJson())
          .toList(),
    };

RecentDestination _$RecentDestinationFromJson(Map<String, dynamic> json) =>
    RecentDestination(
      id: (json['id'] as num).toInt(),
      name: json['name'] as String,
      categoryName: json['categoryName'] as String,
      cityName: json['cityName'] as String,
      createdAt: const UtcDateTimeConverter().fromJson(
        json['createdAt'] as String,
      ),
      hasKeywords: json['hasKeywords'] as bool,
      hasImages: json['hasImages'] as bool,
    );

Map<String, dynamic> _$RecentDestinationToJson(RecentDestination instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'categoryName': instance.categoryName,
      'cityName': instance.cityName,
      'createdAt': const UtcDateTimeConverter().toJson(instance.createdAt),
      'hasKeywords': instance.hasKeywords,
      'hasImages': instance.hasImages,
    };
