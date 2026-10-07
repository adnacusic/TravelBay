import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/category.dart';
import '../../providers/category_provider.dart';
import '../../providers/user_activity_provider.dart';
import '../../utils/category_icons.dart';
import '../../utils/dialogs.dart';

/// Preferred categories (UserPreferences). They are the strongest recommender signal,
/// so saving them changes the recommendations on the home screen. Pops with a success message.
class TravelPreferencesScreen extends StatefulWidget {
  const TravelPreferencesScreen({super.key});

  @override
  State<TravelPreferencesScreen> createState() => _TravelPreferencesScreenState();
}

class _TravelPreferencesScreenState extends State<TravelPreferencesScreen> {
  static const _categoryListSize = 100;

  List<Category>? _categories;
  final Set<int> _selected = {};
  String? _error;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final categoriesFuture = context
          .read<CategoryProvider>()
          .get(filter: {'pageSize': _categoryListSize, 'sortBy': 'Name'});
      final preferencesFuture = context.read<UserPreferenceProvider>().getAll();
      final categories = await categoriesFuture;
      final preferences = await preferencesFuture;
      if (mounted) {
        setState(() {
          final preferredIds = preferences.map((p) => p.categoryId).toSet();
          // An inactive category stays visible only if it is already chosen, so it can be removed.
          _categories = categories.items
              .where((c) => c.isActive || preferredIds.contains(c.id))
              .toList();
          _selected
            ..clear()
            ..addAll(preferredIds);
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  Future<void> _save() async {
    setState(() => _isSaving = true);
    try {
      await context.read<UserPreferenceProvider>().setCategories(_selected.toList());
      if (mounted) {
        Navigator.pop(
          context,
          _selected.isEmpty
              ? 'Preferencije su uklonjene. Preporuke se sada temelje na ocjenama i popularnosti.'
              : 'Preferencije su sačuvane. Preporuke na početnoj stranici ih već uzimaju u obzir.',
        );
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSaving = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      title: 'Preferencije putovanja',
      isForm: true,
      child: _buildBody(),
    );
  }

  Widget _buildBody() {
    final categories = _categories;
    if (_error != null) {
      return Center(child: Padding(padding: const EdgeInsets.all(24), child: Text(_error!)));
    }
    if (categories == null) {
      return const Center(child: CircularProgressIndicator());
    }

    final theme = Theme.of(context);
    return Column(
      children: [
        Expanded(
          child: ListView(
            padding: const EdgeInsets.symmetric(vertical: 8),
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(16, 8, 16, 8),
                child: Text(
                  'Označite vrste destinacija koje volite. Destinacije iz tih kategorija '
                  'dobijaju prednost u preporukama.',
                  style: theme.textTheme.bodyMedium,
                ),
              ),
              for (final category in categories)
                CheckboxListTile(
                  value: _selected.contains(category.id),
                  onChanged: _isSaving
                      ? null
                      : (checked) => setState(() {
                            if (checked == true) {
                              _selected.add(category.id);
                            } else {
                              _selected.remove(category.id);
                            }
                          }),
                  secondary: Icon(categoryIconData(category.iconName)),
                  title: Text(category.name),
                  subtitle: category.isActive ? null : const Text('Kategorija više nije aktivna'),
                ),
            ],
          ),
        ),
        SafeArea(
          top: false,
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: SizedBox(
              width: double.infinity,
              child: FilledButton.icon(
                onPressed: _isSaving ? null : _save,
                icon: _isSaving
                    ? const SizedBox.square(
                        dimension: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.save_outlined),
                label: Text('Sačuvaj (odabrano: ${_selected.length})'),
              ),
            ),
          ),
        ),
      ],
    );
  }
}
