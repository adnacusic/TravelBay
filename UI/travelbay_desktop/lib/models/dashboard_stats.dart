import 'package:json_annotation/json_annotation.dart';

import 'utc_date_time_converter.dart';

part 'dashboard_stats.g.dart';

@JsonSerializable(explicitToJson: true)
class DashboardStats {
  DashboardStats({
    required this.totalDestinations,
    required this.totalUsers,
    required this.destinationsWithImages,
    required this.destinationsWithoutImages,
    required this.destinationsWithKeywords,
    required this.destinationsWithoutKeywords,
    required this.processedDestinations,
    required this.processedPercent,
    this.recentDestinations = const [],
  });

  final int totalDestinations;
  final int totalUsers;
  final int destinationsWithImages;
  final int destinationsWithoutImages;
  final int destinationsWithKeywords;
  final int destinationsWithoutKeywords;

  /// Destinations with both keywords and at least one image.
  final int processedDestinations;
  final double processedPercent;
  final List<RecentDestination> recentDestinations;

  factory DashboardStats.fromJson(Map<String, dynamic> json) =>
      _$DashboardStatsFromJson(json);

  Map<String, dynamic> toJson() => _$DashboardStatsToJson(this);
}

@JsonSerializable()
@UtcDateTimeConverter()
class RecentDestination {
  RecentDestination({
    required this.id,
    required this.name,
    required this.categoryName,
    required this.cityName,
    required this.createdAt,
    required this.hasKeywords,
    required this.hasImages,
  });

  final int id;
  final String name;
  final String categoryName;
  final String cityName;
  final DateTime createdAt;
  final bool hasKeywords;
  final bool hasImages;

  bool get isComplete => hasKeywords && hasImages;

  factory RecentDestination.fromJson(Map<String, dynamic> json) =>
      _$RecentDestinationFromJson(json);

  Map<String, dynamic> toJson() => _$RecentDestinationToJson(this);
}
