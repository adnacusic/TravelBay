import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../providers/auth_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';

/// Step 2 of "Zaboravljena lozinka": the emailed code and the new password.
/// Pops with a success message when the password was changed.
class ResetPasswordScreen extends StatefulWidget {
  const ResetPasswordScreen({super.key, required this.email, required this.sentMessage});

  final String email;

  /// The API's answer to the code request, shown at the top.
  final String sentMessage;

  @override
  State<ResetPasswordScreen> createState() => _ResetPasswordScreenState();
}

class _ResetPasswordScreenState extends State<ResetPasswordScreen> {
  /// Same as PasswordResetRules.CodeLength in the API.
  static const _codeLength = 6;
  static final _codePattern = RegExp('^[0-9]{$_codeLength}\$');

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;
  bool _isResending = false;

  static String? _validateCode(String? value) {
    final text = value?.trim() ?? '';
    if (text.isEmpty) {
      return 'Unesite kod iz emaila.';
    }
    return _codePattern.hasMatch(text) ? null : 'Kod ima tačno $_codeLength cifara, npr. 042913.';
  }

  Future<void> _save() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    setState(() => _isSaving = true);
    try {
      final message = await context.read<AuthProvider>().resetPassword(
            email: widget.email,
            code: (form.value['code'] as String).trim(),
            newPassword: form.value['newPassword'] as String,
            confirmNewPassword: form.value['confirmNewPassword'] as String,
          );
      if (mounted) {
        Navigator.pop(context, message.isEmpty ? 'Lozinka je promijenjena. Prijavite se novom lozinkom.' : message);
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

  Future<void> _resend() async {
    setState(() => _isResending = true);
    try {
      final message = await context.read<AuthProvider>().requestPasswordResetCode(widget.email);
      if (mounted) {
        showSuccessMessage(context, message.isEmpty ? 'Novi kod je poslan.' : message);
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isResending = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return MasterScreen(
      title: 'Nova lozinka',
      isForm: true,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: FormBuilder(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(widget.sentMessage, style: theme.textTheme.bodyMedium),
              const SizedBox(height: 4),
              Text(
                'Email: ${widget.email}. Kod vrijedi 15 minuta.',
                style: theme.textTheme.bodySmall,
              ),
              const SizedBox(height: 20),
              FormBuilderTextField(
                name: 'code',
                keyboardType: TextInputType.number,
                maxLength: _codeLength,
                autofocus: true,
                decoration: const InputDecoration(
                  labelText: 'Kod iz emaila *',
                  prefixIcon: Icon(Icons.pin_outlined),
                ),
                validator: _validateCode,
              ),
              const SizedBox(height: 8),
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
              const SizedBox(height: 8),
              TextButton(
                onPressed: _isResending || _isSaving ? null : _resend,
                child: const Text('Nije stigao kod? Pošalji ponovo'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
