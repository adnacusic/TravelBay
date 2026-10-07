// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'trip_plan.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TripPlan _$TripPlanFromJson(Map<String, dynamic> json) => TripPlan(
  id: (json['id'] as num).toInt(),
  name: json['name'] as String,
  startDate: const UtcDateTimeConverter().fromJson(json['startDate'] as String),
  endDate: const UtcDateTimeConverter().fromJson(json['endDate'] as String),
  status: $enumDecode(_$TripPlanStatusEnumMap, json['status']),
  createdAt: const UtcDateTimeConverter().fromJson(json['createdAt'] as String),
  updatedAt: _$JsonConverterFromJson<String, DateTime>(
    json['updatedAt'],
    const UtcDateTimeConverter().fromJson,
  ),
  items:
      (json['items'] as List<dynamic>?)
          ?.map((e) => TripPlanItem.fromJson(e as Map<String, dynamic>))
          .toList() ??
      const [],
);

Map<String, dynamic> _$TripPlanToJson(TripPlan instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'startDate': const UtcDateTimeConverter().toJson(instance.startDate),
  'endDate': const UtcDateTimeConverter().toJson(instance.endDate),
  'status': _$TripPlanStatusEnumMap[instance.status]!,
  'createdAt': const UtcDateTimeConverter().toJson(instance.createdAt),
  'updatedAt': _$JsonConverterToJson<String, DateTime>(
    instance.updatedAt,
    const UtcDateTimeConverter().toJson,
  ),
  'items': instance.items.map((e) => e.toJson()).toList(),
};

const _$TripPlanStatusEnumMap = {
  TripPlanStatus.draft: 0,
  TripPlanStatus.active: 1,
  TripPlanStatus.completed: 2,
  TripPlanStatus.cancelled: 3,
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);

TripPlanItem _$TripPlanItemFromJson(Map<String, dynamic> json) => TripPlanItem(
  id: (json['id'] as num).toInt(),
  destinationId: (json['destinationId'] as num).toInt(),
  destinationName: json['destinationName'] as String? ?? '',
  dayNumber: (json['dayNumber'] as num).toInt(),
  orderIndex: (json['orderIndex'] as num?)?.toInt() ?? 0,
  notes: json['notes'] as String?,
);

Map<String, dynamic> _$TripPlanItemToJson(TripPlanItem instance) =>
    <String, dynamic>{
      'id': instance.id,
      'destinationId': instance.destinationId,
      'destinationName': instance.destinationName,
      'dayNumber': instance.dayNumber,
      'orderIndex': instance.orderIndex,
      'notes': instance.notes,
    };
