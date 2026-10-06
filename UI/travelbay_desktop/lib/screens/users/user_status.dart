import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/user.dart';
import '../../providers/user_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/jwt_claims.dart';
import '../../widgets/tone_chip.dart';

class UserStatusChip extends StatelessWidget {
  const UserStatusChip({super.key, required this.isActive});

  final bool isActive;

  @override
  Widget build(BuildContext context) {
    return ToneChip(
      label: isActive ? 'Aktivan' : 'Neaktivan',
      tone: isActive ? ChipTone.positive : ChipTone.negative,
    );
  }
}

const selfDeactivationReason = 'Ne možete deaktivirati vlastiti račun.';

/// Confirms and flips the account status. Returns the updated user, or null when
/// the admin cancelled or the call failed (the error is already shown).
Future<User?> toggleUserActive(BuildContext context, User user) async {
  final activate = !user.isActive;
  final confirmed = await showConfirmDialog(
    context,
    title: activate ? 'Aktivacija računa' : 'Deaktivacija računa',
    message: activate
        ? 'Korisnik ${user.fullName} će se ponovo moći prijaviti u aplikaciju.'
        : 'Korisnik ${user.fullName} se više neće moći prijaviti. '
            'Njegove recenzije i planovi ostaju sačuvani, a račun možete kasnije ponovo aktivirati.',
    confirmLabel: activate ? 'Aktiviraj' : 'Deaktiviraj',
    destructive: !activate,
  );
  if (!confirmed || !context.mounted) {
    return null;
  }

  try {
    final provider = context.read<UserProvider>();
    final updated =
        activate ? await provider.activate(user.id) : await provider.deactivate(user.id);
    if (context.mounted) {
      showSuccessMessage(
        context,
        activate
            ? 'Račun korisnika ${user.fullName} je aktiviran.'
            : 'Račun korisnika ${user.fullName} je deaktiviran.',
      );
    }
    return updated;
  } on Exception catch (e) {
    if (context.mounted) {
      await showErrorDialog(context, e);
    }
    return null;
  }
}

String roleLabel(String? role) => switch (role) {
      RoleNames.admin => 'Administrator',
      RoleNames.user => 'Korisnik',
      _ => '-',
    };
