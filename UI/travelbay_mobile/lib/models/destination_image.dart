import 'package:json_annotation/json_annotation.dart';

part 'destination_image.g.dart';

@JsonSerializable()
class DestinationImage {
  DestinationImage({
    required this.id,
    required this.imageUrl,
    this.source,
    this.orderIndex = 0,
    this.isAiGenerated = false,
  });

  final int id;

  /// Absolute URL, or an API-relative path for uploaded images (see AppConfig.resolveImageUrl).
  final String imageUrl;
  final String? source;
  final int orderIndex;
  final bool isAiGenerated;

  factory DestinationImage.fromJson(Map<String, dynamic> json) =>
      _$DestinationImageFromJson(json);

  Map<String, dynamic> toJson() => _$DestinationImageToJson(this);
}
