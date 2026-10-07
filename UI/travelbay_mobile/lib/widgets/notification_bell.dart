import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../providers/notification_provider.dart';
import '../screens/notifications/notifications_screen.dart';
import '../utils/app_navigator.dart';

/// App bar bell with the number of unread notifications; the number follows the polling.
class NotificationBell extends StatelessWidget {
  const NotificationBell({super.key});

  static const _maxShownCount = 99;

  @override
  Widget build(BuildContext context) {
    final unread = context.watch<NotificationProvider>().unreadCount;
    return IconButton(
      tooltip: unread == 0 ? 'Notifikacije' : 'Notifikacije ($unread nepročitanih)',
      onPressed: () => openPage<void>(context, const NotificationsScreen()),
      icon: Badge(
        isLabelVisible: unread > 0,
        label: Text(unread > _maxShownCount ? '$_maxShownCount+' : '$unread'),
        child: Icon(unread > 0 ? Icons.notifications : Icons.notifications_none),
      ),
    );
  }
}
