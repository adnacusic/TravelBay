import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/category.dart';
import '../../models/city.dart';
import '../../models/country.dart';
import '../../models/destination.dart';
import '../../models/destination_image.dart';
import '../../providers/category_provider.dart';
import '../../providers/city_provider.dart';
import '../../providers/country_provider.dart';
import '../../providers/destination_image_provider.dart';
import '../../providers/destination_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/form_errors.dart';
import '../../utils/formatters.dart';
import '../../utils/image_files.dart';
import '../../utils/validators.dart';
import '../../widgets/image_url_dialog.dart';
import '../../widgets/network_thumbnail.dart';
import '../../widgets/status_chip.dart';

/// Create (no [destinationId]) or edit a destination together with its images.
/// Pops with a success message when saved, or with null when closed.
class DestinationFormScreen extends StatefulWidget {
  const DestinationFormScreen({super.key, this.destinationId});

  final int? destinationId;

  @override
  State<DestinationFormScreen> createState() => _DestinationFormScreenState();
}

/// An image chosen in the form, uploaded only after the destination is saved.
class _PendingImage {
  _PendingImage.file(PickedImage this.file) : url = null;

  _PendingImage.url(String this.url) : file = null;

  final PickedImage? file;
  final String? url;

  String get label => file?.fileName ?? url!;
}

class _DestinationFormScreenState extends State<DestinationFormScreen> {
  static const _maxNameLength = 200;
  static const _maxDescriptionLength = 2000;
  static const _referencePageSize = 100;

  final _formKey = GlobalKey<FormBuilderState>();

  Destination? _destination;
  List<Category> _categories = [];
  List<Country> _countries = [];
  List<City> _cities = [];
  int? _selectedCountryId;

  List<DestinationImage> _savedImages = [];
  final List<_PendingImage> _pendingImages = [];
  String? _imageError;

  bool _isLoading = true;
  bool _isSaving = false;
  String? _loadError;

  bool get _isEdit => widget.destinationId != null;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    final referenceFilter = {'pageSize': _referencePageSize, 'sortBy': 'Name'};
    try {
      final categoriesFuture =
          context.read<CategoryProvider>().get(filter: referenceFilter);
      final countriesFuture =
          context.read<CountryProvider>().get(filter: referenceFilter);
      final citiesFuture =
          context.read<CityProvider>().get(filter: referenceFilter);
      final destinationFuture = _isEdit
          ? context.read<DestinationProvider>().getById(widget.destinationId!)
          : Future<Destination?>.value(null);

      final categories = await categoriesFuture;
      final countries = await countriesFuture;
      final cities = await citiesFuture;
      final destination = await destinationFuture;
      if (!mounted) {
        return;
      }

      setState(() {
        _categories = categories.items;
        _countries = countries.items;
        _cities = cities.items;
        _destination = destination;
        _savedImages = [...?destination?.images];
        _selectedCountryId = destination == null
            ? null
            : _cities
                .where((c) => c.id == destination.cityId)
                .map((c) => c.countryId)
                .firstOrNull;
        _loadError = _categories.isEmpty || _cities.isEmpty
            ? 'Destinaciju nije moguće unijeti dok ne postoje bar jedna '
                'kategorija i bar jedan grad.'
            : null;
      });
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _loadError = e.toString());
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  Future<void> _save() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final values = form.value;
    final keywords = (values['keywords'] as String?)?.trim() ?? '';
    final request = {
      'name': (values['name'] as String).trim(),
      'description': (values['description'] as String).trim(),
      'categoryId': values['categoryId'],
      'cityId': values['cityId'],
      'keywords': keywords.isEmpty ? null : keywords,
    };

