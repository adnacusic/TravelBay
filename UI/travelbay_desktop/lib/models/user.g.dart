// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

User _$UserFromJson(Map<String, dynamic> json) => User(
  id: (json['id'] as num).toInt(),
  firstName: json['firstName'] as String,
  lastName: json['lastName'] as String,
  email: json['email'] as String,
  username: json['username'] as String,
  role: json['role'] as String?,
  isActive: json['isActive'] as bool? ?? true,
  createdAt: const UtcDateTimeConverter().fromJson(json['createdAt'] as String),
  lastLoginAt: _$JsonConverterFromJson<String, DateTime>(
    json['lastLoginAt'],
    const UtcDateTimeConverter().fromJson,
  ),
  phoneNumber: json['phoneNumber'] as String?,
  updatedAt: _$JsonConverterFromJson<String, DateTime>(
    json['updatedAt'],
    const UtcDateTimeConverter().fromJson,
  ),
  profileImageId: (json['profileImageId'] as num?)?.toInt(),
);

Map<String, dynamic> _$UserToJson(User instance) => <String, dynamic>{
  'id': instance.id,
  'firstName': instance.firstName,
  'lastName': instance.lastName,
  'email': instance.email,
  'username': instance.username,
  'role': instance.role,
  'isActive': instance.isActive,
  'createdAt': const UtcDateTimeConverter().toJson(instance.createdAt),
  'lastLoginAt': _$JsonConverterToJson<String, DateTime>(
    instance.lastLoginAt,
    const UtcDateTimeConverter().toJson,
  ),
  'phoneNumber': instance.phoneNumber,
  'updatedAt': _$JsonConverterToJson<String, DateTime>(
    instance.updatedAt,
    const UtcDateTimeConverter().toJson,
  ),
  'profileImageId': instance.profileImageId,
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
