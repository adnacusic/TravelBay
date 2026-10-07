import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/destination.dart';
import '../../models/enums.dart';
import '../../models/review.dart';
import '../../models/user_activity.dart';
import '../../providers/destination_provider.dart';
import '../../providers/review_provider.dart';
import '../../providers/user_activity_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/destination_widgets.dart';
import '../../widgets/network_thumbnail.dart';
import '../trips/trip_item_sheet.dart';
import 'add_to_collection_sheet.dart';
import 'review_form.dart';

/// Destination details with reviews and the user's actions (save, collection, trip plan,
/// review). Opening it records a ViewHistory entry, which feeds the recommender.
class DestinationDetailsScreen extends StatefulWidget {
  const DestinationDetailsScreen({super.key, required this.destinationId});

  final int destinationId;

  @override
  State<DestinationDetailsScreen> createState() => _DestinationDetailsScreenState();
}

class _DestinationDetailsScreenState extends State<DestinationDetailsScreen> {
  static const _galleryHeight = 240.0;
  static const _reviewPageSize = 5;

  Destination? _destination;
  String? _error;
  int _imageIndex = 0;

  SavedDestination? _saved;
  bool _isSaving = false;

  final List<Review> _reviews = [];
  int _reviewTotal = 0;
  int _reviewPage = 0;
  bool _isLoadingReviews = false;

  /// The user's own reviews of this destination (any status), newest first.
  List<Review>? _myReviews;

  @override
  void initState() {
    super.initState();
    _recordView();
    _load();
  }

  /// A failed view record must not block the details; it is only logged.
  Future<void> _recordView() async {
    try {
      await context.read<ViewHistoryProvider>().record(widget.destinationId);
    } on Exception catch (e) {
      debugPrint('ViewHistory record failed for destination ${widget.destinationId}: $e');
    }
  }

