// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Review _$ReviewFromJson(Map<String, dynamic> json) => Review(
  id: (json['id'] as num).toInt(),
  destinationId: (json['destinationId'] as num).toInt(),
  destinationName: json['destinationName'] as String? ?? '',
  userId: (json['userId'] as num).toInt(),
  reviewerDisplayName: json['reviewerDisplayName'] as String? ?? '',
  rating: (json['rating'] as num).toInt(),
  comment: json['comment'] as String?,
  status: $enumDecode(_$ReviewStatusEnumMap, json['status']),
  createdAt: const UtcDateTimeConverter().fromJson(json['createdAt'] as String),
  moderatedByUserId: (json['moderatedByUserId'] as num?)?.toInt(),
  moderatedByDisplayName: json['moderatedByDisplayName'] as String?,
  moderatedAt: _$JsonConverterFromJson<String, DateTime>(
    json['moderatedAt'],
    const UtcDateTimeConverter().fromJson,
  ),
  moderationReason: json['moderationReason'] as String?,
);

Map<String, dynamic> _$ReviewToJson(Review instance) => <String, dynamic>{
  'id': instance.id,
  'destinationId': instance.destinationId,
  'destinationName': instance.destinationName,
  'userId': instance.userId,
  'reviewerDisplayName': instance.reviewerDisplayName,
  'rating': instance.rating,
  'comment': instance.comment,
  'status': _$ReviewStatusEnumMap[instance.status]!,
  'createdAt': const UtcDateTimeConverter().toJson(instance.createdAt),
  'moderatedByUserId': instance.moderatedByUserId,
  'moderatedByDisplayName': instance.moderatedByDisplayName,
  'moderatedAt': _$JsonConverterToJson<String, DateTime>(
    instance.moderatedAt,
    const UtcDateTimeConverter().toJson,
  ),
  'moderationReason': instance.moderationReason,
};

const _$ReviewStatusEnumMap = {
  ReviewStatus.pending: 0,
  ReviewStatus.approved: 1,
  ReviewStatus.rejected: 2,
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
