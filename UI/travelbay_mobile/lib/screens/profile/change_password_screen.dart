import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../providers/user_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';

/// Own password change; the current password is checked by the API. Pops with a success message.
class ChangePasswordScreen extends StatefulWidget {
  const ChangePasswordScreen({super.key});

  @override
  State<ChangePasswordScreen> createState() => _ChangePasswordScreenState();
}

class _ChangePasswordScreenState extends State<ChangePasswordScreen> {
  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  Future<void> _save() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    setState(() => _isSaving = true);
    try {
      await context.read<UserProvider>().changePassword(
            password: form.value['password'] as String,
            newPassword: form.value['newPassword'] as String,
            confirmNewPassword: form.value['confirmNewPassword'] as String,
          );
      if (mounted) {
        Navigator.pop(
          context,
          'Lozinka je promijenjena. Pri sljedećoj prijavi koristite novu lozinku.',
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
    return MasterScreen(
      title: 'Promjena lozinke',
      isForm: true,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: FormBuilder(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              FormBuilderTextField(
                name: 'password',
                obscureText: true,
                decoration: const InputDecoration(labelText: 'Trenutna lozinka *'),
                validator: Validators.requiredText('Trenutna lozinka'),
              ),
              const SizedBox(height: 16),
              FormBuilderTextField(
                name: 'newPassword',
                obscureText: true,
                decoration: InputDecoration(
                  labelText: 'Nova lozinka *',
                  helperText: 'Najmanje ${Validators.minPasswordLength} znakova.',
                ),
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
              ),
              const SizedBox(height: 24),
              FilledButton.icon(
                onPressed: _isSaving ? null : _save,
                icon: _isSaving
                    ? const SizedBox.square(
                        dimension: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.lock_reset),
                label: const Text('Promijeni lozinku'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
