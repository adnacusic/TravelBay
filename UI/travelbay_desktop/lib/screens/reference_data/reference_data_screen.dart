import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/city.dart';
import '../../models/country.dart';
import '../../providers/city_provider.dart';
import '../../providers/country_provider.dart';
import '../../utils/dialogs.dart';
import '../../widgets/pagination_bar.dart';
import '../../widgets/search_field.dart';
import 'city_form_dialog.dart';
import 'country_form_dialog.dart';

/// Countries (left) and the cities of the selected country (right).
class ReferenceDataScreen extends StatefulWidget {
  const ReferenceDataScreen({super.key});

  @override
  State<ReferenceDataScreen> createState() => _ReferenceDataScreenState();
}

class _ReferenceDataScreenState extends State<ReferenceDataScreen> {
  static const _pageSize = 10;
  static const _dropdownListSize = 100;

  final _countrySearch = TextEditingController();
  final _citySearch = TextEditingController();

  /// Every country, for the city form dropdown.
  List<Country> _allCountries = [];

  List<Country> _countries = [];
  int _countryTotal = 0;
  int _countryPage = 1;
  bool _countriesLoading = true;

  /// Null shows the cities of all countries.
  Country? _selectedCountry;

  List<City> _cities = [];
  int _cityTotal = 0;
  int _cityPage = 1;
  bool _citiesLoading = true;

  @override
  void initState() {
    super.initState();
    _loadAllCountries();
    _loadCountries(1);
    _loadCities(1);
  }

  @override
  void dispose() {
    _countrySearch.dispose();
    _citySearch.dispose();
    super.dispose();
  }

  Future<void> _loadAllCountries() async {
    try {
      final result = await context
          .read<CountryProvider>()
          .get(filter: {'pageSize': _dropdownListSize, 'sortBy': 'Name'});
      if (mounted) {
        setState(() => _allCountries = result.items);
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _loadCountries(int page) async {
    setState(() => _countriesLoading = true);
    try {
      final result = await context.read<CountryProvider>().get(filter: {
        'name': _countrySearch.text.trim(),
        'includeTotalCount': true,
        'sortBy': 'Name',
        'page': page,
        'pageSize': _pageSize,
      });
      if (mounted) {
        setState(() {
          _countries = result.items;
          // Keep the selection, but with the current data (e.g. after a rename).
          final selectedId = _selectedCountry?.id;
          _selectedCountry =
              result.items.where((c) => c.id == selectedId).firstOrNull ?? _selectedCountry;
          _countryTotal = result.totalCount ?? result.items.length;
          _countryPage = page;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _countriesLoading = false);
      }
    }
  }

  Future<void> _loadCities(int page) async {
    setState(() => _citiesLoading = true);
    try {
      final result = await context.read<CityProvider>().get(filter: {
        'name': _citySearch.text.trim(),
        'countryId': _selectedCountry?.id,
        'includeTotalCount': true,
        'sortBy': 'Name',
        'page': page,
        'pageSize': _pageSize,
      });
      if (mounted) {
        setState(() {
          _cities = result.items;
          _cityTotal = result.totalCount ?? result.items.length;
          _cityPage = page;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _citiesLoading = false);
      }
    }
  }

  void _selectCountry(Country? country) {
    setState(() => _selectedCountry = country);
    _loadCities(1);
  }

  /// City counts per country and the dropdown list change with every country or city edit.
  void _reloadAll() {
    _loadAllCountries();
    _loadCountries(_countryPage);
    _loadCities(_cityPage);
  }

  Future<void> _openCountryForm([Country? country]) async {
    final message = await showDialog<String>(
      context: context,
      builder: (context) => CountryFormDialog(country: country),
    );
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _reloadAll();
    }
  }

  Future<void> _deleteCountry(Country country) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje države',
      message: 'Da li ste sigurni da želite obrisati državu "${country.name}"?',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<CountryProvider>().remove(country.id);
      if (!mounted) {
        return;
      }
      showSuccessMessage(context, 'Država "${country.name}" je obrisana.');
      if (_selectedCountry?.id == country.id) {
        _selectedCountry = null;
      }
      if (_countries.length == 1 && _countryPage > 1) {
        _countryPage--;
      }
      _reloadAll();
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _openCityForm([City? city]) async {
    final message = await showDialog<String>(
      context: context,
      builder: (context) => CityFormDialog(
        city: city,
        countries: _allCountries,
        initialCountryId: _selectedCountry?.id,
      ),
    );
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _reloadAll();
    }
  }

  Future<void> _deleteCity(City city) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje grada',
      message: 'Da li ste sigurni da želite obrisati grad "${city.name}" (${city.countryName})?',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<CityProvider>().remove(city.id);
      if (!mounted) {
        return;
      }
      showSuccessMessage(context, 'Grad "${city.name}" je obrisan.');
      if (_cities.length == 1 && _cityPage > 1) {
        _cityPage--;
      }
      _reloadAll();
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      section: AdminSection.referenceData,
      title: 'Države i gradovi',
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Expanded(flex: 2, child: _buildCountries()),
            const SizedBox(width: 24),
            Expanded(flex: 3, child: _buildCities()),
          ],
        ),
      ),
    );
  }

  Widget _buildCountries() {
    return _Panel(
      title: 'Države',
      action: FilledButton.icon(
        onPressed: () => _openCountryForm(),
        icon: const Icon(Icons.add),
        label: const Text('Nova država'),
      ),
      search: SearchField(
        label: 'Pretraga država',
        controller: _countrySearch,
        width: 280,
        onSearch: (_) => _loadCountries(1),
      ),
      isLoading: _countriesLoading,
      isEmpty: _countries.isEmpty,
      emptyText: 'Nema država koje odgovaraju pretrazi.',
      table: DataTable(
        showCheckboxColumn: false,
        columns: const [
          DataColumn(label: Text('Naziv')),
          DataColumn(label: Text('Gradova'), numeric: true),
          DataColumn(label: Text('Akcije')),
        ],
        rows: [
          for (final country in _countries)
            DataRow(
              selected: country.id == _selectedCountry?.id,
              onSelectChanged: (_) => _selectCountry(
                country.id == _selectedCountry?.id ? null : country,
              ),
              cells: [
                DataCell(Text(country.name)),
                DataCell(Text('${country.cityCount}')),
                DataCell(
                  _RowActions(
                    onEdit: () => _openCountryForm(country),
                    onDelete: () => _deleteCountry(country),
                    deleteBlockedReason: country.cityCount > 0
                        ? 'Država ima ${country.cityCount} gradova; prvo obrišite njene gradove.'
                        : null,
                  ),
                ),
              ],
            ),
        ],
      ),
      pagination: PaginationBar(
        page: _countryPage,
        pageSize: _pageSize,
        totalCount: _countryTotal,
        onPageChanged: _loadCountries,
      ),
      hint: 'Klik na državu prikazuje samo njene gradove; ponovni klik prikazuje sve.',
    );
  }

  Widget _buildCities() {
    final noCountries = _allCountries.isEmpty;
    final addButton = FilledButton.icon(
      onPressed: noCountries ? null : () => _openCityForm(),
      icon: const Icon(Icons.add),
      label: const Text('Novi grad'),
    );

    return _Panel(
      title: _selectedCountry == null ? 'Gradovi (sve države)' : 'Gradovi — ${_selectedCountry!.name}',
      action: noCountries
          ? Tooltip(message: 'Prvo dodajte bar jednu državu.', child: addButton)
          : addButton,
      search: SearchField(
        label: 'Pretraga gradova',
        controller: _citySearch,
        width: 280,
        onSearch: (_) => _loadCities(1),
      ),
      isLoading: _citiesLoading,
      isEmpty: _cities.isEmpty,
      emptyText: _selectedCountry == null
          ? 'Nema gradova koji odgovaraju pretrazi.'
          : 'Država ${_selectedCountry!.name} nema gradova koji odgovaraju pretrazi.',
      table: DataTable(
        showCheckboxColumn: false,
        columns: const [
          DataColumn(label: Text('Naziv')),
          DataColumn(label: Text('Država')),
          DataColumn(label: Text('Destinacija'), numeric: true),
          DataColumn(label: Text('Akcije')),
        ],
        rows: [
          for (final city in _cities)
            DataRow(
              onSelectChanged: (_) => _openCityForm(city),
              cells: [
                DataCell(Text(city.name)),
                DataCell(Text(city.countryName)),
                DataCell(Text('${city.destinationCount}')),
                DataCell(
                  _RowActions(
                    onEdit: () => _openCityForm(city),
                    onDelete: () => _deleteCity(city),
                    deleteBlockedReason: city.destinationCount > 0
                        ? 'Grad koristi ${city.destinationCount} destinacija, pa se ne može obrisati.'
                        : null,
                  ),
                ),
              ],
            ),
        ],
      ),
      pagination: PaginationBar(
        page: _cityPage,
        pageSize: _pageSize,
        totalCount: _cityTotal,
        onPageChanged: _loadCities,
      ),
    );
  }
}

