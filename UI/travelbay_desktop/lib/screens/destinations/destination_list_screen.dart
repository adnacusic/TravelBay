import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/category.dart';
import '../../models/destination.dart';
import '../../providers/category_provider.dart';
import '../../providers/city_provider.dart';
import '../../providers/destination_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/network_thumbnail.dart';
import '../../widgets/pagination_bar.dart';
import '../../widgets/scrollable_table.dart';
import '../../widgets/search_field.dart';
import '../../widgets/status_chip.dart';
import 'destination_form_screen.dart';

class DestinationListScreen extends StatefulWidget {
  const DestinationListScreen({super.key});

  @override
  State<DestinationListScreen> createState() => _DestinationListScreenState();
}

class _DestinationListScreenState extends State<DestinationListScreen> {
  static const _pageSize = 10;

  final _searchController = TextEditingController();

  List<Category> _categories = [];
  int? _categoryFilter;

  /// Why a new destination cannot be added yet; null when everything it needs exists.
  String? _addBlockedReason;

  List<Destination> _destinations = [];
  int _totalCount = 0;
  int _page = 1;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadReferenceData();
    _loadPage(1);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadReferenceData() async {
    try {
      final categories = await context
          .read<CategoryProvider>()
          .get(filter: {'pageSize': 100, 'sortBy': 'Name'});
      if (!mounted) {
        return;
      }
      final cities = await context
          .read<CityProvider>()
          .get(filter: {'pageSize': 1});
      if (!mounted) {
        return;
      }

      setState(() {
        _categories = categories.items;
        _addBlockedReason = categories.items.isEmpty
            ? 'Prvo dodajte bar jednu kategoriju.'
            : cities.items.isEmpty
                ? 'Prvo dodajte bar jedan grad (Države i gradovi).'
                : null;
      });
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _loadPage(int page) async {
    setState(() => _isLoading = true);
    try {
      final result = await context.read<DestinationProvider>().get(filter: {
        'name': _searchController.text.trim(),
        'categoryId': _categoryFilter,
        'includeCategory': true,
        'includeImages': true,
        'includeTotalCount': true,
        'sortBy': 'Id desc',
        'page': page,
        'pageSize': _pageSize,
      });
      if (mounted) {
        setState(() {
          _destinations = result.items;
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

  void _clearFilters() {
    _searchController.clear();
    setState(() => _categoryFilter = null);
    _loadPage(1);
  }

  Future<void> _openForm([Destination? destination]) async {
    final message = await openPage<String>(
      context,
      DestinationFormScreen(destinationId: destination?.id),
    );
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      // A new destination has the highest Id, so it shows on top of page 1.
      _loadPage(destination == null ? 1 : _page);
    }
  }

  Future<void> _delete(Destination destination) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje destinacije',
      message: 'Da li ste sigurni da želite obrisati destinaciju '
          '"${destination.name}"? Destinacija više neće biti vidljiva korisnicima.',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<DestinationProvider>().remove(destination.id);
      if (!mounted) {
        return;
      }
      showSuccessMessage(
        context,
        'Destinacija "${destination.name}" je obrisana.',
      );
      final lastItemOnPage = _destinations.length == 1 && _page > 1;
      _loadPage(lastItemOnPage ? _page - 1 : _page);
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final addButton = FilledButton.icon(
      onPressed: _addBlockedReason == null ? () => _openForm() : null,
      icon: const Icon(Icons.add),
      label: const Text('Nova destinacija'),
    );

    return MasterScreen(
      section: AdminSection.destinations,
      title: 'Upravljanje destinacijama',
      actions: [
        _addBlockedReason == null
            ? addButton
            : Tooltip(message: _addBlockedReason!, child: addButton),
      ],
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
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

  Widget _buildFilters() {
    return Row(
      children: [
        SearchField(
          label: 'Pretraga po nazivu',
          controller: _searchController,
          onSearch: (_) => _loadPage(1),
        ),
        const SizedBox(width: 16),
        SizedBox(
          width: 240,
          child: DropdownButtonFormField<int?>(
            key: ValueKey(_categoryFilter),
            initialValue: _categoryFilter,
            decoration: const InputDecoration(labelText: 'Kategorija'),
            items: [
              const DropdownMenuItem<int?>(
                value: null,
                child: Text('Sve kategorije'),
              ),
              for (final category in _categories)
                DropdownMenuItem<int?>(
                  value: category.id,
                  child: Text(category.name),
                ),
            ],
            onChanged: (value) {
              setState(() => _categoryFilter = value);
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
    if (_isLoading && _destinations.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_destinations.isEmpty) {
      return const Center(
        child: Text('Nema destinacija koje odgovaraju zadanim filterima.'),
      );
    }

    return Card(
      clipBehavior: Clip.antiAlias,
      child: Stack(
        children: [
          SingleChildScrollView(
            child: ScrollableTable(
              child: DataTable(
                showCheckboxColumn: false,
                dataRowMinHeight: 64,
                dataRowMaxHeight: 72,
                columns: const [
                  DataColumn(label: Text('Slika')),
                  DataColumn(label: Text('Naziv')),
                  DataColumn(label: Text('Kategorija')),
                  DataColumn(label: Text('Grad')),
                  DataColumn(label: Text('Dodano')),
                  DataColumn(label: Text('AI status')),
                  DataColumn(label: Text('Akcije')),
                ],
                rows: [
                  for (final destination in _destinations)
                    DataRow(
                      onSelectChanged: (_) => _openForm(destination),
                      cells: [
                        DataCell(
                          NetworkThumbnail(
                            imageUrl: destination.coverImage?.imageUrl,
                            width: 72,
                            height: 52,
                          ),
                        ),
                        DataCell(
                          ConstrainedBox(
                            constraints: const BoxConstraints(maxWidth: 260),
                            child: Text(
                              destination.name,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                        ),
                        DataCell(Text(destination.category?.name ?? '-')),
                        DataCell(Text(destination.cityName)),
                        DataCell(Text(formatDate(destination.createdAt))),
                        DataCell(
                          AiStatusChips(
                            hasKeywords: destination.hasKeywords,
                            hasImages: destination.hasImages,
                          ),
                        ),
                        DataCell(
                          Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              IconButton(
                                tooltip: 'Uredi',
                                icon: const Icon(Icons.edit_outlined),
                                onPressed: () => _openForm(destination),
                              ),
                              IconButton(
                                tooltip: 'Obriši',
                                icon: Icon(
                                  Icons.delete_outline,
                                  color: Theme.of(context).colorScheme.error,
                                ),
                                onPressed: () => _delete(destination),
                              ),
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
}
