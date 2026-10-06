import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../models/category.dart';
import '../../providers/category_provider.dart';
import '../../utils/category_icons.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';
import '../../widgets/form_dialog.dart';

/// Create (no [category]) or edit a category. Resolves to a success message when saved.
class CategoryFormDialog extends StatefulWidget {
  const CategoryFormDialog({super.key, this.category});

  final Category? category;

  @override
  State<CategoryFormDialog> createState() => _CategoryFormDialogState();
}

class _CategoryFormDialogState extends State<CategoryFormDialog> {
  /// Same limits as the API CategoryInsertValidator.
  static const _minNameLength = 4;
  static const _maxNameLength = 50;

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  bool get _isEdit => widget.category != null;

  Future<void> _submit() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final request = {
      'name': (form.value['name'] as String).trim(),
      'iconName': form.value['iconName'],
      'isActive': form.value['isActive'] as bool,
    };

    setState(() => _isSaving = true);
    try {
      final provider = context.read<CategoryProvider>();
      final saved = _isEdit
          ? await provider.update(widget.category!.id, request)
          : await provider.insert(request);
      if (mounted) {
        Navigator.pop(
          context,
          _isEdit
              ? 'Izmjene kategorije "${saved.name}" su sačuvane.'
              : 'Kategorija "${saved.name}" je dodana.',
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
    final category = widget.category;

    return FormDialog(
      title: _isEdit ? 'Uređivanje kategorije' : 'Nova kategorija',
      submitLabel: _isEdit ? 'Sačuvaj izmjene' : 'Dodaj kategoriju',
      isSubmitting: _isSaving,
      onSubmit: _submit,
      child: FormBuilder(
        key: _formKey,
        initialValue: {
          'name': category?.name,
          'iconName': category?.iconName,
          'isActive': category?.isActive ?? true,
        },
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            FormBuilderTextField(
              name: 'name',
              autofocus: true,
              maxLength: _maxNameLength,
              decoration: const InputDecoration(labelText: 'Naziv *'),
              validator: Validators.requiredTextRange(
                'Naziv',
                minLength: _minNameLength,
                maxLength: _maxNameLength,
              ),
            ),
            const SizedBox(height: 8),
            FormBuilderDropdown<String?>(
              name: 'iconName',
              decoration: const InputDecoration(labelText: 'Ikona'),
              items: [
                const DropdownMenuItem<String?>(value: null, child: Text('Bez ikone')),
                for (final icon in categoryIcons)
                  DropdownMenuItem<String?>(
                    value: icon.name,
                    child: Row(
                      children: [
                        Icon(icon.icon, size: 20),
                        const SizedBox(width: 12),
                        Text(icon.label),
                      ],
                    ),
                  ),
              ],
            ),
            const SizedBox(height: 16),
            FormBuilderSwitch(
              name: 'isActive',
              title: const Text('Aktivna'),
              subtitle: const Text(
                'Neaktivna kategorija se ne nudi pri unosu novih destinacija.',
              ),
              decoration: const InputDecoration(border: InputBorder.none),
            ),
          ],
        ),
      ),
    );
  }
}
