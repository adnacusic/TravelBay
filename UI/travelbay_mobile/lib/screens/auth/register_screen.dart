import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../layouts/container_screen.dart';
import '../../layouts/master_screen.dart';
import '../../providers/auth_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';

/// New user account. The API assigns the User role itself; no role is sent.
class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  /// Same limits as the API UserInsertValidator.
  static const _maxNameLength = 50;
  static const _minUsernameLength = 3;
  static const _maxUsernameLength = 100;

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSubmitting = false;

  Future<void> _submit() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final values = form.value;
    final phone = (values['phoneNumber'] as String?)?.trim() ?? '';
    final request = {
      'firstName': (values['firstName'] as String).trim(),
      'lastName': (values['lastName'] as String).trim(),
      'email': (values['email'] as String).trim(),
      'username': (values['username'] as String).trim(),
      'password': values['password'] as String,
      'phoneNumber': phone.isEmpty ? null : phone,
    };

    setState(() => _isSubmitting = true);
    try {
      await context.read<AuthProvider>().register(request);
      if (mounted) {
        openSection(context, const ContainerScreen(welcomeMessage: 'Dobro došli u TravelBay!'));
      }
    } on Exception catch (e) {
      if (mounted) {
        showFormErrors(context, _formKey.currentState, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSubmitting = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      title: 'Registracija',
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: FormBuilder(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              FormBuilderTextField(
                name: 'firstName',
                maxLength: _maxNameLength,
                decoration: const InputDecoration(labelText: 'Ime *'),
                validator: Validators.requiredText('Ime', maxLength: _maxNameLength),
              ),
              const SizedBox(height: 8),
              FormBuilderTextField(
                name: 'lastName',
                maxLength: _maxNameLength,
                decoration: const InputDecoration(labelText: 'Prezime *'),
                validator: Validators.requiredText('Prezime', maxLength: _maxNameLength),
              ),
              const SizedBox(height: 8),
              FormBuilderTextField(
                name: 'email',
                keyboardType: TextInputType.emailAddress,
                decoration: const InputDecoration(labelText: 'Email *'),
                validator: Validators.email,
              ),
              const SizedBox(height: 16),
              FormBuilderTextField(
                name: 'username',
                maxLength: _maxUsernameLength,
                decoration: const InputDecoration(labelText: 'Korisničko ime *'),
                validator: Validators.requiredTextRange(
                  'Korisničko ime',
                  minLength: _minUsernameLength,
                  maxLength: _maxUsernameLength,
                ),
              ),
              const SizedBox(height: 8),
              FormBuilderTextField(
                name: 'phoneNumber',
                keyboardType: TextInputType.phone,
                decoration: const InputDecoration(
                  labelText: 'Telefon',
                  hintText: '+387 61 234 567',
                ),
                validator: Validators.optionalPhone,
              ),
              const SizedBox(height: 16),
              FormBuilderTextField(
                name: 'password',
                obscureText: true,
                decoration: InputDecoration(
                  labelText: 'Lozinka *',
                  helperText: 'Najmanje ${Validators.minPasswordLength} znakova.',
                ),
                validator: Validators.newPassword,
              ),
              const SizedBox(height: 16),
              FormBuilderTextField(
                name: 'confirmPassword',
                obscureText: true,
                decoration: const InputDecoration(labelText: 'Potvrda lozinke *'),
                validator: Validators.passwordConfirmation(
                  () => _formKey.currentState?.fields['password']?.value as String?,
                ),
              ),
              const SizedBox(height: 24),
              FilledButton(
                onPressed: _isSubmitting ? null : _submit,
                style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(50)),
                child: _isSubmitting
                    ? const SizedBox.square(
                        dimension: 20,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Text('Napravi račun'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
