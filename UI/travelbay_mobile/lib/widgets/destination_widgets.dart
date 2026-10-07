import 'package:flutter/material.dart';

import '../models/destination.dart';
import 'network_thumbnail.dart';

/// Average rating with the number of approved reviews, e.g. "★ 4,5 (12)".
class RatingLabel extends StatelessWidget {
  const RatingLabel({super.key, required this.averageRating, required this.reviewCount});

  final double? averageRating;
  final int reviewCount;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final rating = averageRating;
    if (rating == null) {
      return Text('Još nema ocjena', style: theme.textTheme.bodySmall);
    }
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(Icons.star, size: 16, color: theme.colorScheme.tertiary),
        const SizedBox(width: 3),
        Text(
          '${rating.toStringAsFixed(1).replaceAll('.', ',')} ($reviewCount)',
          style: theme.textTheme.bodySmall?.copyWith(fontWeight: FontWeight.w600),
        ),
      ],
    );
  }
}

/// AI keywords of a destination as small tags (at most [maxTags]).
class KeywordTags extends StatelessWidget {
  const KeywordTags({super.key, required this.keywords, this.maxTags = 3});

  final String? keywords;
  final int maxTags;

  @override
  Widget build(BuildContext context) {
    final tags = (keywords ?? '')
        .split(',')
        .map((k) => k.trim())
        .where((k) => k.isNotEmpty)
        .take(maxTags)
        .toList();
    if (tags.isEmpty) {
      return const SizedBox.shrink();
    }

    final scheme = Theme.of(context).colorScheme;
    return Wrap(
      spacing: 6,
      runSpacing: 4,
      children: [
        for (final tag in tags)
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
            decoration: BoxDecoration(
              color: scheme.secondaryContainer,
              borderRadius: BorderRadius.circular(10),
            ),
            child: Text(
              '#$tag',
              style: TextStyle(fontSize: 11, color: scheme.onSecondaryContainer),
            ),
          ),
      ],
    );
  }
}

/// Destination row for vertical lists: image, name, place, rating and AI tags.
class DestinationTile extends StatelessWidget {
  const DestinationTile({super.key, required this.destination, required this.onTap});

  final Destination destination;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final category = destination.category?.name;

    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(10),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              NetworkThumbnail(
                imageUrl: destination.coverImage?.imageUrl,
                width: 96,
                height: 80,
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      destination.name,
                      style: theme.textTheme.titleSmall,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 2),
                    Text(
                      [destination.cityName, ?category].join(' · '),
                      style: theme.textTheme.bodySmall,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 4),
                    RatingLabel(
                      averageRating: destination.averageRating,
                      reviewCount: destination.reviewCount,
                    ),
                    const SizedBox(height: 6),
                    KeywordTags(keywords: destination.keywords),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Compact destination row for the user's own lists (saved, collection, history):
/// cover image, name, place and a line such as the date it was saved.
class DestinationRefTile extends StatelessWidget {
  const DestinationRefTile({
    super.key,
    required this.name,
    required this.cityName,
    required this.imageUrl,
    required this.detail,
    required this.onTap,
    this.trailing,
  });

  final String name;
  final String cityName;
  final String? imageUrl;
  final String detail;
  final VoidCallback onTap;
  final Widget? trailing;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.fromLTRB(10, 10, 4, 10),
          child: Row(
            children: [
              NetworkThumbnail(imageUrl: imageUrl, width: 72, height: 60),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      name,
                      style: theme.textTheme.titleSmall,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    if (cityName.isNotEmpty)
                      Text(cityName, style: theme.textTheme.bodySmall),
                    const SizedBox(height: 2),
                    Text(detail, style: theme.textTheme.labelSmall),
                  ],
                ),
              ),
              ?trailing,
            ],
          ),
        ),
      ),
    );
  }
}
