import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/collection.dart';
import '../../providers/collection_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/destination_widgets.dart';
import '../../widgets/empty_state.dart';
import '../destination/destination_details_screen.dart';
import 'collection_name_dialog.dart';

/// One collection with its destinations: rename, remove a destination, delete the collection.
/// Pops with a success message when the collection was deleted.
class CollectionDetailsScreen extends StatefulWidget {
  const CollectionDetailsScreen({super.key, required this.collectionId});

  final int collectionId;

  @override
  State<CollectionDetailsScreen> createState() => _CollectionDetailsScreenState();
}

class _CollectionDetailsScreenState extends State<CollectionDetailsScreen> {
  Collection? _collection;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final collection = await context.read<CollectionProvider>().getById(widget.collectionId);
      if (mounted) {
        setState(() {
          _collection = collection;
          _error = null;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  Future<void> _rename(Collection collection) async {
    final saved = await CollectionNameDialog.show(context, collection: collection);
    if (saved != null && mounted) {
      setState(() => _collection = saved);
      showSuccessMessage(context, 'Kolekcija je preimenovana u "${saved.name}".');
    }
  }

  Future<void> _delete(Collection collection) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje kolekcije',
      message: 'Obrisati kolekciju "${collection.name}"? Destinacije ostaju u aplikaciji '
          'i među sačuvanima, briše se samo ova kolekcija.',
    );
    if (!confirmed || !mounted) {
      return;
    }
    try {
      await context.read<CollectionProvider>().remove(collection.id);
      if (mounted) {
        Navigator.pop(context, 'Kolekcija "${collection.name}" je obrisana.');
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _removeItem(Collection collection, CollectionItem item) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Ukloni iz kolekcije',
      message: 'Ukloniti "${item.destinationName}" iz kolekcije "${collection.name}"?',
      confirmLabel: 'Ukloni',
    );
    if (!confirmed || !mounted) {
      return;
    }
    try {
      await context.read<CollectionProvider>().removeItem(collection.id, item.id);
      if (mounted) {
        showSuccessMessage(context, '${item.destinationName} je uklonjena iz kolekcije.');
        await _load();
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _openDestination(CollectionItem item) async {
    await openPage<void>(context, DestinationDetailsScreen(destinationId: item.destinationId));
    await _load();
  }

  @override
  Widget build(BuildContext context) {
    final collection = _collection;

    return MasterScreen(
      title: collection?.name ?? 'Kolekcija',
      actions: [
        if (collection != null) ...[
          IconButton(
            tooltip: 'Preimenuj',
            icon: const Icon(Icons.edit_outlined),
            onPressed: () => _rename(collection),
          ),
          IconButton(
            tooltip: 'Obriši kolekciju',
            icon: const Icon(Icons.delete_outline),
            onPressed: () => _delete(collection),
          ),
        ],
      ],
      child: _buildBody(collection),
    );
  }

  Widget _buildBody(Collection? collection) {
    if (_error != null) {
      return Center(child: Padding(padding: const EdgeInsets.all(24), child: Text(_error!)));
    }
    if (collection == null) {
      return const Center(child: CircularProgressIndicator());
    }
    if (collection.items.isEmpty) {
      return const EmptyState(
        icon: Icons.collections_bookmark_outlined,
        text: 'Kolekcija je prazna.\n'
            'Otvorite destinaciju i dodirnite "U kolekciju" da je dodate ovdje.',
      );
    }

    final theme = Theme.of(context);
    return ListView(
      padding: const EdgeInsets.all(12),
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(4, 0, 4, 8),
          child: Text(
            '${destinationCountLabel(collection.items.length)} · '
            'napravljena ${formatDate(collection.createdAt)}',
            style: theme.textTheme.bodySmall,
          ),
        ),
        for (final item in collection.items)
          DestinationRefTile(
            name: item.destinationName,
            cityName: item.cityName,
            imageUrl: item.imageUrl,
            detail: 'Dodano ${formatDate(item.addedAt)}',
            onTap: () => _openDestination(item),
            trailing: IconButton(
              tooltip: 'Ukloni iz kolekcije',
              icon: const Icon(Icons.remove_circle_outline),
              onPressed: () => _removeItem(collection, item),
            ),
          ),
      ],
    );
  }
}
