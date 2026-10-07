import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/user.dart';
import '../../providers/auth_provider.dart';
import '../../providers/user_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';

/// The signed-in user edits their own data (Users/Me). Pops with a success message.
class ProfileEditScreen extends StatefulWidget {
  const ProfileEditScreen({super.key, required this.user});

  final User user;

  @override
  State<ProfileEditScreen> createState() => _ProfileEditScreenState();
}

class _ProfileEditScreenState extends State<ProfileEditScreen> {
  /// Same limits as the API UserProfileUpdateValidator.
  static const _maxNameLength = 50;
  static const _minUsernameLength = 3;
  static const _maxUsernameLength = 100;

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  Future<void> _save() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final values = form.value;
    final phone = (values['phoneNumber'] as String?)?.trim() ?? '';
    final request = {
      'firstName': (values['firstName'] as String).trim(),
      'lastName': (values['lastName'] as String).trim(),
      'username': (values['username'] as String).trim(),
      'email': (values['email'] as String).trim(),
      'phoneNumber': phone.isEmpty ? null : phone,
    };

    setState(() => _isSaving = true);
    try {
      final saved = await context.read<UserProvider>().updateMe(request);
      if (!mounted) {
        return;
      }
      context.read<AuthProvider>().updateDisplayName(saved.firstName, saved.lastName);
      Navigator.pop(context, 'Vaši podaci su sačuvani.');
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
    final user = widget.user;

    return MasterScreen(
      title: 'Uredi profil',
      isForm: true,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: FormBuilder(
          key: _formKey,
          initialValue: {
            'firstName': user.firstName,
            'lastName': user.lastName,
            'username': user.username,
            'email': user.email,
            'phoneNumber': user.phoneNumber ?? '',
          },
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              FormBuilderTextField(
                name: 'firstName',
                maxLength: _maxNameLength,
                textCapitalization: TextCapitalization.words,
                decoration: const InputDecoration(labelText: 'Ime *'),
                validator: Validators.requiredText('Ime', maxLength: _maxNameLength),
              ),
              const SizedBox(height: 8),
              FormBuilderTextField(
                name: 'lastName',
                maxLength: _maxNameLength,
                textCapitalization: TextCapitalization.words,
                decoration: const InputDecoration(labelText: 'Prezime *'),
                validator: Validators.requiredText('Prezime', maxLength: _maxNameLength),
              ),
              const SizedBox(height: 8),
              FormBuilderTextField(
                name: 'username',
                maxLength: _maxUsernameLength,
                decoration: const InputDecoration(
                  labelText: 'Korisničko ime *',
                  helperText: 'Koristi se za prijavu.',
                ),
                validator: Validators.requiredTextRange(
                  'Korisničko ime',
                  minLength: _minUsernameLength,
                  maxLength: _maxUsernameLength,
                ),
              ),
              const SizedBox(height: 8),
              FormBuilderTextField(
                name: 'email',
                keyboardType: TextInputType.emailAddress,
                decoration: const InputDecoration(labelText: 'Email *'),
                validator: Validators.email,
              ),
              const SizedBox(height: 20),
              FormBuilderTextField(
                name: 'phoneNumber',
                keyboardType: TextInputType.phone,
                decoration: const InputDecoration(
                  labelText: 'Telefon',
                  hintText: '+387 61 234 567',
                ),
                validator: Validators.optionalPhone,
              ),
              const SizedBox(height: 24),
              FilledButton.icon(
                onPressed: _isSaving ? null : _save,
                icon: _isSaving
                    ? const SizedBox.square(
                        dimension: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.save_outlined),
                label: const Text('Sačuvaj izmjene'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
