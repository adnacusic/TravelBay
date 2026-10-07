// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'recommendation.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Recommendation _$RecommendationFromJson(Map<String, dynamic> json) =>
    Recommendation(
      destination: Destination.fromJson(
        json['destination'] as Map<String, dynamic>,
      ),
      matchPercent: (json['matchPercent'] as num).toInt(),
      explanation: json['explanation'] as String,
    );

Map<String, dynamic> _$RecommendationToJson(Recommendation instance) =>
    <String, dynamic>{
      'destination': instance.destination.toJson(),
      'matchPercent': instance.matchPercent,
      'explanation': instance.explanation,
    };
