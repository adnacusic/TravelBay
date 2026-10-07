import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../models/collection.dart';
import '../../providers/collection_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';
import '../../widgets/form_title.dart';

/// Creates a collection (no [collection]) or renames one. Resolves to the saved collection.
class CollectionNameDialog extends StatefulWidget {
  const CollectionNameDialog({super.key, this.collection});

  final Collection? collection;

  static Future<Collection?> show(BuildContext context, {Collection? collection}) {
    return showDialog<Collection>(
      context: context,
      barrierDismissible: false,
      builder: (context) => CollectionNameDialog(collection: collection),
    );
  }

  @override
  State<CollectionNameDialog> createState() => _CollectionNameDialogState();
}

class _CollectionNameDialogState extends State<CollectionNameDialog> {
  /// Same limit as the API CollectionInsertValidator.
  static const _maxNameLength = 200;

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  bool get _isEdit => widget.collection != null;

  Future<void> _save() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final name = (form.value['name'] as String).trim();
    setState(() => _isSaving = true);
    try {
      final provider = context.read<CollectionProvider>();
      final saved = _isEdit
          ? await provider.rename(widget.collection!.id, name)
          : await provider.create(name);
      if (mounted) {
        Navigator.pop(context, saved);
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
    return AlertDialog(
      titlePadding: const EdgeInsets.fromLTRB(24, 12, 8, 0),
      title: FormTitle(
        title: _isEdit ? 'Preimenuj kolekciju' : 'Nova kolekcija',
        enabled: !_isSaving,
      ),
      content: FormBuilder(
        key: _formKey,
        initialValue: {'name': widget.collection?.name ?? ''},
        child: FormBuilderTextField(
          name: 'name',
          autofocus: true,
          maxLength: _maxNameLength,
          decoration: const InputDecoration(
            labelText: 'Naziv kolekcije *',
            hintText: 'npr. Ljeto na moru',
          ),
          validator: Validators.requiredText('Naziv kolekcije', maxLength: _maxNameLength),
        ),
      ),
      actions: [
        TextButton(
          onPressed: _isSaving ? null : () => Navigator.pop(context),
          child: const Text('Odustani'),
        ),
        FilledButton(
          onPressed: _isSaving ? null : _save,
          child: Text(_isEdit ? 'Sačuvaj' : 'Napravi'),
        ),
      ],
    );
  }
}
