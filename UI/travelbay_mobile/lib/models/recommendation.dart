import 'package:json_annotation/json_annotation.dart';

import 'destination.dart';

part 'recommendation.g.dart';

/// A recommended destination with its match score and the recommender's explanation.
@JsonSerializable(explicitToJson: true)
class Recommendation {
  Recommendation({
    required this.destination,
    required this.matchPercent,
    required this.explanation,
  });

  final Destination destination;

  /// 0–100.
  final int matchPercent;

  /// Why it was recommended, generated from the real factors behind the score.
  final String explanation;

  factory Recommendation.fromJson(Map<String, dynamic> json) =>
      _$RecommendationFromJson(json);

  Map<String, dynamic> toJson() => _$RecommendationToJson(this);
}
