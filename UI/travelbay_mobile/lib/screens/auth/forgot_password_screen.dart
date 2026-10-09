import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../providers/auth_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';
import 'reset_password_screen.dart';

/// Step 1 of "Zaboravljena lozinka": the account's email; the API emails a 6-digit code.
/// Pops with a success message once the password was reset in step 2.
class ForgotPasswordScreen extends StatefulWidget {
  const ForgotPasswordScreen({super.key});

  @override
  State<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSending = false;

  Future<void> _send() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final email = (form.value['email'] as String).trim();
    setState(() => _isSending = true);
    try {
      final message = await context.read<AuthProvider>().requestPasswordResetCode(email);
      if (!mounted) {
        return;
      }
      final result = await openPage<String>(
        context,
        ResetPasswordScreen(email: email, sentMessage: message),
      );
      if (result != null && mounted) {
        Navigator.pop(context, result);
      }
    } on Exception catch (e) {
      if (mounted) {
        showFormErrors(context, _formKey.currentState, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSending = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      title: 'Zaboravljena lozinka',
      isForm: true,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: FormBuilder(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                'Unesite email svog računa. Poslat ćemo vam kod od 6 cifara kojim postavljate '
                'novu lozinku.',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              const SizedBox(height: 20),
              FormBuilderTextField(
                name: 'email',
                keyboardType: TextInputType.emailAddress,
                autofocus: true,
                decoration: const InputDecoration(
                  labelText: 'Email *',
                  prefixIcon: Icon(Icons.email_outlined),
                ),
                validator: Validators.email,
                onSubmitted: (_) => _send(),
              ),
              const SizedBox(height: 24),
              FilledButton.icon(
                onPressed: _isSending ? null : _send,
                icon: _isSending
                    ? const SizedBox.square(
                        dimension: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.send_outlined),
                label: const Text('Pošalji kod'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