  Future<void> _load() async {
    try {
      final destinationFuture = context.read<DestinationProvider>().getById(widget.destinationId);
      final savedFuture = context.read<SavedDestinationProvider>().findFor(widget.destinationId);
      final destination = await destinationFuture;
      final saved = await savedFuture;
      if (mounted) {
        setState(() {
          _destination = destination;
          _saved = saved;
        });
      }
      await _reloadReviews();
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  /// Rating, approved list and the user's own review, e.g. after submitting one.
  Future<void> _reloadReviews() async {
    final reviewProvider = context.read<ReviewProvider>();
    final destinationProvider = context.read<DestinationProvider>();
    setState(() {
      _reviews.clear();
      _reviewPage = 0;
      _reviewTotal = 0;
    });
    try {
      final mine = await reviewProvider.mineFor(widget.destinationId);
      final destination = await destinationProvider.getById(widget.destinationId);
      if (mounted) {
        setState(() {
          _myReviews = mine;
          _destination = destination;
        });
      }
      await _loadMoreReviews();
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _loadMoreReviews() async {
    if (_isLoadingReviews) {
      return;
    }
    setState(() => _isLoadingReviews = true);
    try {
      final result = await context
          .read<ReviewProvider>()
          .approvedFor(widget.destinationId, page: _reviewPage + 1, pageSize: _reviewPageSize);
      if (mounted) {
        setState(() {
          _reviews.addAll(result.items);
          _reviewTotal = result.totalCount ?? _reviews.length;
          _reviewPage++;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isLoadingReviews = false);
      }
    }
  }

  Future<void> _toggleSaved(Destination destination) async {
    final saved = _saved;
    if (saved != null) {
      final confirmed = await showConfirmDialog(
        context,
        title: 'Uklanjanje iz sačuvanih',
        message: 'Ukloniti "${destination.name}" iz sačuvanih destinacija?',
        confirmLabel: 'Ukloni',
      );
      if (!confirmed || !mounted) {
        return;
      }
    }

    setState(() => _isSaving = true);
    try {
      final provider = context.read<SavedDestinationProvider>();
      if (saved == null) {
        final created = await provider.save(destination.id);
        if (mounted) {
          setState(() => _saved = created);
          showSuccessMessage(context, '${destination.name} je sačuvana.');
        }
      } else {
        await provider.remove(saved.id);
        if (mounted) {
          setState(() => _saved = null);
          showSuccessMessage(context, '${destination.name} je uklonjena iz sačuvanih.');
        }
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

  Future<void> _addToCollection(Destination destination) async {
    final message = await AddToCollectionSheet.show(context, destination);
    if (message != null && mounted) {
      showSuccessMessage(context, message);
    }
  }

  Future<void> _addToPlan(Destination destination) async {
    final message = await TripItemSheet.show(context, destination: destination);
    if (message != null && mounted) {
      showSuccessMessage(context, message);
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      title: _destination?.name ?? 'Destinacija',
      child: _buildBody(),
    );
  }

  Widget _buildBody() {
    final destination = _destination;
    if (_error != null) {
      return Center(
        child: Padding(padding: const EdgeInsets.all(24), child: Text(_error!)),
      );
    }
    if (destination == null) {
      return const Center(child: CircularProgressIndicator());
    }

    final theme = Theme.of(context);
    final width = MediaQuery.sizeOf(context).width;

    return ListView(
      padding: const EdgeInsets.only(bottom: 32),
      children: [
        _buildGallery(destination, width),
        Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(destination.name, style: theme.textTheme.headlineSmall),
              const SizedBox(height: 4),
              Row(
                children: [
                  Icon(Icons.place_outlined, size: 18, color: theme.colorScheme.primary),
                  const SizedBox(width: 4),
                  Expanded(
                    child: Text(
                      [destination.cityName, ?destination.category?.name].join(' · '),
                      style: theme.textTheme.bodyMedium,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              RatingLabel(
                averageRating: destination.averageRating,
                reviewCount: destination.reviewCount,
              ),
              const SizedBox(height: 12),
              _buildActions(destination),
              const SizedBox(height: 12),
              KeywordTags(keywords: destination.keywords, maxTags: 10),
              const SizedBox(height: 16),
              Text(destination.description, style: theme.textTheme.bodyLarge),
              const Divider(height: 40),
              _buildReviews(destination),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildActions(Destination destination) {
    final isSaved = _saved != null;
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: [
        FilledButton.tonalIcon(
          onPressed: _isSaving ? null : () => _toggleSaved(destination),
          icon: Icon(isSaved ? Icons.bookmark : Icons.bookmark_border),
          label: Text(isSaved ? 'Sačuvano' : 'Sačuvaj'),
        ),
        OutlinedButton.icon(
          onPressed: () => _addToCollection(destination),
          icon: const Icon(Icons.collections_bookmark_outlined),
          label: const Text('U kolekciju'),
        ),
        OutlinedButton.icon(
          onPressed: () => _addToPlan(destination),
          icon: const Icon(Icons.map_outlined),
          label: const Text('U plan putovanja'),
        ),
      ],
    );
  }

  Widget _buildReviews(Destination destination) {
    final theme = Theme.of(context);
    final myReviews = _myReviews;
    final activeReview = myReviews?.where((r) => r.status != ReviewStatus.rejected).firstOrNull;
    final rejected = myReviews?.where((r) => r.status == ReviewStatus.rejected).firstOrNull;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text('Recenzije ($_reviewTotal)', style: theme.textTheme.titleMedium),
        const SizedBox(height: 12),
        if (myReviews != null) ...[
          if (activeReview?.status == ReviewStatus.pending)
            _ReviewNotice(
              icon: Icons.hourglass_top,
              text: 'Vaša recenzija (${activeReview!.rating}★) čeka odobrenje administratora. '
                  'Dobićete obavještenje kad bude objavljena.',
            )
          else if (activeReview == null) ...[
            if (rejected != null)
              _ReviewNotice(
                icon: Icons.info_outline,
                text: 'Vaša prethodna recenzija je odbijena'
                    '${rejected.moderationReason == null ? '' : ': ${rejected.moderationReason}'}. '
                    'Možete napisati novu.',
              ),
            ReviewForm(destinationId: destination.id, onSubmitted: _reloadReviews),
          ],
          const SizedBox(height: 12),
        ],
        if (_reviews.isEmpty && !_isLoadingReviews)
          Text('Još nema objavljenih recenzija.', style: theme.textTheme.bodyMedium)
        else
          for (final review in _reviews) _ReviewTile(review: review),
        if (_reviews.length < _reviewTotal)
          Center(
            child: _isLoadingReviews
                ? const Padding(
                    padding: EdgeInsets.all(8),
                    child: CircularProgressIndicator(),
                  )
                : TextButton(
                    onPressed: _loadMoreReviews,
                    child: const Text('Prikaži još recenzija'),
                  ),
          ),
      ],
    );
  }

  Widget _buildGallery(Destination destination, double width) {
    final images = destination.images;
    if (images.isEmpty) {
      return Image.asset(
        'assets/images/destination_placeholder.jpg',
        height: _galleryHeight,
        width: width,
        fit: BoxFit.cover,
      );
    }

    return SizedBox(
      height: _galleryHeight,
      child: Stack(
        children: [
          PageView.builder(
            itemCount: images.length,
            onPageChanged: (index) => setState(() => _imageIndex = index),
            itemBuilder: (context, index) => NetworkThumbnail(
              imageUrl: images[index].imageUrl,
              width: width,
              height: _galleryHeight,
              borderRadius: 0,
            ),
          ),
          if (images.length > 1)
            Positioned(
              bottom: 8,
              right: 12,
              child: Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                decoration: BoxDecoration(
                  color: Colors.black54,
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Text(
                  '${_imageIndex + 1}/${images.length}',
                  style: const TextStyle(color: Colors.white, fontSize: 12),
                ),
              ),
            ),
        ],
      ),
    );
  }
}

class _ReviewNotice extends StatelessWidget {
  const _ReviewNotice({required this.icon, required this.text});

  final IconData icon;
  final String text;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Card(
      color: scheme.secondaryContainer,
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, color: scheme.onSecondaryContainer),
            const SizedBox(width: 10),
            Expanded(
              child: Text(text, style: TextStyle(color: scheme.onSecondaryContainer)),
            ),
          ],
        ),
      ),
    );
  }
}

class _ReviewTile extends StatelessWidget {
  const _ReviewTile({required this.review});

  static const _maxRating = 5;

  final Review review;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              for (var star = 1; star <= _maxRating; star++)
                Icon(
                  star <= review.rating ? Icons.star : Icons.star_border,
                  size: 16,
                  color: theme.colorScheme.tertiary,
                ),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  '${review.reviewerDisplayName} · ${formatDate(review.createdAt)}',
                  style: theme.textTheme.bodySmall,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
          if (review.comment != null && review.comment!.isNotEmpty) ...[
            const SizedBox(height: 4),
            Text(review.comment!),
          ],
        ],
      ),
    );
  }
}
