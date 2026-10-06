import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/category.dart';
import '../../providers/category_provider.dart';
import '../../utils/category_icons.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/pagination_bar.dart';
import '../../widgets/scrollable_table.dart';
import '../../widgets/search_field.dart';
import '../../widgets/tone_chip.dart';
import 'category_form_dialog.dart';

class CategoryListScreen extends StatefulWidget {
  const CategoryListScreen({super.key});

  @override
  State<CategoryListScreen> createState() => _CategoryListScreenState();
}

class _CategoryListScreenState extends State<CategoryListScreen> {
  static const _pageSize = 10;

  final _searchController = TextEditingController();

  List<Category> _categories = [];
  int _totalCount = 0;
  int _page = 1;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadPage(1);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadPage(int page) async {
    setState(() => _isLoading = true);
    try {
      final result = await context.read<CategoryProvider>().get(filter: {
        'name': _searchController.text.trim(),
        'includeTotalCount': true,
        'sortBy': 'Id desc',
        'page': page,
        'pageSize': _pageSize,
      });
      if (mounted) {
        setState(() {
          _categories = result.items;
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

  Future<void> _openForm([Category? category]) async {
    final message = await showDialog<String>(
      context: context,
      builder: (context) => CategoryFormDialog(category: category),
    );
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _loadPage(category == null ? 1 : _page);
    }
  }

  Future<void> _delete(Category category) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje kategorije',
      message: 'Da li ste sigurni da želite obrisati kategoriju "${category.name}"?',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<CategoryProvider>().remove(category.id);
      if (mounted) {
        showSuccessMessage(context, 'Kategorija "${category.name}" je obrisana.');
        _loadPage(_categories.length == 1 && _page > 1 ? _page - 1 : _page);
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      section: AdminSection.categories,
      title: 'Upravljanje kategorijama',
      actions: [
        FilledButton.icon(
          onPressed: () => _openForm(),
          icon: const Icon(Icons.add),
          label: const Text('Nova kategorija'),
        ),
      ],
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                SearchField(
                  label: 'Pretraga po nazivu',
                  controller: _searchController,
                  onSearch: (_) => _loadPage(1),
                ),
              ],
            ),
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

  Widget _buildTable() {
    if (_isLoading && _categories.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_categories.isEmpty) {
      return const Center(child: Text('Nema kategorija koje odgovaraju pretrazi.'));
    }

    return Card(
      clipBehavior: Clip.antiAlias,
      child: Stack(
        children: [
          SingleChildScrollView(
            child: ScrollableTable(
              child: DataTable(
                showCheckboxColumn: false,
                columns: const [
                  DataColumn(label: Text('Ikona')),
                  DataColumn(label: Text('Naziv')),
                  DataColumn(label: Text('Status')),
                  DataColumn(label: Text('Broj destinacija'), numeric: true),
                  DataColumn(label: Text('Dodano')),
                  DataColumn(label: Text('Akcije')),
                ],
                rows: [
                  for (final category in _categories)
                    DataRow(
                      onSelectChanged: (_) => _openForm(category),
                      cells: [
                        DataCell(Icon(categoryIconData(category.iconName))),
                        DataCell(Text(category.name)),
                        DataCell(
                          ToneChip(
                            label: category.isActive ? 'Aktivna' : 'Neaktivna',
                            tone: category.isActive ? ChipTone.positive : ChipTone.neutral,
                          ),
                        ),
                        DataCell(Text('${category.destinationCount}')),
                        DataCell(Text(formatDate(category.createdAt))),
                        DataCell(_buildActions(category)),
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

  Widget _buildActions(Category category) {
    final inUse = category.destinationCount > 0;
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        IconButton(
          tooltip: 'Uredi',
          icon: const Icon(Icons.edit_outlined),
          onPressed: () => _openForm(category),
        ),
        Tooltip(
          message: inUse
              ? 'Kategoriju koristi ${category.destinationCount} destinacija, pa se ne može obrisati. '
                  'Možete je deaktivirati.'
              : 'Obriši',
          child: IconButton(
            icon: Icon(
              Icons.delete_outline,
              color: inUse ? null : Theme.of(context).colorScheme.error,
            ),
            onPressed: inUse ? null : () => _delete(category),
          ),
        ),
      ],
    );
  }
}
