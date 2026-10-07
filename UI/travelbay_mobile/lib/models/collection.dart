import 'package:json_annotation/json_annotation.dart';

import 'utc_date_time_converter.dart';

part 'collection.g.dart';

@JsonSerializable(explicitToJson: true)
@UtcDateTimeConverter()
class Collection {
  Collection({
    required this.id,
    required this.name,
    required this.createdAt,
    this.items = const [],
  });

  final int id;
  final String name;
  final DateTime createdAt;
  final List<CollectionItem> items;

  factory Collection.fromJson(Map<String, dynamic> json) =>
      _$CollectionFromJson(json);

  Map<String, dynamic> toJson() => _$CollectionToJson(this);
}

@JsonSerializable()
@UtcDateTimeConverter()
class CollectionItem {
  CollectionItem({
    required this.id,
    required this.destinationId,
    this.destinationName = '',
    required this.addedAt,
  });

  final int id;
  final int destinationId;
  final String destinationName;
  final DateTime addedAt;

  factory CollectionItem.fromJson(Map<String, dynamic> json) =>
      _$CollectionItemFromJson(json);

  Map<String, dynamic> toJson() => _$CollectionItemToJson(this);
}
