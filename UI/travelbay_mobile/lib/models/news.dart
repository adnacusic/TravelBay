import 'package:json_annotation/json_annotation.dart';

import 'utc_date_time_converter.dart';

part 'news.g.dart';

@JsonSerializable()
@UtcDateTimeConverter()
class News {
  News({
    required this.id,
    required this.title,
    required this.content,
    required this.imageUrl,
    required this.publishedAt,
    required this.createdAt,
  });

  final int id;
  final String title;
  final String content;

  /// Absolute URL, or an API-relative path for uploaded images (see AppConfig.resolveImageUrl).
  final String imageUrl;
  final DateTime publishedAt;
  final DateTime createdAt;

  factory News.fromJson(Map<String, dynamic> json) => _$NewsFromJson(json);

  Map<String, dynamic> toJson() => _$NewsToJson(this);
}
