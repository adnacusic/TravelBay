import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/user.dart';
import '../../models/user_stats.dart';
import '../../providers/auth_provider.dart';
import '../../providers/user_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/stat_card.dart';
import 'reset_password_dialog.dart';
import 'user_list_screen.dart';
import 'user_status.dart';

/// Read-only profile and activity of one user, with account actions.
/// Pops with true when the account was changed, so the list reloads.
class UserDetailsScreen extends StatefulWidget {
  const UserDetailsScreen({super.key, required this.userId});

  final int userId;

  @override
  State<UserDetailsScreen> createState() => _UserDetailsScreenState();
}

class _UserDetailsScreenState extends State<UserDetailsScreen> {
  User? _user;
  UserActivity? _activity;
  bool _changed = false;
  String? _loadError;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final provider = context.read<UserProvider>();
      final userFuture = provider.getById(widget.userId);
      final activityFuture = provider.getActivity(widget.userId);
      final user = await userFuture;
      final activity = await activityFuture;
      if (mounted) {
        setState(() {
          _user = user;
          _activity = activity;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _loadError = e.toString());
      }
    }
  }

  Future<void> _toggleActive(User user) async {
    final updated = await toggleUserActive(context, user);
    if (updated != null && mounted) {
      setState(() {
        _user = updated;
        _changed = true;
      });
    }
  }

  Future<void> _resetPassword(User user) async {
    final changed = await showDialog<bool>(
      context: context,
      builder: (context) => ResetPasswordDialog(user: user),
    );
    if (changed == true && mounted) {
      showSuccessMessage(context, 'Lozinka korisnika ${user.fullName} je promijenjena.');
    }
  }

  void _close() => Navigator.pop(context, _changed);

  @override
  Widget build(BuildContext context) {
    return PopScope<bool>(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          _close();
        }
      },
      child: MasterScreen(
        section: AdminSection.users,
        title: 'Detalji korisnika',
        child: _buildBody(),
      ),
    );
  }

  Widget _buildBody() {
    final user = _user;
    final activity = _activity;
    if (_loadError != null) {
      return Center(child: Text(_loadError!));
    }
    if (user == null || activity == null) {
      return const Center(child: CircularProgressIndicator());
    }

    final theme = Theme.of(context);
    final isSelf = user.id == context.read<AuthProvider>().currentUserId;

    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      child: Align(
        alignment: Alignment.topCenter,
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 1100),
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      CircleAvatar(
                        radius: 28,
                        child: Text(
                          _initials(user),
                          style: theme.textTheme.titleLarge,
                        ),
                      ),
                      const SizedBox(width: 16),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(user.fullName, style: theme.textTheme.titleLarge),
                            const SizedBox(height: 6),
                            Row(
                              children: [
                                UserStatusChip(isActive: user.isActive),
                                const SizedBox(width: 8),
                                Text(roleLabel(user.role)),
                              ],
                            ),
                          ],
                        ),
                      ),
                      IconButton(
                        tooltip: 'Zatvori',
                        icon: const Icon(Icons.close),
                        onPressed: _close,
                      ),
                    ],
                  ),
                  const SizedBox(height: 24),
                  Wrap(
                    spacing: 48,
                    runSpacing: 16,
                    children: [
                      _info('Korisničko ime', user.username),
                      _info('Email', user.email),
                      _info('Telefon', user.phoneNumber ?? '-'),
                      _info('Registrovan', formatDate(user.createdAt)),
                      _info('Zadnja prijava', formatDateTime(user.lastLoginAt)),
                    ],
                  ),
                  const SizedBox(height: 24),
                  Text('Aktivnost', style: theme.textTheme.titleMedium),
                  const SizedBox(height: 12),
                  Wrap(
                    spacing: 16,
                    runSpacing: 16,
                    children: [
                      StatCard(
                        icon: Icons.rate_review_outlined,
                        label: 'Recenzije',
                        value: '${activity.reviewCount}',
                        detail: 'na čekanju ${activity.pendingReviewCount} · '
                            'odobrene ${activity.approvedReviewCount} · '
                            'odbijene ${activity.rejectedReviewCount}',
                        width: 320,
                      ),
                      StatCard(
                        icon: Icons.map_outlined,
                        label: 'Planovi putovanja',
                        value: '${activity.tripPlanCount}',
                        width: 220,
                      ),
                      StatCard(
                        icon: Icons.collections_bookmark_outlined,
                        label: 'Kolekcije',
                        value: '${activity.collectionCount}',
                        width: 220,
                      ),
                      StatCard(
                        icon: Icons.bookmark_border,
                        label: 'Sačuvane destinacije',
                        value: '${activity.savedDestinationCount}',
                        width: 220,
                      ),
                      StatCard(
                        icon: Icons.visibility_outlined,
                        label: 'Pregledi destinacija',
                        value: '${activity.viewCount}',
                        width: 220,
                      ),
                    ],
                  ),
                  const SizedBox(height: 24),
                  const Divider(),
                  const SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      OutlinedButton(onPressed: _close, child: const Text('Nazad')),
                      const SizedBox(width: 12),
                      OutlinedButton.icon(
                        onPressed: () => _resetPassword(user),
                        icon: const Icon(Icons.lock_reset),
                        label: const Text('Postavi novu lozinku'),
                      ),
                      const SizedBox(width: 12),
                      _buildToggleButton(user, isSelf: isSelf),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildToggleButton(User user, {required bool isSelf}) {
    if (!user.isActive) {
      return FilledButton.icon(
        onPressed: () => _toggleActive(user),
        icon: const Icon(Icons.person_add_alt_1_outlined),
        label: const Text('Aktiviraj račun'),
      );
    }

    final button = FilledButton.icon(
      style: FilledButton.styleFrom(
        backgroundColor: Theme.of(context).colorScheme.error,
      ),
      onPressed: isSelf ? null : () => _toggleActive(user),
      icon: const Icon(Icons.person_off_outlined),
      label: const Text('Deaktiviraj račun'),
    );
    return isSelf ? Tooltip(message: selfDeactivationReason, child: button) : button;
  }

  Widget _info(String label, String value) {
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: theme.textTheme.bodySmall),
        const SizedBox(height: 2),
        Text(value, style: theme.textTheme.bodyLarge),
      ],
    );
  }

  String _initials(User user) {
    final first = user.firstName.isEmpty ? '' : user.firstName[0];
    final last = user.lastName.isEmpty ? '' : user.lastName[0];
    return '$first$last'.toUpperCase();
  }
}
