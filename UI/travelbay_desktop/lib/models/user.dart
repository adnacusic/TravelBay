import 'package:json_annotation/json_annotation.dart';

import 'utc_date_time_converter.dart';

part 'user.g.dart';

@JsonSerializable()
@UtcDateTimeConverter()
class User {
  User({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.email,
    required this.username,
    this.role,
    this.isActive = true,
    required this.createdAt,
    this.lastLoginAt,
    this.phoneNumber,
    this.updatedAt,
    this.profileImageId,
  });

  final int id;
  final String firstName;
  final String lastName;
  final String email;
  final String username;
  final String? role;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? lastLoginAt;
  final String? phoneNumber;
  final DateTime? updatedAt;
  final int? profileImageId;

  String get fullName => '$firstName $lastName'.trim();

  factory User.fromJson(Map<String, dynamic> json) => _$UserFromJson(json);

  Map<String, dynamic> toJson() => _$UserToJson(this);
}
