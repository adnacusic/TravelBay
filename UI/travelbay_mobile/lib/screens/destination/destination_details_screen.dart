import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/destination.dart';
import '../../providers/destination_provider.dart';
import '../../providers/user_activity_provider.dart';
import '../../widgets/destination_widgets.dart';
import '../../widgets/network_thumbnail.dart';

/// Destination details. Opening it records a ViewHistory entry, which feeds the recommender.
class DestinationDetailsScreen extends StatefulWidget {
  const DestinationDetailsScreen({super.key, required this.destinationId});

  final int destinationId;

  @override
  State<DestinationDetailsScreen> createState() => _DestinationDetailsScreenState();
}

class _DestinationDetailsScreenState extends State<DestinationDetailsScreen> {
  static const _galleryHeight = 240.0;

  Destination? _destination;
  String? _error;
  int _imageIndex = 0;

  @override
  void initState() {
    super.initState();
    _recordView();
    _load();
  }

  Future<void> _load() async {
    try {
      final destination =
          await context.read<DestinationProvider>().getById(widget.destinationId);
      if (mounted) {
        setState(() => _destination = destination);
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  /// A failed view record must not block the details; it is only logged.
  Future<void> _recordView() async {
    try {
      await context.read<ViewHistoryProvider>().record(widget.destinationId);
    } on Exception catch (e) {
      debugPrint('ViewHistory record failed for destination ${widget.destinationId}: $e');
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
              KeywordTags(keywords: destination.keywords, maxTags: 10),
              const SizedBox(height: 16),
              Text(destination.description, style: theme.textTheme.bodyLarge),
            ],
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
