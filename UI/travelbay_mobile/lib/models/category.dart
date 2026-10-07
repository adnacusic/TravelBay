import 'package:json_annotation/json_annotation.dart';

import 'utc_date_time_converter.dart';

part 'category.g.dart';

@JsonSerializable()
@UtcDateTimeConverter()
class Category {
  Category({
    required this.id,
    required this.name,
    this.iconName,
    this.isActive = true,
    this.destinationCount = 0,
    this.createdAt,
  });

  final int id;
  final String name;
  final String? iconName;
  final bool isActive;

  /// Filled in list responses only.
  final int destinationCount;
  final DateTime? createdAt;

  factory Category.fromJson(Map<String, dynamic> json) =>
      _$CategoryFromJson(json);

  Map<String, dynamic> toJson() => _$CategoryToJson(this);
}
