import 'package:flutter/material.dart';

import '../../models/user.dart';
import '../../utils/formatters.dart';
import '../../widgets/scrollable_table.dart';
import 'user_status.dart';

/// User list table. Long text cells are capped and ellipsized; when the window is
/// too narrow for all columns the table scrolls horizontally instead of overflowing.
class UserTable extends StatelessWidget {
  const UserTable({
    super.key,
    required this.users,
    required this.currentUserId,
    required this.onOpenDetails,
    required this.onToggleActive,
  });

  static const _nameWidth = 180.0;
  static const _usernameWidth = 140.0;
  static const _emailWidth = 220.0;

  final List<User> users;

  /// The signed-in admin, who cannot deactivate their own account.
  final int? currentUserId;
  final ValueChanged<User> onOpenDetails;
  final ValueChanged<User> onToggleActive;

  @override
  Widget build(BuildContext context) {
    return ScrollableTable(
      child: DataTable(
        showCheckboxColumn: false,
        columnSpacing: 24,
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
          for (final user in users)
            DataRow(
              onSelectChanged: (_) => onOpenDetails(user),
              cells: [
                DataCell(_capped(user.fullName, _nameWidth)),
                DataCell(_capped(user.username, _usernameWidth)),
                DataCell(_capped(user.email, _emailWidth)),
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
                        onPressed: () => onOpenDetails(user),
                      ),
                      _toggleButton(context, user),
                    ],
                  ),
                ),
              ],
            ),
        ],
      ),
    );
  }

  /// Full text stays available in a tooltip.
  Widget _capped(String text, double maxWidth) {
    return Tooltip(
      message: text,
      child: ConstrainedBox(
        constraints: BoxConstraints(maxWidth: maxWidth),
        child: Text(text, overflow: TextOverflow.ellipsis, maxLines: 1),
      ),
    );
  }

  Widget _toggleButton(BuildContext context, User user) {
    if (!user.isActive) {
      return IconButton(
        tooltip: 'Aktiviraj račun',
        icon: const Icon(Icons.person_add_alt_1_outlined),
        onPressed: () => onToggleActive(user),
      );
    }

    final isSelf = user.id == currentUserId;
    return Tooltip(
      message: isSelf ? selfDeactivationReason : 'Deaktiviraj račun',
      child: IconButton(
        icon: Icon(
          Icons.person_off_outlined,
          color: isSelf ? null : Theme.of(context).colorScheme.error,
        ),
        onPressed: isSelf ? null : () => onToggleActive(user),
      ),
    );
  }
}
