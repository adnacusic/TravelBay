import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/user.dart';
import '../../providers/auth_provider.dart';
import '../../providers/user_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../auth/login_screen.dart';

/// Profile tab. Step 1 shows the signed-in account and sign-out; editing,
/// preferences, history and statistics follow in the next step.
class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  User? _user;
  String? _error;

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
        setState(() => _error = e.toString());
      }
    }
  }

  Future<void> _logout() async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Odjava',
      message: 'Da li se želite odjaviti?',
      confirmLabel: 'Odjavi se',
      destructive: false,
    );
    if (!confirmed || !mounted) {
      return;
    }
    context.read<AuthProvider>().logout();
    openSection(context, const LoginScreen());
  }

  @override
  Widget build(BuildContext context) {
    final user = _user;
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Profil')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          if (_error != null)
            Text(_error!)
          else if (user == null)
            const Center(child: CircularProgressIndicator())
          else
            Card(
              child: ListTile(
                leading: CircleAvatar(
                  child: Text(user.firstName.isEmpty ? '?' : user.firstName[0]),
                ),
                title: Text(user.fullName),
                subtitle: Text('${user.username} · ${user.email}'),
              ),
            ),
          const SizedBox(height: 16),
          Text(
            'Uređivanje profila, preferencije putovanja, historija pregleda i promjena '
            'lozinke stižu u sljedećem koraku.',
            style: theme.textTheme.bodyMedium,
          ),
          const SizedBox(height: 24),
          OutlinedButton.icon(
            onPressed: _logout,
            icon: const Icon(Icons.logout),
            label: const Text('Odjava'),
          ),
        ],
      ),
    );
  }
}