    setState(() => _isSaving = true);
    try {
      final destinationProvider = context.read<DestinationProvider>();
      final saved = _isEdit
          ? await destinationProvider.update(widget.destinationId!, request)
          : await destinationProvider.insert(request);

      final failedImages = await _uploadPendingImages(saved.id);
      if (!mounted) {
        return;
      }

      if (failedImages.isNotEmpty) {
        await showErrorDialog(
          context,
          'Destinacija je sačuvana, ali sljedeće slike nisu dodane:\n\n'
          '${failedImages.join('\n')}',
        );
        if (!mounted) {
          return;
        }
      }

      Navigator.pop(
        context,
        _isEdit
            ? 'Izmjene destinacije "${saved.name}" su sačuvane.'
            : 'Destinacija "${saved.name}" je dodana.',
      );
    } on Exception catch (e) {
      if (mounted) {
        showFormErrors(context, _formKey.currentState, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSaving = false);
      }
    }
  }

  /// Returns a line per image that could not be added.
  Future<List<String>> _uploadPendingImages(int destinationId) async {
    final imageProvider = context.read<DestinationImageProvider>();
    final failures = <String>[];

    for (final image in _pendingImages) {
      try {
        if (image.url != null) {
          await imageProvider.addFromUrl(destinationId, image.url!);
        } else {
          final file = image.file!;
          await imageProvider.upload(
            destinationId,
            fileName: file.fileName,
            contentType: file.contentType,
            base64Content: file.base64Content,
          );
        }
      } on Exception catch (e) {
        failures.add('${image.label}: $e');
      }
    }
    return failures;
  }

  Future<void> _pickImageFiles() async {
    setState(() => _imageError = null);
    final picked = await pickImageFiles();
    if (!mounted) {
      return;
    }
    setState(() {
      _pendingImages.addAll(picked.images.map(_PendingImage.file));
      _imageError =
          picked.rejected.isEmpty ? null : picked.rejected.join('\n');
    });
  }

  Future<void> _addImageUrl() async {
    setState(() => _imageError = null);
    final url = await showDialog<String>(
      context: context,
      builder: (context) => const ImageUrlDialog(),
    );
    if (url != null && mounted) {
      setState(() => _pendingImages.add(_PendingImage.url(url)));
    }
  }

  Future<void> _deleteSavedImage(DestinationImage image) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje slike',
      message: 'Da li ste sigurni da želite obrisati ovu sliku? '
          'Brisanje se ne može poništiti.',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<DestinationImageProvider>().remove(image.id);
      if (mounted) {
        setState(() => _savedImages.remove(image));
        showSuccessMessage(context, 'Slika je obrisana.');
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
      section: AdminSection.destinations,
      title: _isEdit ? 'Uređivanje destinacije' : 'Nova destinacija',
      child: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_loadError != null) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(_loadError!, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            OutlinedButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('Nazad'),
            ),
          ],
        ),
      );
    }

    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      child: Align(
        alignment: Alignment.topCenter,
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 1200),
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  _buildFormHeader(),
                  const SizedBox(height: 16),
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Expanded(flex: 3, child: _buildFields()),
                      const SizedBox(width: 32),
                      Expanded(flex: 2, child: _buildImagesPanel()),
                    ],
                  ),
                  const SizedBox(height: 24),
                  const Divider(),
                  const SizedBox(height: 8),
                  _buildActions(),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildFormHeader() {
    final theme = Theme.of(context);
    final destination = _destination;

    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                destination?.name ?? 'Podaci o destinaciji',
                style: theme.textTheme.titleLarge,
              ),
              const SizedBox(height: 4),
              Text(
                destination == null
                    ? 'Polja označena sa * su obavezna.'
                    : 'Dodano ${formatDate(destination.createdAt)}'
                        '${destination.updatedAt == null ? '' : ' · izmijenjeno ${formatDate(destination.updatedAt)}'}'
                        ' · ${_ratingText(destination)}',
                style: theme.textTheme.bodySmall,
              ),
              if (destination != null) ...[
                const SizedBox(height: 8),
                AiStatusChips(
                  hasKeywords: destination.hasKeywords,
                  hasImages: _savedImages.isNotEmpty,
                ),
              ],
            ],
          ),
        ),
        IconButton(
          tooltip: 'Zatvori bez snimanja',
          icon: const Icon(Icons.close),
          onPressed: _isSaving ? null : () => Navigator.pop(context),
        ),
      ],
    );
  }

  String _ratingText(Destination destination) {
    final rating = destination.averageRating;
    if (rating == null) {
      return 'još nema odobrenih recenzija';
    }
    return 'prosječna ocjena ${rating.toStringAsFixed(1)} '
        '(${destination.reviewCount} odobrenih recenzija)';
  }

  Widget _buildFields() {
    final destination = _destination;
    final categoryItems = _categories
        .where((c) => c.isActive || c.id == destination?.categoryId)
        .map((c) => DropdownMenuItem(value: c.id, child: Text(c.name)))
        .toList();
    final cityItems = _cities
        .where((c) => c.countryId == _selectedCountryId)
        .map((c) => DropdownMenuItem(value: c.id, child: Text(c.name)))
        .toList();

    return FormBuilder(
      key: _formKey,
      initialValue: {
        'name': destination?.name,
        'categoryId': destination?.categoryId,
        'countryId': _selectedCountryId,
        'description': destination?.description,
        'keywords': destination?.keywords,
      },
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          FormBuilderTextField(
            name: 'name',
            maxLength: _maxNameLength,
            decoration: const InputDecoration(labelText: 'Naziv *'),
            validator: Validators.requiredText(
              'Naziv',
              maxLength: _maxNameLength,
            ),
          ),
          const SizedBox(height: 8),
          FormBuilderDropdown<int>(
            name: 'categoryId',
            decoration: const InputDecoration(labelText: 'Kategorija *'),
            items: categoryItems,
            validator: Validators.requiredSelection<int>('kategoriju'),
          ),
          const SizedBox(height: 20),
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: FormBuilderDropdown<int>(
                  name: 'countryId',
                  decoration: const InputDecoration(labelText: 'Država *'),
                  items: _countries
                      .map((c) => DropdownMenuItem(value: c.id, child: Text(c.name)))
                      .toList(),
                  validator: Validators.requiredSelection<int>('državu'),
                  onChanged: (countryId) {
                    if (countryId == _selectedCountryId) {
                      return;
                    }
                    setState(() => _selectedCountryId = countryId);
                    _formKey.currentState?.fields['cityId']?.didChange(null);
                  },
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: FormBuilderDropdown<int>(
                  // Rebuilt per country so the item list and value always match.
                  key: ValueKey('city-$_selectedCountryId'),
                  name: 'cityId',
                  initialValue: cityItems.any((i) => i.value == destination?.cityId)
                      ? destination?.cityId
                      : null,
                  enabled: _selectedCountryId != null,
                  decoration: InputDecoration(
                    labelText: 'Grad *',
                    helperText: _selectedCountryId == null
                        ? 'Prvo odaberite državu.'
                        : cityItems.isEmpty
                            ? 'Odabrana država nema unesenih gradova.'
                            : null,
                  ),
                  items: cityItems,
                  validator: Validators.requiredSelection<int>('grad'),
                ),
              ),
            ],
          ),
          const SizedBox(height: 20),
          FormBuilderTextField(
            name: 'description',
            minLines: 5,
            maxLines: 10,
            maxLength: _maxDescriptionLength,
            decoration: const InputDecoration(
              labelText: 'Opis *',
              alignLabelWithHint: true,
            ),
            validator: Validators.requiredText(
              'Opis',
              maxLength: _maxDescriptionLength,
            ),
          ),
          const SizedBox(height: 8),
          FormBuilderTextField(
            name: 'keywords',
            decoration: const InputDecoration(
              labelText: 'Ključne riječi',
              helperText: 'Odvojene zarezom. Popunjava ih AI agent, '
                  'a možete ih unijeti i ručno.',
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildImagesPanel() {
    final theme = Theme.of(context);
    final hasImages = _savedImages.isNotEmpty || _pendingImages.isNotEmpty;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text('Slike', style: theme.textTheme.titleMedium),
        const SizedBox(height: 4),
        Text(
          'Prva slika je naslovna. Nove slike se dodaju pri snimanju destinacije.',
          style: theme.textTheme.bodySmall,
        ),
        const SizedBox(height: 12),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: [
            OutlinedButton.icon(
              onPressed: _isSaving ? null : _pickImageFiles,
              icon: const Icon(Icons.upload_file),
              label: const Text('Dodaj sa računara'),
            ),
            OutlinedButton.icon(
              onPressed: _isSaving ? null : _addImageUrl,
              icon: const Icon(Icons.link),
              label: const Text('Dodaj URL'),
            ),
          ],
        ),
        if (_imageError != null) ...[
          const SizedBox(height: 8),
          Text(_imageError!, style: TextStyle(color: theme.colorScheme.error)),
        ],
        const SizedBox(height: 12),
        if (!hasImages)
          Container(
            height: 140,
            alignment: Alignment.center,
            decoration: BoxDecoration(
              color: theme.colorScheme.surfaceContainerHighest,
              borderRadius: BorderRadius.circular(8),
            ),
            child: const Text(
              'Destinacija još nema sliku.\nDodajte je ručno ili je pronađite AI agentom.',
              textAlign: TextAlign.center,
            ),
          )
        else
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              for (final image in _savedImages)
                _ImageTile(
                  preview: NetworkThumbnail(
                    imageUrl: image.imageUrl,
                    width: _ImageTile.width,
                    height: _ImageTile.height,
                  ),
                  badge: image.isAiGenerated ? 'AI' : null,
                  deleteTooltip: 'Obriši sliku',
                  onDelete: _isSaving ? null : () => _deleteSavedImage(image),
                ),
              for (final image in _pendingImages)
                _ImageTile(
                  preview: image.file != null
                      ? ClipRRect(
                          borderRadius: BorderRadius.circular(6),
                          child: Image.memory(
                            image.file!.bytes,
                            width: _ImageTile.width,
                            height: _ImageTile.height,
                            fit: BoxFit.cover,
                          ),
                        )
                      : NetworkThumbnail(
                          imageUrl: image.url,
                          width: _ImageTile.width,
                          height: _ImageTile.height,
                        ),
                  badge: 'Nova',
                  deleteTooltip: 'Ukloni (još nije sačuvana)',
                  onDelete: _isSaving
                      ? null
                      : () => setState(() => _pendingImages.remove(image)),
                ),
            ],
          ),
      ],
    );
  }

  Widget _buildActions() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.end,
      children: [
        OutlinedButton(
          onPressed: _isSaving ? null : () => Navigator.pop(context),
          child: const Text('Odustani'),
        ),
        const SizedBox(width: 12),
        FilledButton.icon(
          onPressed: _isSaving ? null : _save,
          icon: _isSaving
              ? const SizedBox.square(
                  dimension: 18,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Icon(Icons.save_outlined),
          label: Text(_isEdit ? 'Sačuvaj izmjene' : 'Dodaj destinaciju'),
        ),
      ],
    );
  }
}

class _ImageTile extends StatelessWidget {
  const _ImageTile({
    required this.preview,
    required this.deleteTooltip,
    required this.onDelete,
    this.badge,
  });

  static const double width = 168;
  static const double height = 112;

  final Widget preview;
  final String? badge;
  final String deleteTooltip;
  final VoidCallback? onDelete;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return SizedBox(
      width: width,
      height: height,
      child: Stack(
        children: [
          preview,
          if (badge != null)
            Positioned(
              left: 6,
              top: 6,
              child: Container(
                padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                decoration: BoxDecoration(
                  color: scheme.primaryContainer,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  badge!,
                  style: TextStyle(
                    fontSize: 11,
                    color: scheme.onPrimaryContainer,
                  ),
                ),
              ),
            ),
          Positioned(
            right: 4,
            top: 4,
            child: IconButton.filledTonal(
              tooltip: deleteTooltip,
              iconSize: 18,
              visualDensity: VisualDensity.compact,
              onPressed: onDelete,
              icon: const Icon(Icons.delete_outline),
            ),
          ),
        ],
      ),
    );
  }
}
