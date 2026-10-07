import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../models/collection.dart';
import '../../models/destination.dart';
import '../../providers/collection_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';

/// Puts a destination into one of the user's collections, or into a new one.
/// Resolves to a success message.
class AddToCollectionSheet extends StatefulWidget {
  const AddToCollectionSheet({super.key, required this.destination});

  final Destination destination;

  static Future<String?> show(BuildContext context, Destination destination) {
    return showModalBottomSheet<String>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      builder: (context) => Padding(
        padding: EdgeInsets.only(bottom: MediaQuery.viewInsetsOf(context).bottom),
        child: AddToCollectionSheet(destination: destination),
      ),
    );
  }

  @override
  State<AddToCollectionSheet> createState() => _AddToCollectionSheetState();
}

class _AddToCollectionSheetState extends State<AddToCollectionSheet> {
  static const _maxNameLength = 200;

  final _formKey = GlobalKey<FormBuilderState>();

  List<Collection>? _collections;
  String? _error;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final result = await context.read<CollectionProvider>().list();
      if (mounted) {
        setState(() => _collections = result.items);
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  bool _contains(Collection collection) =>
      collection.items.any((i) => i.destinationId == widget.destination.id);

  Future<void> _addTo(Collection collection) async {
    setState(() => _isSaving = true);
    try {
      await context.read<CollectionProvider>().addItem(collection.id, widget.destination.id);
      if (mounted) {
        Navigator.pop(context, '${widget.destination.name} je dodana u kolekciju "${collection.name}".');
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSaving = false);
      }
    }
  }

  Future<void> _createAndAdd() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    setState(() => _isSaving = true);
    try {
      final provider = context.read<CollectionProvider>();
      final collection = await provider.create((form.value['name'] as String).trim());
      await provider.addItem(collection.id, widget.destination.id);
      if (mounted) {
        Navigator.pop(
          context,
          'Kolekcija "${collection.name}" je napravljena i ${widget.destination.name} je dodana u nju.',
        );
      }
    } on Exception catch (e) {
      if (mounted) {
        showFormErrors(context, _formKey.currentState, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSaving = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final collections = _collections;

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.fromLTRB(20, 0, 20, 20),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text('Dodaj u kolekciju', style: theme.textTheme.titleMedium),
            const SizedBox(height: 8),
            if (_error != null)
              Text(_error!)
            else if (collections == null)
              const Padding(
                padding: EdgeInsets.all(16),
                child: Center(child: CircularProgressIndicator()),
              )
            else if (collections.isEmpty)
              Text('Još nemate kolekcija. Napravite prvu ispod.', style: theme.textTheme.bodyMedium)
            else
              ConstrainedBox(
                constraints: const BoxConstraints(maxHeight: 260),
                child: ListView(
                  shrinkWrap: true,
                  children: [
                    for (final collection in collections)
                      ListTile(
                        leading: const Icon(Icons.collections_bookmark_outlined),
                        title: Text(collection.name),
                        subtitle: Text(
                          _contains(collection)
                              ? 'Destinacija je već u ovoj kolekciji'
                              : '${collection.items.length} destinacija',
                        ),
                        enabled: !_isSaving && !_contains(collection),
                        onTap: () => _addTo(collection),
                      ),
                  ],
                ),
              ),
            const Divider(height: 24),
            FormBuilder(
              key: _formKey,
              child: FormBuilderTextField(
                name: 'name',
                maxLength: _maxNameLength,
                decoration: const InputDecoration(
                  labelText: 'Nova kolekcija',
                  hintText: 'npr. Ljeto na moru',
                ),
                validator: Validators.requiredText('Naziv kolekcije', maxLength: _maxNameLength),
              ),
            ),
            const SizedBox(height: 8),
            FilledButton.icon(
              onPressed: _isSaving ? null : _createAndAdd,
              icon: const Icon(Icons.add),
              label: const Text('Napravi kolekciju i dodaj'),
            ),
          ],
        ),
      ),
    );
  }
}
