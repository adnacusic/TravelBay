import 'package:json_annotation/json_annotation.dart';

import 'enums.dart';
import 'utc_date_time_converter.dart';

part 'trip_plan.g.dart';

@JsonSerializable(explicitToJson: true)
@UtcDateTimeConverter()
class TripPlan {
  TripPlan({
    required this.id,
    required this.name,
    required this.startDate,
    required this.endDate,
    required this.status,
    required this.createdAt,
    this.updatedAt,
    this.items = const [],
  });

  final int id;
  final String name;
  final DateTime startDate;
  final DateTime endDate;
  final TripPlanStatus status;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final List<TripPlanItem> items;

  /// Number of calendar days the trip covers (day numbers run 1..dayCount).
  int get dayCount => endDate.difference(startDate).inDays + 1;

  factory TripPlan.fromJson(Map<String, dynamic> json) =>
      _$TripPlanFromJson(json);

  Map<String, dynamic> toJson() => _$TripPlanToJson(this);
}

@JsonSerializable()
class TripPlanItem {
  TripPlanItem({
    required this.id,
    required this.destinationId,
    this.destinationName = '',
    required this.dayNumber,
    this.orderIndex = 0,
    this.notes,
  });

  final int id;
  final int destinationId;
  final String destinationName;
  final int dayNumber;
  final int orderIndex;
  final String? notes;

  factory TripPlanItem.fromJson(Map<String, dynamic> json) =>
      _$TripPlanItemFromJson(json);

  Map<String, dynamic> toJson() => _$TripPlanItemToJson(this);
}
