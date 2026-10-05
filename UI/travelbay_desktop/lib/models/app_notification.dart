import 'package:json_annotation/json_annotation.dart';

import 'enums.dart';
import 'utc_date_time_converter.dart';

part 'app_notification.g.dart';

/// Named AppNotification so it does not clash with Flutter's own Notification class.
@JsonSerializable()
@UtcDateTimeConverter()
class AppNotification {
  AppNotification({
    required this.id,
    required this.title,
    required this.message,
    required this.type,
    this.isRead = false,
    required this.createdAt,
  });

  final int id;
  final String title;
  final String message;
  final NotificationType type;
  final bool isRead;
  final DateTime createdAt;

  factory AppNotification.fromJson(Map<String, dynamic> json) =>
      _$AppNotificationFromJson(json);

  Map<String, dynamic> toJson() => _$AppNotificationToJson(this);
}
