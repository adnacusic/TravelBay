import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/news.dart';
import '../../providers/news_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/image_files.dart';
import '../../utils/validators.dart';
import '../../widgets/image_url_dialog.dart';
import '../../widgets/network_thumbnail.dart';

/// Create (no [newsId]) or edit a news item. Pops with a success message when saved.
class NewsFormScreen extends StatefulWidget {
  const NewsFormScreen({super.key, this.newsId});

  final int? newsId;

  @override
  State<NewsFormScreen> createState() => _NewsFormScreenState();
}

class _NewsFormScreenState extends State<NewsFormScreen> {
  /// Same limits as the API NewsRequestValidator.
  static const _maxTitleLength = 200;
  static const _maxContentLength = 5000;
  static const _imageWidth = 420.0;
  static const _imageHeight = 236.0;

  static final _dateFormat = DateFormat('dd.MM.yyyy. HH:mm');

  final _formKey = GlobalKey<FormBuilderState>();

  News? _news;

  /// A newly chosen image replaces the saved one only when the form is saved.
  PickedImage? _newFile;
  String? _newUrl;
  String? _imageError;

  bool _isLoading = true;
  bool _isSaving = false;
  String? _loadError;

  bool get _isEdit => widget.newsId != null;
  bool get _hasNewImage => _newFile != null || _newUrl != null;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    if (!_isEdit) {
      setState(() => _isLoading = false);
      return;
    }
    try {
      final news = await context.read<NewsProvider>().getById(widget.newsId!);
      if (mounted) {
        setState(() => _news = news);
      }
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

  Future<void> _pickFile() async {
    final picked = await pickImageFiles(allowMultiple: false);
    if (!mounted) {
      return;
    }
    setState(() {
      if (picked.images.isNotEmpty) {
        _newFile = picked.images.first;
        _newUrl = null;
      }
      _imageError = picked.rejected.isEmpty ? null : picked.rejected.join('\n');
    });
  }

  Future<void> _enterUrl() async {
    final url = await showDialog<String>(
      context: context,
      builder: (context) => const ImageUrlDialog(),
    );
    if (url != null && mounted) {
      setState(() {
        _newUrl = url;
        _newFile = null;
        _imageError = null;
      });
    }
  }

  Future<void> _save() async {
    final form = _formKey.currentState!;
    final fieldsValid = form.saveAndValidate();
    final imageMissing = !_isEdit && !_hasNewImage;
    setState(() {
      _imageError = imageMissing
          ? 'Slika je obavezna. Odaberite je sa računara ili unesite URL.'
          : _imageError;
    });
    if (!fieldsValid || imageMissing) {
      return;
    }

    final values = form.value;
    final request = <String, dynamic>{
      'title': (values['title'] as String).trim(),
      'content': (values['content'] as String).trim(),
      'publishedAt': (values['publishedAt'] as DateTime).toUtc().toIso8601String(),
      'imageUrl': _newUrl,
      if (_newFile != null) ...{
        'fileName': _newFile!.fileName,
        'contentType': _newFile!.contentType,
        'base64Content': _newFile!.base64Content,
      },
    };

    setState(() => _isSaving = true);
    try {
      final provider = context.read<NewsProvider>();
      final saved = _isEdit
          ? await provider.update(widget.newsId!, request)
          : await provider.insert(request);
      if (mounted) {
        Navigator.pop(
          context,
          _isEdit
              ? 'Izmjene novosti "${saved.title}" su sačuvane.'
              : 'Novost "${saved.title}" je objavljena.',
        );
      }
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

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      section: AdminSection.news,
      title: _isEdit ? 'Uređivanje novosti' : 'Nova novost',
      child: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_loadError != null) {
      return Center(child: Text(_loadError!));
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
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          _news?.title ?? 'Podaci o novosti',
                          style: Theme.of(context).textTheme.titleLarge,
                        ),
                      ),
                      IconButton(
                        tooltip: 'Zatvori bez snimanja',
                        icon: const Icon(Icons.close),
                        onPressed: _isSaving ? null : () => Navigator.pop(context),
                      ),
                    ],
                  ),
                  Text(
                    'Polja označena sa * su obavezna.',
                    style: Theme.of(context).textTheme.bodySmall,
                  ),
                  const SizedBox(height: 16),
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Expanded(flex: 3, child: _buildFields()),
                      const SizedBox(width: 32),
                      Expanded(flex: 2, child: _buildImagePanel()),
                    ],
                  ),
                  const SizedBox(height: 24),
                  const Divider(),
                  const SizedBox(height: 8),
                  Row(
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
                        label: Text(_isEdit ? 'Sačuvaj izmjene' : 'Objavi novost'),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildFields() {
    final news = _news;
    return FormBuilder(
      key: _formKey,
      initialValue: {
        'title': news?.title,
        'publishedAt': news?.publishedAt.toLocal() ?? DateTime.now(),
        'content': news?.content,
      },
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          FormBuilderTextField(
            name: 'title',
            maxLength: _maxTitleLength,
            decoration: const InputDecoration(labelText: 'Naslov *'),
            validator: Validators.requiredText('Naslov', maxLength: _maxTitleLength),
          ),
          const SizedBox(height: 8),
          FormBuilderDateTimePicker(
            name: 'publishedAt',
            inputType: InputType.both,
            format: _dateFormat,
            firstDate: DateTime(2000),
            lastDate: DateTime(2100),
            decoration: const InputDecoration(
              labelText: 'Datum i vrijeme objave *',
              suffixIcon: Icon(Icons.calendar_month_outlined),
              helperText: 'Novosti su u aplikaciji poredane po ovom datumu.',
            ),
            validator: (value) => value == null ? 'Odaberite datum objave.' : null,
          ),
          const SizedBox(height: 20),
          FormBuilderTextField(
            name: 'content',
            minLines: 8,
            maxLines: 16,
            maxLength: _maxContentLength,
            decoration: const InputDecoration(
              labelText: 'Tekst *',
              alignLabelWithHint: true,
            ),
            validator: Validators.requiredText('Tekst', maxLength: _maxContentLength),
          ),
        ],
      ),
    );
  }

  Widget _buildImagePanel() {
    final theme = Theme.of(context);

    final Widget preview;
    if (_newFile != null) {
      preview = ClipRRect(
        borderRadius: BorderRadius.circular(6),
        child: Image.memory(
          _newFile!.bytes,
          width: _imageWidth,
          height: _imageHeight,
          fit: BoxFit.cover,
        ),
      );
    } else {
      preview = NetworkThumbnail(
        imageUrl: _newUrl ?? _news?.imageUrl,
        width: _imageWidth,
        height: _imageHeight,
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Slika *', style: theme.textTheme.titleMedium),
        const SizedBox(height: 4),
        Text(
          _hasNewImage
              ? 'Nova slika zamjenjuje postojeću pri snimanju.'
              : 'Slika se prikazuje uz novost u mobilnoj aplikaciji.',
          style: theme.textTheme.bodySmall,
        ),
        const SizedBox(height: 12),
        preview,
        const SizedBox(height: 12),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: [
            OutlinedButton.icon(
              onPressed: _isSaving ? null : _pickFile,
              icon: const Icon(Icons.upload_file),
              label: const Text('Odaberi sa računara'),
            ),
            OutlinedButton.icon(
              onPressed: _isSaving ? null : _enterUrl,
              icon: const Icon(Icons.link),
              label: const Text('Unesi URL'),
            ),
            if (_hasNewImage)
              TextButton(
                onPressed: _isSaving
                    ? null
                    : () => setState(() {
                          _newFile = null;
                          _newUrl = null;
                        }),
                child: Text(_isEdit ? 'Vrati postojeću' : 'Ukloni'),
              ),
          ],
        ),
        if (_imageError != null) ...[
          const SizedBox(height: 8),
          Text(_imageError!, style: TextStyle(color: theme.colorScheme.error)),
        ],
      ],
    );
  }
}