class _Panel extends StatelessWidget {
  const _Panel({
    required this.title,
    required this.action,
    required this.search,
    required this.isLoading,
    required this.isEmpty,
    required this.emptyText,
    required this.table,
    required this.pagination,
    this.hint,
  });

  final String title;
  final Widget action;
  final Widget search;
  final bool isLoading;
  final bool isEmpty;
  final String emptyText;
  final Widget table;
  final Widget pagination;
  final String? hint;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      clipBehavior: Clip.antiAlias,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Expanded(child: Text(title, style: theme.textTheme.titleMedium)),
                action,
              ],
            ),
            if (hint != null) ...[
              const SizedBox(height: 4),
              Text(hint!, style: theme.textTheme.bodySmall),
            ],
            const SizedBox(height: 12),
            Align(alignment: Alignment.centerLeft, child: search),
            const SizedBox(height: 8),
            if (isLoading) const LinearProgressIndicator() else const SizedBox(height: 4),
            Expanded(
              child: isEmpty && !isLoading
                  ? Center(child: Text(emptyText))
                  : SingleChildScrollView(
                      child: SizedBox(width: double.infinity, child: table),
                    ),
            ),
            pagination,
          ],
        ),
      ),
    );
  }
}

class _RowActions extends StatelessWidget {
  const _RowActions({
    required this.onEdit,
    required this.onDelete,
    this.deleteBlockedReason,
  });

  final VoidCallback onEdit;
  final VoidCallback onDelete;

  /// When set, delete is disabled and this explains why.
  final String? deleteBlockedReason;

  @override
  Widget build(BuildContext context) {
    final blocked = deleteBlockedReason != null;
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        IconButton(
          tooltip: 'Uredi',
          icon: const Icon(Icons.edit_outlined),
          onPressed: onEdit,
        ),
        Tooltip(
          message: deleteBlockedReason ?? 'Obriši',
          child: IconButton(
            icon: Icon(
              Icons.delete_outline,
              color: blocked ? null : Theme.of(context).colorScheme.error,
            ),
            onPressed: blocked ? null : onDelete,
          ),
        ),
      ],
    );
  }
}
