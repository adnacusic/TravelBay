import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../models/city.dart';
import '../../models/country.dart';
import '../../providers/city_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';
import '../../widgets/form_dialog.dart';

/// Create (no [city]) or edit a city; the country comes from a dropdown of [countries].
/// Resolves to a success message when saved.
class CityFormDialog extends StatefulWidget {
  const CityFormDialog({
    super.key,
    this.city,
    required this.countries,
    this.initialCountryId,
  });

  final City? city;
  final List<Country> countries;
  final int? initialCountryId;

  @override
  State<CityFormDialog> createState() => _CityFormDialogState();
}

class _CityFormDialogState extends State<CityFormDialog> {
  static const _maxNameLength = 100;

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  bool get _isEdit => widget.city != null;

  Future<void> _submit() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final request = {
      'name': (form.value['name'] as String).trim(),
      'countryId': form.value['countryId'] as int,
    };

    setState(() => _isSaving = true);
    try {
      final provider = context.read<CityProvider>();
      final saved = _isEdit
          ? await provider.update(widget.city!.id, request)
          : await provider.insert(request);
      if (mounted) {
        Navigator.pop(
          context,
          _isEdit
              ? 'Izmjene grada "${saved.name}" su sačuvane.'
              : 'Grad "${saved.name}" je dodan (${saved.countryName}).',
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
      title: _isEdit ? 'Uređivanje grada' : 'Novi grad',
      submitLabel: _isEdit ? 'Sačuvaj izmjene' : 'Dodaj grad',
      isSubmitting: _isSaving,
      onSubmit: _submit,
      child: FormBuilder(
        key: _formKey,
        initialValue: {
          'name': widget.city?.name,
          'countryId': widget.city?.countryId ?? widget.initialCountryId,
        },
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            FormBuilderDropdown<int>(
              name: 'countryId',
              decoration: const InputDecoration(labelText: 'Država *'),
              items: [
                for (final country in widget.countries)
                  DropdownMenuItem(value: country.id, child: Text(country.name)),
              ],
              validator: Validators.requiredSelection<int>('državu'),
            ),
            const SizedBox(height: 20),
            FormBuilderTextField(
              name: 'name',
              autofocus: true,
              maxLength: _maxNameLength,
              decoration: const InputDecoration(labelText: 'Naziv *'),
              validator: Validators.requiredText('Naziv', maxLength: _maxNameLength),
              onSubmitted: (_) => _submit(),
            ),
          ],
        ),
      ),
    );
  }
}
