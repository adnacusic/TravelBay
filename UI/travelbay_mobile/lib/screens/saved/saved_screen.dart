import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/collection.dart';
import '../../models/user_activity.dart';
import '../../providers/collection_provider.dart';
import '../../providers/user_activity_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/destination_widgets.dart';
import '../../widgets/empty_state.dart';
import '../../widgets/network_thumbnail.dart';
import '../../widgets/paged_list.dart';
import '../destination/destination_details_screen.dart';
import 'collection_details_screen.dart';
import 'collection_name_dialog.dart';

/// "Sačuvano" tab: saved destinations and the user's collections.
/// [activation] changes every time the tab is opened, so both lists show fresh data.
class SavedScreen extends StatefulWidget {
  const SavedScreen({super.key, required this.activation});

  final int activation;

  @override
  State<SavedScreen> createState() => _SavedScreenState();
}

class _SavedScreenState extends State<SavedScreen> {
  static const _savedPageSize = 20;

  int _version = 0;

  void _reload() => setState(() => _version++);

  @override
  void didUpdateWidget(SavedScreen oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.activation != widget.activation) {
      _version++;
    }
  }

  Future<void> _openDestination(int destinationId) async {
    await openPage<void>(context, DestinationDetailsScreen(destinationId: destinationId));
    _reload();
  }

  Future<void> _unsave(SavedDestination saved) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Ukloni iz sačuvanih',
      message: 'Ukloniti "${saved.destinationName}" iz sačuvanih destinacija?',
      confirmLabel: 'Ukloni',
    );
    if (!confirmed || !mounted) {
      return;
    }
    try {
      await context.read<SavedDestinationProvider>().remove(saved.id);
      if (mounted) {
        showSuccessMessage(context, '${saved.destinationName} je uklonjena iz sačuvanih.');
        _reload();
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _createCollection() async {
    final created = await CollectionNameDialog.show(context);
    if (created != null && mounted) {
      showSuccessMessage(
        context,
        'Kolekcija "${created.name}" je napravljena. Destinacije dodajete iz njihovih detalja.',
      );
      _reload();
    }
  }

  Future<void> _openCollection(Collection collection) async {
    final message = await openPage<String>(
      context,
      CollectionDetailsScreen(collectionId: collection.id),
    );
    if (!mounted) {
      return;
    }
    if (message != null) {
      showSuccessMessage(context, message);
    }
    _reload();
  }

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 2,
      child: Builder(
        builder: (context) => Scaffold(
          appBar: AppBar(
            title: const Text('Sačuvano'),
            bottom: const TabBar(
              tabs: [
                Tab(text: 'Destinacije'),
                Tab(text: 'Kolekcije'),
              ],
            ),
          ),
          floatingActionButton: _CollectionFab(onPressed: _createCollection),
          body: TabBarView(
            children: [
              PagedList<SavedDestination>(
                key: ValueKey('saved-$_version'),
                fetchPage: (page) => context
                    .read<SavedDestinationProvider>()
                    .get(page: page, pageSize: _savedPageSize),
                empty: const EmptyState(
                  icon: Icons.bookmark_border,
                  text: 'Još niste sačuvali nijednu destinaciju.\n'
                      'Otvorite destinaciju i dodirnite "Sačuvaj".',
                ),
                itemBuilder: (context, saved) => DestinationRefTile(
                  name: saved.destinationName,
                  cityName: saved.cityName,
                  imageUrl: saved.imageUrl,
                  detail: 'Sačuvano ${formatDate(saved.savedAt)}',
                  onTap: () => _openDestination(saved.destinationId),
                  trailing: IconButton(
                    tooltip: 'Ukloni iz sačuvanih',
                    icon: const Icon(Icons.bookmark_remove_outlined),
                    onPressed: () => _unsave(saved),
                  ),
                ),
              ),
              _CollectionList(
                key: ValueKey('collections-$_version'),
                onOpen: _openCollection,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// "Nova kolekcija" is offered only on the collections tab.
class _CollectionFab extends StatelessWidget {
  const _CollectionFab({required this.onPressed});

  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    final controller = DefaultTabController.of(context);
    return ListenableBuilder(
      listenable: controller,
      builder: (context, _) => controller.index == 1
          ? FloatingActionButton.extended(
              onPressed: onPressed,
              icon: const Icon(Icons.add),
              label: const Text('Nova kolekcija'),
            )
          : const SizedBox.shrink(),
    );
  }
}

class _CollectionList extends StatelessWidget {
  const _CollectionList({super.key, required this.onOpen});

  static const _pageSize = 20;

  final ValueChanged<Collection> onOpen;

  @override
  Widget build(BuildContext context) {
    return PagedList<Collection>(
      fetchPage: (page) =>
          context.read<CollectionProvider>().list(page: page, pageSize: _pageSize),
      empty: const EmptyState(
        icon: Icons.collections_bookmark_outlined,
        text: 'Još nemate kolekcija.\nNapravite prvu dugmetom "Nova kolekcija".',
      ),
      itemBuilder: (context, collection) => _CollectionCard(
        collection: collection,
        onTap: () => onOpen(collection),
      ),
    );
  }
}

class _CollectionCard extends StatelessWidget {
  const _CollectionCard({required this.collection, required this.onTap});

  static const _previewCount = 3;

  final Collection collection;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final count = collection.items.length;
    final preview = collection.items.take(_previewCount).toList();

    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      collection.name,
                      style: theme.textTheme.titleSmall,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                  Text(destinationCountLabel(count), style: theme.textTheme.bodySmall),
                  const Icon(Icons.chevron_right),
                ],
              ),
              const SizedBox(height: 8),
              if (preview.isEmpty)
                Text('Kolekcija je prazna.', style: theme.textTheme.bodySmall)
              else
                Row(
                  children: [
                    for (final item in preview)
                      Padding(
                        padding: const EdgeInsets.only(right: 8),
                        child: NetworkThumbnail(imageUrl: item.imageUrl, width: 84, height: 60),
                      ),
                  ],
                ),
            ],
          ),
        ),
      ),
    );
  }
}
