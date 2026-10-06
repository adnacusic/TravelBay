import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/user.dart';
import '../../models/user_stats.dart';
import '../../providers/auth_provider.dart';
import '../../providers/user_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../utils/jwt_claims.dart';
import '../../widgets/pagination_bar.dart';
import '../../widgets/search_field.dart';
import '../../widgets/stat_card.dart';
import 'user_details_screen.dart';
import 'user_status.dart';

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

    final currentUserId = context.read<AuthProvider>().currentUserId;

    return Card(
      clipBehavior: Clip.antiAlias,
      child: Stack(
        children: [
          SingleChildScrollView(
            child: SizedBox(
              width: double.infinity,
              child: DataTable(
                showCheckboxColumn: false,
                columns: const [
                  DataColumn(label: Text('Ime i prezime')),
                  DataColumn(label: Text('Korisničko ime')),
                  DataColumn(label: Text('Email')),
                  DataColumn(label: Text('Uloga')),
                  DataColumn(label: Text('Registrovan')),
                  DataColumn(label: Text('Zadnja prijava')),
                  DataColumn(label: Text('Status')),
                  DataColumn(label: Text('Akcije')),
                ],
                rows: [
                  for (final user in _users)
                    DataRow(
                      onSelectChanged: (_) => _openDetails(user),
                      cells: [
                        DataCell(Text(user.fullName)),
                        DataCell(Text(user.username)),
                        DataCell(Text(user.email)),
                        DataCell(Text(roleLabel(user.role))),
                        DataCell(Text(formatDate(user.createdAt))),
                        DataCell(Text(formatDateTime(user.lastLoginAt))),
                        DataCell(UserStatusChip(isActive: user.isActive)),
                        DataCell(
                          Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              IconButton(
                                tooltip: 'Detalji',
                                icon: const Icon(Icons.visibility_outlined),
                                onPressed: () => _openDetails(user),
                              ),
                              _buildToggleButton(user, isSelf: user.id == currentUserId),
                            ],
                          ),
                        ),
                      ],
                    ),
                ],
              ),
            ),
          ),
          if (_isLoading) const LinearProgressIndicator(),
        ],
      ),
    );
  }

  Widget _buildToggleButton(User user, {required bool isSelf}) {
    if (user.isActive) {
      final disabled = isSelf;
      return Tooltip(
        message: disabled ? selfDeactivationReason : 'Deaktiviraj račun',
        child: IconButton(
          icon: Icon(
            Icons.person_off_outlined,
            color: disabled ? null : Theme.of(context).colorScheme.error,
          ),
          onPressed: disabled ? null : () => _toggleActive(user),
        ),
      );
    }
    return IconButton(
      tooltip: 'Aktiviraj račun',
      icon: const Icon(Icons.person_add_alt_1_outlined),
      onPressed: () => _toggleActive(user),
    );
  }
}

String roleLabel(String? role) => switch (role) {
      RoleNames.admin => 'Administrator',
      RoleNames.user => 'Korisnik',
      _ => '-',
    };
