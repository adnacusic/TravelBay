import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/category.dart';
import '../../models/city.dart';
import '../../models/country.dart';
import '../../models/destination.dart';
import '../../providers/category_provider.dart';
import '../../providers/destination_provider.dart';
import '../../providers/location_provider.dart';
import '../../utils/app_navigator.dart';
import '../../widgets/destination_widgets.dart';
import '../../widgets/search_field.dart';
import '../destination/destination_details_screen.dart';

/// Search by name or place (city, country) with category, location and minimum rating
/// filters; filtering runs in the API and results load page by page.
class SearchScreen extends StatefulWidget {
  const SearchScreen({super.key, this.initialCategoryId});

  /// Category chosen on the home screen.
  final int? initialCategoryId;

  @override
  State<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends State<SearchScreen> {
  static const _pageSize = 20;
  static const _referenceListSize = 100;

  /// Minimum average rating options; null means any rating (also unrated destinations).
  static const _ratingOptions = <double?>[null, 3, 4, 4.5];

  final _searchController = TextEditingController();

  List<Category> _categories = [];
  int? _categoryId;
  List<Country> _countries = [];
  int? _countryId;
  List<City> _cities = [];
  int? _cityId;
  double? _minRating;

  final List<Destination> _results = [];
  int _totalCount = 0;
  int _page = 0;
  bool _isLoading = false;
  String? _error;

  bool get _hasMore => _results.length < _totalCount;

  @override
  void initState() {
    super.initState();
    _categoryId = widget.initialCategoryId;
    _loadCategories();
    _loadCountries();
    _loadCities();
    _search();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadCategories() async {
    try {
      final result = await context
          .read<CategoryProvider>()
          .get(filter: {'pageSize': _referenceListSize, 'sortBy': 'Name'});
      if (mounted) {
        setState(() => _categories = result.items.where((c) => c.isActive).toList());
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  Future<void> _loadCountries() async {
    try {
      final result = await context
          .read<CountryProvider>()
          .get(filter: {'pageSize': _referenceListSize, 'sortBy': 'Name'});
      if (mounted) {
        setState(() => _countries = result.items);
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  /// Cities of the selected country (all cities when no country is chosen).
  Future<void> _loadCities() async {
    try {
      final result = await context.read<CityProvider>().get(filter: {
        'pageSize': _referenceListSize,
        'sortBy': 'Name',
        'countryId': _countryId,
      });
      if (mounted) {
        setState(() => _cities = result.items);
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  /// Starts a new search (first page) with the current text and filters.
  Future<void> _search() async {
    setState(() {
      _results.clear();
      _totalCount = 0;
      _page = 0;
    });
    await _loadNextPage();
  }

  Future<void> _loadNextPage() async {
    if (_isLoading) {
      return;
    }
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final result = await context.read<DestinationProvider>().get(filter: {
        'searchText': _searchController.text.trim(),
        'categoryId': _categoryId,
        'countryId': _countryId,
        'cityId': _cityId,
        'minRating': _minRating,
        'includeCategory': true,
        'includeImages': true,
        'includeTotalCount': true,
        'sortBy': 'Name',
        'page': _page + 1,
        'pageSize': _pageSize,
      });
      if (mounted) {
        setState(() {
          _results.addAll(result.items);
          _totalCount = result.totalCount ?? _results.length;
          _page++;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  void _selectCategory(int? categoryId) {
    setState(() => _categoryId = categoryId);
    _search();
  }

  void _selectCountry(int? countryId) {
    setState(() {
      _countryId = countryId;
      _cityId = null;
      _cities = [];
    });
    _loadCities();
    _search();
  }

  void _selectCity(int? cityId) {
    setState(() => _cityId = cityId);
    _search();
  }

  void _selectRating(double? minRating) {
    setState(() => _minRating = minRating);
    _search();
  }

  /// "Sve ocjene", "★ 3+", "★ 4,5+".
  static String _ratingLabel(double? rating) {
    if (rating == null) {
      return 'Sve ocjene';
    }
    final text = rating == rating.roundToDouble()
        ? rating.toStringAsFixed(0)
        : rating.toString().replaceAll('.', ',');
    return '★ $text+';
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Pretraga')),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(12, 8, 12, 4),
            child: SearchField(
              label: 'Naziv, grad ili država',
              controller: _searchController,
              width: double.infinity,
              onSearch: (_) => _search(),
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(12, 4, 12, 4),
            child: Row(
              children: [
                Expanded(
                  child: DropdownButtonFormField<int?>(
                    key: ValueKey('country-$_countryId'),
                    initialValue: _countryId,
                    isExpanded: true,
                    decoration: const InputDecoration(labelText: 'Država', isDense: true),
                    items: [
                      const DropdownMenuItem<int?>(value: null, child: Text('Sve države')),
                      for (final country in _countries)
                        DropdownMenuItem<int?>(value: country.id, child: Text(country.name)),
                    ],
                    onChanged: _selectCountry,
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: DropdownButtonFormField<int?>(
                    key: ValueKey('city-$_countryId-$_cityId-${_cities.length}'),
                    initialValue: _cityId,
                    isExpanded: true,
                    decoration: const InputDecoration(labelText: 'Grad', isDense: true),
                    items: [
                      const DropdownMenuItem<int?>(value: null, child: Text('Svi gradovi')),
                      for (final city in _cities)
                        DropdownMenuItem<int?>(value: city.id, child: Text(city.name)),
                    ],
                    onChanged: _selectCity,
                  ),
                ),
              ],
            ),
          ),
          SizedBox(
            height: 48,
            child: ListView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 12),
              children: [
                for (final rating in _ratingOptions)
                  Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: ChoiceChip(
                      label: Text(_ratingLabel(rating)),
                      selected: _minRating == rating,
                      onSelected: (_) => _selectRating(rating),
                    ),
                  ),
              ],
            ),
          ),
          SizedBox(
            height: 48,
            child: ListView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 12),
              children: [
                Padding(
                  padding: const EdgeInsets.only(right: 8),
                  child: ChoiceChip(
                    label: const Text('Sve kategorije'),
                    selected: _categoryId == null,
                    onSelected: (_) => _selectCategory(null),
                  ),
                ),
                for (final category in _categories)
                  Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: ChoiceChip(
                      label: Text(category.name),
                      selected: _categoryId == category.id,
                      onSelected: (_) => _selectCategory(category.id),
                    ),
                  ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 4, 16, 4),
            child: Text(
              _isLoading && _results.isEmpty ? 'Tražim…' : 'Pronađeno: $_totalCount',
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ),
          Expanded(child: _buildResults()),
        ],
      ),
    );
  }

  Widget _buildResults() {
    if (_error != null && _results.isEmpty) {
      return Center(
        child: Padding(padding: const EdgeInsets.all(24), child: Text(_error!)),
      );
    }
    if (_isLoading && _results.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_results.isEmpty) {
      return const Center(child: Text('Nema destinacija za zadanu pretragu.'));
    }

    return ListView.builder(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
      itemCount: _results.length + (_hasMore ? 1 : 0),
      itemBuilder: (context, index) {
        if (index == _results.length) {
          return Padding(
            padding: const EdgeInsets.all(12),
            child: Center(
              child: _isLoading
                  ? const CircularProgressIndicator()
                  : OutlinedButton(
                      onPressed: _loadNextPage,
                      child: const Text('Učitaj još'),
                    ),
            ),
          );
        }
        final destination = _results[index];
        return DestinationTile(
          destination: destination,
          onTap: () => openPage<void>(
            context,
            DestinationDetailsScreen(destinationId: destination.id),
          ),
        );
      },
    );
  }
}
