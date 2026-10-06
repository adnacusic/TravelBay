import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/user.dart';
import '../../models/user_stats.dart';
import '../../providers/auth_provider.dart';
import '../../providers/user_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../widgets/pagination_bar.dart';
import '../../widgets/search_field.dart';
import '../../widgets/stat_card.dart';
import 'user_details_screen.dart';
import 'user_status.dart';
import 'user_table.dart';

class UserListScreen extends StatefulWidget {
  const UserListScreen({super.key});

  @override
  State<UserListScreen> createState() => _UserListScreenState();
}

class _UserListScreenState extends State<UserListScreen> {
  static const _pageSize = 10;

  final _searchController = TextEditingController();
  bool? _statusFilter;

  UserStats? _stats;
  List<User> _users = [];
  int _totalCount = 0;
  int _page = 1;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadStats();
    _loadPage(1);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadStats() async {
    try {
      final stats = await context.read<UserProvider>().getStats();
      if (mounted) {
        setState(() => _stats = stats);
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _loadPage(int page) async {
    setState(() => _isLoading = true);
    try {
      final result = await context.read<UserProvider>().get(filter: {
        'searchText': _searchController.text.trim(),
        'isActive': _statusFilter,
        'includeTotalCount': true,
        'sortBy': 'LastName, FirstName',
        'page': page,
        'pageSize': _pageSize,
      });
      if (mounted) {
        setState(() {
          _users = result.items;
          _totalCount = result.totalCount ?? result.items.length;
          _page = page;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  void _reload() {
    _loadStats();
    _loadPage(_page);
  }

  void _clearFilters() {
    _searchController.clear();
    setState(() => _statusFilter = null);
    _loadPage(1);
  }

  Future<void> _openDetails(User user) async {
    final changed = await openPage<bool>(context, UserDetailsScreen(userId: user.id));
    if (changed == true && mounted) {
      _reload();
    }
  }

  Future<void> _toggleActive(User user) async {
    final updated = await toggleUserActive(context, user);
    if (updated != null && mounted) {
      _reload();
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      section: AdminSection.users,
      title: 'Upravljanje korisnicima',
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _buildStats(),
            const SizedBox(height: 16),
            _buildFilters(),
            const SizedBox(height: 16),
            Expanded(child: _buildTable()),
            const SizedBox(height: 8),
            PaginationBar(
              page: _page,
              pageSize: _pageSize,
              totalCount: _totalCount,
              onPageChanged: _loadPage,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildStats() {
    final stats = _stats;
    if (stats == null) {
      return const SizedBox(height: 4);
    }
    return Wrap(
      spacing: 16,
      runSpacing: 16,
      children: [
        StatCard(
          icon: Icons.people_outline,
          label: 'Ukupno korisnika',
          value: '${stats.totalUsers}',
          width: 220,
        ),
        StatCard(
          icon: Icons.check_circle_outline,
          label: 'Aktivni',
          value: '${stats.activeUsers}',
          width: 220,
        ),
        StatCard(
          icon: Icons.block,
          label: 'Neaktivni',
          value: '${stats.inactiveUsers}',
          width: 220,
        ),
        StatCard(
          icon: Icons.admin_panel_settings_outlined,
          label: 'Administratori',
          value: '${stats.administrators}',
          width: 220,
        ),
        StatCard(
          icon: Icons.person_add_alt,
          label: 'Novi korisnici',
          value: '${stats.newUsers}',
          detail: 'u zadnjih ${stats.newUsersPeriodDays} dana',
          width: 220,
        ),
      ],
    );
  }

  Widget _buildFilters() {
    return Row(
      children: [
        SearchField(
          label: 'Ime, prezime, korisničko ime ili email',
          controller: _searchController,
          width: 380,
          onSearch: (_) => _loadPage(1),
        ),
        const SizedBox(width: 16),
        SizedBox(
          width: 200,
          child: DropdownButtonFormField<bool?>(
            key: ValueKey(_statusFilter),
            initialValue: _statusFilter,
            decoration: const InputDecoration(labelText: 'Status'),
            items: const [
              DropdownMenuItem(value: null, child: Text('Svi')),
              DropdownMenuItem(value: true, child: Text('Aktivni')),
              DropdownMenuItem(value: false, child: Text('Neaktivni')),
            ],
            onChanged: (value) {
              setState(() => _statusFilter = value);
              _loadPage(1);
            },
          ),
        ),
        const SizedBox(width: 16),
        TextButton.icon(
          onPressed: _clearFilters,
          icon: const Icon(Icons.filter_alt_off_outlined),
          label: const Text('Očisti filtere'),
        ),
      ],
    );
  }

  Widget _buildTable() {
    if (_isLoading && _users.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_users.isEmpty) {
      return const Center(
        child: Text('Nema korisnika koji odgovaraju zadanim filterima.'),
      );
    }

    return Card(
      clipBehavior: Clip.antiAlias,
      child: Stack(
        children: [
          SingleChildScrollView(
            child: UserTable(
              users: _users,
              currentUserId: context.read<AuthProvider>().currentUserId,
              onOpenDetails: _openDetails,
              onToggleActive: _toggleActive,
            ),
          ),
          if (_isLoading) const LinearProgressIndicator(),
        ],
      ),
    );
  }
}
