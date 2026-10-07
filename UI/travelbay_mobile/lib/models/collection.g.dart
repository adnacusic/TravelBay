// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'collection.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Collection _$CollectionFromJson(Map<String, dynamic> json) => Collection(
  id: (json['id'] as num).toInt(),
  name: json['name'] as String,
  createdAt: const UtcDateTimeConverter().fromJson(json['createdAt'] as String),
  items:
      (json['items'] as List<dynamic>?)
          ?.map((e) => CollectionItem.fromJson(e as Map<String, dynamic>))
          .toList() ??
      const [],
);

Map<String, dynamic> _$CollectionToJson(Collection instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'createdAt': const UtcDateTimeConverter().toJson(instance.createdAt),
      'items': instance.items.map((e) => e.toJson()).toList(),
    };

CollectionItem _$CollectionItemFromJson(Map<String, dynamic> json) =>
    CollectionItem(
      id: (json['id'] as num).toInt(),
      destinationId: (json['destinationId'] as num).toInt(),
      destinationName: json['destinationName'] as String? ?? '',
      cityName: json['cityName'] as String? ?? '',
      imageUrl: json['imageUrl'] as String?,
      addedAt: const UtcDateTimeConverter().fromJson(json['addedAt'] as String),
    );

Map<String, dynamic> _$CollectionItemToJson(CollectionItem instance) =>
    <String, dynamic>{
      'id': instance.id,
      'destinationId': instance.destinationId,
      'destinationName': instance.destinationName,
      'cityName': instance.cityName,
      'imageUrl': instance.imageUrl,
      'addedAt': const UtcDateTimeConverter().toJson(instance.addedAt),
    };
