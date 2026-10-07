// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'destination_image.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DestinationImage _$DestinationImageFromJson(Map<String, dynamic> json) =>
    DestinationImage(
      id: (json['id'] as num).toInt(),
      imageUrl: json['imageUrl'] as String,
      source: json['source'] as String?,
      orderIndex: (json['orderIndex'] as num?)?.toInt() ?? 0,
      isAiGenerated: json['isAiGenerated'] as bool? ?? false,
    );

Map<String, dynamic> _$DestinationImageToJson(DestinationImage instance) =>
    <String, dynamic>{
      'id': instance.id,
      'imageUrl': instance.imageUrl,
      'source': instance.source,
      'orderIndex': instance.orderIndex,
      'isAiGenerated': instance.isAiGenerated,
    };
