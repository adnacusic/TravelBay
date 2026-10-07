import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/app_notification.dart';
import '../../models/enums.dart';
import '../../providers/notification_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/empty_state.dart';

/// The user's notifications, newest first. The list follows the provider's polling:
/// when the unread count changes (a new notification arrived), it loads again by itself.
class NotificationsScreen extends StatefulWidget {
  const NotificationsScreen({super.key});

  @override
  State<NotificationsScreen> createState() => _NotificationsScreenState();
}

class _NotificationsScreenState extends State<NotificationsScreen> {
  static const _pageSize = 20;
  static const _loadMoreExtent = 300.0;

  late final NotificationProvider _provider;

  final List<AppNotification> _items = [];
  int _totalCount = 0;
  int _page = 0;
  bool _isLoading = false;
  String? _error;
  int _knownUnread = 0;

  @override
  void initState() {
    super.initState();
    _provider = context.read<NotificationProvider>();
    _knownUnread = _provider.unreadCount;
    _provider.addListener(_onProviderChanged);
    _reload();
  }

  @override
  void dispose() {
    _provider.removeListener(_onProviderChanged);
    super.dispose();
  }

  void _onProviderChanged() {
    final unread = _provider.unreadCount;
    if (unread != _knownUnread) {
      _knownUnread = unread;
      _reload();
    }
  }

  Future<void> _reload() async {
    if (_isLoading) {
      return;
    }
    _items.clear();
    _page = 0;
    _totalCount = 0;
    await _loadNextPage();
  }

  Future<void> _loadNextPage() async {
    if (_isLoading) {
      return;
    }
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final result = await _provider.list(page: _page + 1, pageSize: _pageSize);
      if (mounted) {
        setState(() {
          _items.addAll(result.items);
          _totalCount = result.totalCount ?? _items.length;
          _page++;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  Future<void> _markAsRead(AppNotification notification) async {
    if (notification.isRead) {
      return;
    }
    try {
      _knownUnread = _provider.unreadCount - 1;
      await _provider.markAsRead(notification.id);
      if (mounted) {
        setState(() {
          final index = _items.indexWhere((n) => n.id == notification.id);
          if (index >= 0) {
            _items[index] = _asRead(notification);
          }
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _markAllAsRead() async {
    try {
      _knownUnread = 0;
      await _provider.markAllAsRead();
      if (mounted) {
        setState(() {
          for (var i = 0; i < _items.length; i++) {
            _items[i] = _asRead(_items[i]);
          }
        });
        showSuccessMessage(context, 'Sve notifikacije su označene kao pročitane.');
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  AppNotification _asRead(AppNotification n) => AppNotification(
        id: n.id,
        title: n.title,
        message: n.message,
        type: n.type,
        isRead: true,
        createdAt: n.createdAt,
      );

  bool _onScroll(ScrollNotification notification) {
    if (_items.length < _totalCount &&
        !_isLoading &&
        _error == null &&
        notification.metrics.extentAfter < _loadMoreExtent) {
      _loadNextPage();
    }
    return false;
  }

  @override
  Widget build(BuildContext context) {
    final unread = context.watch<NotificationProvider>().unreadCount;

    return MasterScreen(
      title: 'Notifikacije',
      actions: [
        IconButton(
          tooltip: 'Označi sve kao pročitano',
          icon: const Icon(Icons.done_all),
          onPressed: unread == 0 ? null : _markAllAsRead,
        ),
      ],
      child: _buildBody(unread),
    );
  }

  Widget _buildBody(int unread) {
    if (_items.isEmpty) {
      if (_error != null) {
        return Center(child: Padding(padding: const EdgeInsets.all(24), child: Text(_error!)));
      }
      if (_isLoading) {
        return const Center(child: CircularProgressIndicator());
      }
      return const EmptyState(
        icon: Icons.notifications_none,
        text: 'Nemate notifikacija.\nOvdje stižu odluke o vašim recenzijama i promjene planova.',
      );
    }

    final theme = Theme.of(context);
    return NotificationListener<ScrollNotification>(
      onNotification: _onScroll,
      child: ListView(
        padding: const EdgeInsets.symmetric(vertical: 8),
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 4),
            child: Text(
              unread == 0 ? 'Sve je pročitano.' : 'Nepročitanih: $unread',
              style: theme.textTheme.bodySmall,
            ),
          ),
          for (final notification in _items) _NotificationTile(
            notification: notification,
            onTap: () => _markAsRead(notification),
          ),
          if (_isLoading)
            const Padding(
              padding: EdgeInsets.all(16),
              child: Center(child: CircularProgressIndicator()),
            ),
        ],
      ),
    );
  }
}

class _NotificationTile extends StatelessWidget {
  const _NotificationTile({required this.notification, required this.onTap});

  final AppNotification notification;
  final VoidCallback onTap;

  static IconData _icon(NotificationType type) => switch (type) {
        NotificationType.reviewApproved => Icons.thumb_up_alt_outlined,
        NotificationType.reviewRejected => Icons.thumb_down_alt_outlined,
        NotificationType.tripStatusChanged => Icons.map_outlined,
        NotificationType.news => Icons.newspaper_outlined,
        NotificationType.general => Icons.info_outline,
      };

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final unread = !notification.isRead;

    return ListTile(
      tileColor: unread ? theme.colorScheme.primaryContainer.withValues(alpha: 0.35) : null,
      leading: CircleAvatar(child: Icon(_icon(notification.type), size: 20)),
      title: Text(
        notification.title,
        style: TextStyle(fontWeight: unread ? FontWeight.w700 : FontWeight.normal),
      ),
      subtitle: Text('${notification.message}\n${formatDateTime(notification.createdAt)}'),
      isThreeLine: true,
      trailing: unread
          ? Icon(Icons.circle, size: 10, color: theme.colorScheme.primary, semanticLabel: 'Nepročitano')
          : null,
      onTap: onTap,
    );
  }
}
