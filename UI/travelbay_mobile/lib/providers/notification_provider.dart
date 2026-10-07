import 'dart:async';

import 'package:flutter/foundation.dart';

import '../models/app_notification.dart';
import '../models/search_result.dart';
import 'base_provider.dart';

/// The signed-in user's notifications. While the main screen is open the unread
/// count is polled, so the bell badge and an open list refresh on their own.
class NotificationProvider extends BaseProvider<AppNotification> {
  NotificationProvider() : super('Notifications');

  static const pollInterval = Duration(seconds: 15);

  Timer? _timer;
  int _unreadCount = 0;

  int get unreadCount => _unreadCount;

  @override
  AppNotification fromJson(Map<String, dynamic> json) =>
      AppNotification.fromJson(json);

  Future<SearchResult<AppNotification>> list({int page = 1, int pageSize = 20}) =>
      get(filter: {'page': page, 'pageSize': pageSize, 'includeTotalCount': true});

  Future<void> markAsRead(int id) async {
    await sendAction('PUT', '$endpoint/$id/MarkAsRead');
    await refreshUnreadCount();
  }

  Future<void> markAllAsRead() async {
    await sendAction('PUT', '$endpoint/MarkAllAsRead');
    await refreshUnreadCount();
  }

  void startPolling() {
    _timer?.cancel();
    refreshUnreadCount();
    _timer = Timer.periodic(pollInterval, (_) => refreshUnreadCount());
  }

  void stopPolling() {
    _timer?.cancel();
    _timer = null;
  }

  /// A failed poll keeps the last known count; the next tick tries again.
  Future<void> refreshUnreadCount() async {
    try {
      final result = await get(filter: {'isRead': false, 'pageSize': 1, 'includeTotalCount': true});
      final count = result.totalCount ?? result.items.length;
      if (count != _unreadCount) {
        _unreadCount = count;
        notifyListeners();
      }
    } on Exception catch (e) {
      debugPrint('Notification poll failed: $e');
    }
  }

  @override
  void dispose() {
    stopPolling();
    super.dispose();
  }
}
