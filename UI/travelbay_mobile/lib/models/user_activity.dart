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
    required this.savedAt,
  });

  final int id;
  final int destinationId;
  final String destinationName;
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
    required this.viewedAt,
  });

  final int id;
  final int destinationId;
  final String destinationName;
  final DateTime viewedAt;

  factory ViewHistory.fromJson(Map<String, dynamic> json) =>
      _$ViewHistoryFromJson(json);

  Map<String, dynamic> toJson() => _$ViewHistoryToJson(this);
}
