import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../models/user.dart';
import '../../providers/user_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';
import '../../widgets/form_dialog.dart';

/// Admin sets a new password for another user. Resolves to true when it was changed.
class ResetPasswordDialog extends StatefulWidget {
  const ResetPasswordDialog({super.key, required this.user});

  final User user;

  @override
  State<ResetPasswordDialog> createState() => _ResetPasswordDialogState();
}

class _ResetPasswordDialogState extends State<ResetPasswordDialog> {
  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  Future<void> _submit() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    setState(() => _isSaving = true);
    try {
      await context.read<UserProvider>().resetPassword(
            widget.user.id,
            newPassword: form.value['newPassword'] as String,
            confirmNewPassword: form.value['confirmNewPassword'] as String,
          );
      if (mounted) {
        Navigator.pop(context, true);
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
      title: 'Nova lozinka za ${widget.user.fullName}',
      submitLabel: 'Postavi lozinku',
      isSubmitting: _isSaving,
      onSubmit: _submit,
      child: FormBuilder(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              'Korisnik se nakon ovoga prijavljuje novom lozinkom. '
              'Javite mu je sigurnim kanalom.',
              style: Theme.of(context).textTheme.bodySmall,
            ),
            const SizedBox(height: 16),
            FormBuilderTextField(
              name: 'newPassword',
              obscureText: true,
              autofocus: true,
              decoration: const InputDecoration(labelText: 'Nova lozinka *'),
              validator: Validators.newPassword,
            ),
            const SizedBox(height: 16),
            FormBuilderTextField(
              name: 'confirmNewPassword',
              obscureText: true,
              decoration: const InputDecoration(labelText: 'Potvrda nove lozinke *'),
              validator: Validators.passwordConfirmation(
                () => _formKey.currentState?.fields['newPassword']?.value as String?,
              ),
              onSubmitted: (_) => _submit(),
            ),
          ],
        ),
      ),
    );
  }
}
