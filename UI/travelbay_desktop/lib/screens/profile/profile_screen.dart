import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/user.dart';
import '../../providers/auth_provider.dart';
import '../../providers/user_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/form_errors.dart';
import '../../utils/formatters.dart';
import '../../utils/validators.dart';
import '../users/user_list_screen.dart';

/// The signed-in admin edits their own data (Users/Me) and changes their password.
class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  /// Same limits as the API UserProfileUpdateValidator.
  static const _maxNameLength = 50;
  static const _minUsernameLength = 3;
  static const _maxUsernameLength = 100;

  final _profileKey = GlobalKey<FormBuilderState>();
  final _passwordKey = GlobalKey<FormBuilderState>();

  User? _user;
  String? _loadError;
  bool _isSavingProfile = false;
  bool _isSavingPassword = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final user = await context.read<UserProvider>().getMe();
      if (mounted) {
        setState(() => _user = user);
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _loadError = e.toString());
      }
    }
  }

  Future<void> _saveProfile() async {
    final form = _profileKey.currentState!;
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

    setState(() => _isSavingProfile = true);
    try {
      final saved = await context.read<UserProvider>().updateMe(request);
      if (!mounted) {
        return;
      }
      context.read<AuthProvider>().updateDisplayName(saved.firstName, saved.lastName);
      setState(() => _user = saved);
      showSuccessMessage(context, 'Vaši podaci su sačuvani.');
    } on Exception catch (e) {
      if (mounted) {
        showFormErrors(context, _profileKey.currentState, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSavingProfile = false);
      }
    }
  }

  void _resetProfileForm() {
    final user = _user;
    if (user == null) {
      return;
    }
    _profileKey.currentState?.patchValue(_profileValues(user));
  }

  Future<void> _changePassword() async {
    final form = _passwordKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    setState(() => _isSavingPassword = true);
    try {
      await context.read<UserProvider>().changePassword(
            password: form.value['password'] as String,
            newPassword: form.value['newPassword'] as String,
            confirmNewPassword: form.value['confirmNewPassword'] as String,
          );
      if (!mounted) {
        return;
      }
      form.reset();
      showSuccessMessage(
        context,
        'Lozinka je promijenjena. Pri sljedećoj prijavi koristite novu lozinku.',
      );
    } on Exception catch (e) {
      if (mounted) {
        showFormErrors(context, _passwordKey.currentState, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSavingPassword = false);
      }
    }
  }

  Map<String, dynamic> _profileValues(User user) => {
        'firstName': user.firstName,
        'lastName': user.lastName,
        'username': user.username,
        'email': user.email,
        'phoneNumber': user.phoneNumber ?? '',
      };

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      section: AdminSection.profile,
      title: 'Moj profil',
      child: _buildBody(),
    );
  }

  Widget _buildBody() {
    final user = _user;
    if (_loadError != null) {
      return Center(child: Text(_loadError!));
    }
    if (user == null) {
      return const Center(child: CircularProgressIndicator());
    }

    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      child: Wrap(
        spacing: 24,
        runSpacing: 24,
        crossAxisAlignment: WrapCrossAlignment.start,
        children: [
          SizedBox(width: 560, child: _buildProfileCard(user)),
          SizedBox(width: 420, child: _buildPasswordCard()),
        ],
      ),
    );
  }

  Widget _buildProfileCard(User user) {
    final theme = Theme.of(context);

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: FormBuilder(
          key: _profileKey,
          initialValue: _profileValues(user),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text('Lični podaci', style: theme.textTheme.titleMedium),
              const SizedBox(height: 4),
              Text(
                '${roleLabel(user.role)} · član od ${formatDate(user.createdAt)}',
                style: theme.textTheme.bodySmall,
              ),
              const SizedBox(height: 20),
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: FormBuilderTextField(
                      name: 'firstName',
                      maxLength: _maxNameLength,
                      decoration: const InputDecoration(labelText: 'Ime *'),
                      validator: Validators.requiredText('Ime', maxLength: _maxNameLength),
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: FormBuilderTextField(
                      name: 'lastName',
                      maxLength: _maxNameLength,
                      decoration: const InputDecoration(labelText: 'Prezime *'),
                      validator: Validators.requiredText('Prezime', maxLength: _maxNameLength),
                    ),
                  ),
                ],
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
                decoration: const InputDecoration(labelText: 'Email *'),
                validator: Validators.email,
              ),
              const SizedBox(height: 20),
              FormBuilderTextField(
                name: 'phoneNumber',
                decoration: const InputDecoration(
                  labelText: 'Telefon',
                  hintText: '+387 61 234 567',
                ),
                validator: Validators.optionalPhone,
              ),
              const SizedBox(height: 24),
              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  OutlinedButton(
                    onPressed: _isSavingProfile ? null : _resetProfileForm,
                    child: const Text('Poništi izmjene'),
                  ),
                  const SizedBox(width: 12),
                  FilledButton.icon(
                    onPressed: _isSavingProfile ? null : _saveProfile,
                    icon: _isSavingProfile
                        ? const SizedBox.square(
                            dimension: 18,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                        : const Icon(Icons.save_outlined),
                    label: const Text('Sačuvaj podatke'),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildPasswordCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: FormBuilder(
          key: _passwordKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text('Promjena lozinke', style: Theme.of(context).textTheme.titleMedium),
              const SizedBox(height: 20),
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
                  () => _passwordKey.currentState?.fields['newPassword']?.value as String?,
                ),
              ),
              const SizedBox(height: 24),
              Align(
                alignment: Alignment.centerRight,
                child: FilledButton.icon(
                  onPressed: _isSavingPassword ? null : _changePassword,
                  icon: _isSavingPassword
                      ? const SizedBox.square(
                          dimension: 18,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        )
                      : const Icon(Icons.lock_reset),
                  label: const Text('Promijeni lozinku'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
