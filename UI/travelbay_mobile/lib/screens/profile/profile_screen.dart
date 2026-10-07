import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/user.dart';
import '../../models/user_activity.dart';
import '../../providers/auth_provider.dart';
import '../../providers/user_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../utils/image_files.dart';
import '../../widgets/notification_bell.dart';
import '../auth/login_screen.dart';
import '../notifications/notifications_screen.dart';
import 'change_password_screen.dart';
import 'profile_edit_screen.dart';
import 'travel_preferences_screen.dart';
import 'view_history_screen.dart';

/// Profile tab: account, profile picture, activity counters and the account pages.
/// [activation] changes every time the tab is opened, so the counters are fresh.
class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key, required this.activation});

  final int activation;

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  User? _user;
  UserActivity? _activity;
  Uint8List? _image;
  String? _error;
  bool _isSavingImage = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  @override
  void didUpdateWidget(ProfileScreen oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.activation != widget.activation) {
      _load();
    }
  }

  Future<void> _load() async {
    final provider = context.read<UserProvider>();
    try {
      final userFuture = provider.getMe();
      final activityFuture = provider.getActivity();
      final user = await userFuture;
      final activity = await activityFuture;
      final image = await provider.profileImage(user);
      if (mounted) {
        setState(() {
          _user = user;
          _activity = activity;
          _image = image;
          _error = null;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  Future<void> _pickImage() async {
    final PickedImage? picked;
    try {
      picked = await pickImageFile();
    } on ImagePickException catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
      return;
    }
    if (picked == null || !mounted) {
      return;
    }

    setState(() => _isSavingImage = true);
    try {
      final user = await context.read<UserProvider>().setProfileImage(picked);
      if (mounted) {
        setState(() {
          _user = user;
          _image = picked!.bytes;
        });
        showSuccessMessage(context, 'Slika profila je promijenjena.');
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSavingImage = false);
      }
    }
  }

  Future<void> _removeImage() async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Uklanjanje slike',
      message: 'Ukloniti sliku profila? Umjesto nje prikazivat će se vaši inicijali.',
      confirmLabel: 'Ukloni',
    );
    if (!confirmed || !mounted) {
      return;
    }
    setState(() => _isSavingImage = true);
    try {
      final user = await context.read<UserProvider>().removeProfileImage();
      if (mounted) {
        setState(() {
          _user = user;
          _image = null;
        });
        showSuccessMessage(context, 'Slika profila je uklonjena.');
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSavingImage = false);
      }
    }
  }

  Future<void> _imageOptions() async {
    final hasImage = _image != null;
    final choice = await showModalBottomSheet<String>(
      context: context,
      showDragHandle: true,
      builder: (sheetContext) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ListTile(
              leading: const Icon(Icons.photo_library_outlined),
              title: Text(hasImage ? 'Promijeni sliku' : 'Dodaj sliku'),
              subtitle: const Text('JPG, PNG, GIF, WEBP ili BMP, najviše 5 MB'),
              onTap: () => Navigator.pop(sheetContext, 'pick'),
            ),
            if (hasImage)
              ListTile(
                leading: const Icon(Icons.delete_outline),
                title: const Text('Ukloni sliku'),
                onTap: () => Navigator.pop(sheetContext, 'remove'),
              ),
          ],
        ),
      ),
    );
    if (choice == 'pick') {
      await _pickImage();
    } else if (choice == 'remove') {
      await _removeImage();
    }
  }

  /// Sub-pages pop with a success message when they saved something.
  Future<void> _open(Widget page) async {
    final message = await openPage<String>(context, page);
    if (!mounted) {
      return;
    }
    if (message != null) {
      showSuccessMessage(context, message);
    }
    _load();
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
    return Scaffold(
      appBar: AppBar(
        title: const Text('Profil'),
        actions: const [NotificationBell()],
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    final user = _user;
    if (_error != null && user == null) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(_error!, textAlign: TextAlign.center),
              const SizedBox(height: 8),
              OutlinedButton(onPressed: _load, child: const Text('Pokušaj ponovo')),
            ],
          ),
        ),
      );
    }
    if (user == null) {
      return const Center(child: CircularProgressIndicator());
    }

    final theme = Theme.of(context);
    final activity = _activity;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Center(child: _buildAvatar(user)),
        const SizedBox(height: 12),
        Text(user.fullName, textAlign: TextAlign.center, style: theme.textTheme.titleLarge),
        Text(
          '@${user.username} · ${user.email}',
          textAlign: TextAlign.center,
          style: theme.textTheme.bodySmall,
        ),
        Text(
          'Član od ${formatDate(user.createdAt)}',
          textAlign: TextAlign.center,
          style: theme.textTheme.bodySmall,
        ),
        const SizedBox(height: 16),
        if (activity != null) _buildStats(activity),
        const SizedBox(height: 16),
        Card(
          child: Column(
            children: [
              _menuTile(
                Icons.edit_outlined,
                'Uredi profil',
                'Ime, prezime, korisničko ime, email, telefon',
                () => _open(ProfileEditScreen(user: user)),
              ),
              _menuTile(
                Icons.tune,
                'Preferencije putovanja',
                'Omiljene kategorije za preporuke',
                () => _open(const TravelPreferencesScreen()),
              ),
              _menuTile(
                Icons.history,
                'Historija pregleda',
                activity == null ? 'Destinacije koje ste otvarali' : 'Pregleda: ${activity.viewCount}',
                () => _open(const ViewHistoryScreen()),
              ),
              _menuTile(
                Icons.notifications_none,
                'Notifikacije',
                'Odluke o recenzijama i promjene planova',
                () => _open(const NotificationsScreen()),
              ),
              _menuTile(
                Icons.lock_reset,
                'Promjena lozinke',
                'Potrebna je trenutna lozinka',
                () => _open(const ChangePasswordScreen()),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),
        OutlinedButton.icon(
          onPressed: _logout,
          icon: const Icon(Icons.logout),
          label: const Text('Odjava'),
        ),
      ],
    );
  }

  Widget _buildAvatar(User user) {
    final theme = Theme.of(context);
    final image = _image;
    final initials = [user.firstName, user.lastName]
        .where((n) => n.isNotEmpty)
        .map((n) => n[0].toUpperCase())
        .join();

    return Stack(
      children: [
        CircleAvatar(
          radius: 48,
          backgroundImage: image == null ? null : MemoryImage(image),
          child: image == null
              ? Text(initials.isEmpty ? '?' : initials, style: theme.textTheme.headlineSmall)
              : null,
        ),
        Positioned(
          right: 0,
          bottom: 0,
          child: IconButton.filled(
            tooltip: 'Slika profila',
            onPressed: _isSavingImage ? null : _imageOptions,
            icon: _isSavingImage
                ? const SizedBox.square(
                    dimension: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.photo_camera_outlined, size: 20),
          ),
        ),
      ],
    );
  }

  Widget _buildStats(UserActivity activity) {
    return Row(
      children: [
        _StatBox(value: activity.completedTripPlanCount, label: 'Posjećeno'),
        _StatBox(value: activity.reviewCount, label: 'Recenzije'),
        _StatBox(value: activity.tripPlanCount, label: 'Planovi'),
        _StatBox(value: activity.savedDestinationCount, label: 'Sačuvano'),
      ],
    );
  }

  Widget _menuTile(IconData icon, String title, String subtitle, VoidCallback onTap) {
    return ListTile(
      leading: Icon(icon),
      title: Text(title),
      subtitle: Text(subtitle),
      trailing: const Icon(Icons.chevron_right),
      onTap: onTap,
    );
  }
}

class _StatBox extends StatelessWidget {
  const _StatBox({required this.value, required this.label});

  final int value;
  final String label;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Expanded(
      child: Card(
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 12),
          child: Column(
            children: [
              Text('$value', style: theme.textTheme.titleLarge),
              Text(label, style: theme.textTheme.bodySmall),
            ],
          ),
        ),
      ),
    );
  }
}
