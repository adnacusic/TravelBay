import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../providers/auth_provider.dart';
import '../screens/dashboard_screen.dart';
import '../screens/destinations/destination_list_screen.dart';
import '../screens/login_screen.dart';
import '../utils/app_navigator.dart';
import '../utils/dialogs.dart';

enum AdminSection {
  dashboard,
  destinations,
  users,
  reviews,
  categories,
  referenceData,
  news,
  reports,
  aiAgents,
  profile,
}

class _MenuEntry {
  const _MenuEntry(
    this.section,
    this.label,
    this.icon, {
    this.builder,
    this.unavailableReason,
  });

  final AdminSection section;
  final String label;
  final IconData icon;

  /// Null while the module is not available; [unavailableReason] then explains why.
  final Widget Function()? builder;
  final String? unavailableReason;
}

const _inProgress = 'Modul je u izradi.';

final _menu = <_MenuEntry>[
  _MenuEntry(
    AdminSection.dashboard,
    'Dashboard',
    Icons.dashboard_outlined,
    builder: () => const DashboardScreen(),
  ),
  _MenuEntry(
    AdminSection.destinations,
    'Destinacije',
    Icons.place_outlined,
    builder: () => const DestinationListScreen(),
  ),
  const _MenuEntry(
    AdminSection.users,
    'Korisnici',
    Icons.people_outline,
    unavailableReason: _inProgress,
  ),
  const _MenuEntry(
    AdminSection.reviews,
    'Recenzije',
    Icons.rate_review_outlined,
    unavailableReason: _inProgress,
  ),
  const _MenuEntry(
    AdminSection.categories,
    'Kategorije',
    Icons.category_outlined,
    unavailableReason: _inProgress,
  ),
  const _MenuEntry(
    AdminSection.referenceData,
    'Države i gradovi',
    Icons.public_outlined,
    unavailableReason: _inProgress,
  ),
  const _MenuEntry(
    AdminSection.news,
    'Novosti',
    Icons.newspaper_outlined,
    unavailableReason: _inProgress,
  ),
  const _MenuEntry(
    AdminSection.reports,
    'Izvještaji',
    Icons.picture_as_pdf_outlined,
    unavailableReason: _inProgress,
  ),
  const _MenuEntry(
    AdminSection.aiAgents,
    'AI Agenti',
    Icons.smart_toy_outlined,
    unavailableReason:
        'Uskoro: AI agenti za ključne riječi i slike dolaze s AI worker servisom.',
  ),
  const _MenuEntry(
    AdminSection.profile,
    'Moj profil',
    Icons.account_circle_outlined,
    unavailableReason: _inProgress,
  ),
];

/// Admin shell: fixed side menu on the left, page title and content on the right.
class MasterScreen extends StatelessWidget {
  const MasterScreen({
    super.key,
    required this.section,
    required this.title,
    required this.child,
    this.actions = const [],
  });

  final AdminSection section;
  final String title;
  final Widget child;
  final List<Widget> actions;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Row(
        children: [
          _SideMenu(selected: section),
          const VerticalDivider(width: 1),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                _PageHeader(title: title, actions: actions),
                const Divider(height: 1),
                Expanded(child: child),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _PageHeader extends StatelessWidget {
  const _PageHeader({required this.title, required this.actions});

  final String title;
  final List<Widget> actions;

  @override
  Widget build(BuildContext context) {
    final canGoBack = Navigator.of(context).canPop();

    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 12, 24, 12),
      child: Row(
        children: [
          if (canGoBack) ...[
            TextButton.icon(
              onPressed: () => Navigator.of(context).maybePop(),
              icon: const Icon(Icons.arrow_back),
              label: const Text('Nazad'),
            ),
            const SizedBox(width: 8),
          ] else
            const SizedBox(width: 8),
          Expanded(
            child: Text(
              title,
              style: Theme.of(context).textTheme.headlineSmall,
              overflow: TextOverflow.ellipsis,
            ),
          ),
          ...actions,
        ],
      ),
    );
  }
}

class _SideMenu extends StatelessWidget {
  const _SideMenu({required this.selected});

  final AdminSection selected;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final displayName = context.watch<AuthProvider>().displayName;

    return Container(
      width: 248,
      color: scheme.surfaceContainerLow,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 24, 20, 16),
            child: Row(
              children: [
                Icon(Icons.travel_explore, color: scheme.primary, size: 30),
                const SizedBox(width: 10),
                Text(
                  'TravelBay Admin',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                ),
              ],
            ),
          ),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.symmetric(horizontal: 12),
              children: [for (final entry in _menu) _buildEntry(context, entry)],
            ),
          ),
          const Divider(height: 1),
          ListTile(
            leading: const Icon(Icons.person_outline),
            title: Text(displayName, overflow: TextOverflow.ellipsis),
            subtitle: const Text('Administrator'),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(12, 0, 12, 16),
            child: ListTile(
              leading: const Icon(Icons.logout),
              title: const Text('Odjava'),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
              ),
              onTap: () => _logout(context),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildEntry(BuildContext context, _MenuEntry entry) {
    final builder = entry.builder;
    final tile = ListTile(
      leading: Icon(entry.icon),
      title: Text(entry.label),
      enabled: builder != null,
      selected: entry.section == selected,
      selectedTileColor: Theme.of(context).colorScheme.secondaryContainer,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      trailing: builder == null
          ? Text('uskoro', style: Theme.of(context).textTheme.labelSmall)
          : null,
      onTap: builder == null || entry.section == selected
          ? null
          : () => openSection(context, builder()),
    );

    final reason = entry.unavailableReason;
    return reason == null ? tile : Tooltip(message: reason, child: tile);
  }

  Future<void> _logout(BuildContext context) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Odjava',
      message: 'Da li se želite odjaviti iz aplikacije?',
      confirmLabel: 'Odjavi se',
      destructive: false,
    );
    if (!confirmed || !context.mounted) {
      return;
    }

    context.read<AuthProvider>().logout();
    openSection(context, const LoginScreen());
  }
}
