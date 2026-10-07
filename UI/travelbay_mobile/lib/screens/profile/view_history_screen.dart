import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/user_activity.dart';
import '../../providers/user_activity_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/formatters.dart';
import '../../widgets/destination_widgets.dart';
import '../../widgets/empty_state.dart';
import '../../widgets/paged_list.dart';
import '../destination/destination_details_screen.dart';

/// Every opening of a destination's details, newest first; the recommender uses these views.
class ViewHistoryScreen extends StatefulWidget {
  const ViewHistoryScreen({super.key});

  @override
  State<ViewHistoryScreen> createState() => _ViewHistoryScreenState();
}

class _ViewHistoryScreenState extends State<ViewHistoryScreen> {
  static const _pageSize = 20;

  int _version = 0;

  /// Opening a destination adds a new view, so the list is loaded again afterwards.
  Future<void> _open(ViewHistory view) async {
    await openPage<void>(context, DestinationDetailsScreen(destinationId: view.destinationId));
    if (mounted) {
      setState(() => _version++);
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      title: 'Historija pregleda',
      child: PagedList<ViewHistory>(
        key: ValueKey(_version),
        padding: const EdgeInsets.all(12),
        fetchPage: (page) =>
            context.read<ViewHistoryProvider>().get(page: page, pageSize: _pageSize),
        header: Padding(
          padding: const EdgeInsets.fromLTRB(4, 0, 4, 8),
          child: Text(
            'Na osnovu pregleda dobijate preporuke na početnoj stranici.',
            style: Theme.of(context).textTheme.bodySmall,
          ),
        ),
        empty: const EmptyState(
          icon: Icons.history,
          text: 'Još niste pregledali nijednu destinaciju.',
        ),
        itemBuilder: (context, view) => DestinationRefTile(
          name: view.destinationName,
          cityName: view.cityName,
          imageUrl: view.imageUrl,
          detail: 'Pregledano ${formatDateTime(view.viewedAt)}',
          onTap: () => _open(view),
          trailing: const Icon(Icons.chevron_right),
        ),
      ),
    );
  }
}
