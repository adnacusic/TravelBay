import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../models/country.dart';
import '../../providers/country_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';
import '../../widgets/form_dialog.dart';

/// Create (no [country]) or rename a country. Resolves to a success message when saved.
class CountryFormDialog extends StatefulWidget {
  const CountryFormDialog({super.key, this.country});

  final Country? country;

  @override
  State<CountryFormDialog> createState() => _CountryFormDialogState();
}

class _CountryFormDialogState extends State<CountryFormDialog> {
  static const _maxNameLength = 100;

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  bool get _isEdit => widget.country != null;

  Future<void> _submit() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final request = {'name': (form.value['name'] as String).trim()};

    setState(() => _isSaving = true);
    try {
      final provider = context.read<CountryProvider>();
      final saved = _isEdit
          ? await provider.update(widget.country!.id, request)
          : await provider.insert(request);
      if (mounted) {
        Navigator.pop(
          context,
          _isEdit
              ? 'Izmjene države "${saved.name}" su sačuvane.'
              : 'Država "${saved.name}" je dodana.',
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
    return FormDialog(
      title: _isEdit ? 'Uređivanje države' : 'Nova država',
      submitLabel: _isEdit ? 'Sačuvaj izmjene' : 'Dodaj državu',
      isSubmitting: _isSaving,
      onSubmit: _submit,
      child: FormBuilder(
        key: _formKey,
        initialValue: {'name': widget.country?.name},
        child: FormBuilderTextField(
          name: 'name',
          autofocus: true,
          maxLength: _maxNameLength,
          decoration: const InputDecoration(labelText: 'Naziv *'),
          validator: Validators.requiredText('Naziv', maxLength: _maxNameLength),
          onSubmitted: (_) => _submit(),
        ),
      ),
    );
  }
}
