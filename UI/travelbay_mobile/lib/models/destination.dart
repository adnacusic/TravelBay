import 'package:json_annotation/json_annotation.dart';

import 'category.dart';
import 'destination_image.dart';
import 'utc_date_time_converter.dart';

part 'destination.g.dart';

@JsonSerializable(explicitToJson: true)
@UtcDateTimeConverter()
class Destination {
  Destination({
    required this.id,
    required this.name,
    required this.description,
    required this.categoryId,
    this.category,
    required this.cityId,
    this.cityName = '',
    this.keywords,
    required this.createdAt,
    this.updatedAt,
    this.images = const [],
    this.averageRating,
    this.reviewCount = 0,
  });

  final int id;
  final String name;
  final String description;
  final int categoryId;
  final Category? category;
  final int cityId;
  final String cityName;

  /// Comma-separated, filled in by the AI agent (or by the admin).
  final String? keywords;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final List<DestinationImage> images;
  final double? averageRating;
  final int reviewCount;

  bool get hasKeywords => keywords != null && keywords!.trim().isNotEmpty;
  bool get hasImages => images.isNotEmpty;

  DestinationImage? get coverImage => images.isEmpty ? null : images.first;

  factory Destination.fromJson(Map<String, dynamic> json) =>
      _$DestinationFromJson(json);

  Map<String, dynamic> toJson() => _$DestinationToJson(this);
}
