import 'package:flutter/material.dart';

import '../../models/enums.dart';
import '../../models/review.dart';
import '../../utils/formatters.dart';
import '../../widgets/tone_chip.dart';

class ReviewStatusChip extends StatelessWidget {
  const ReviewStatusChip({super.key, required this.status});

  final ReviewStatus status;

  @override
  Widget build(BuildContext context) {
    return ToneChip(
      label: status.label,
      tone: switch (status) {
        ReviewStatus.pending => ChipTone.warning,
        ReviewStatus.approved => ChipTone.positive,
        ReviewStatus.rejected => ChipTone.negative,
      },
    );
  }
}

class RatingStars extends StatelessWidget {
  const RatingStars({super.key, required this.rating, this.size = 18});

  static const maxRating = 5;

  final int rating;
  final double size;

  @override
  Widget build(BuildContext context) {
    final color = Theme.of(context).colorScheme.tertiary;
    return Tooltip(
      message: 'Ocjena $rating od $maxRating',
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          for (var i = 1; i <= maxRating; i++)
            Icon(i <= rating ? Icons.star : Icons.star_border, size: size, color: color),
        ],
      ),
    );
  }
}

/// Why moderation actions are not available for an already moderated review.
String? moderationBlockedReason(Review review) => switch (review.status) {
      ReviewStatus.pending => null,
      ReviewStatus.approved => 'Recenzija je već odobrena; moderacija je završena.',
      ReviewStatus.rejected => 'Recenzija je već odbijena; moderacija je završena.',
    };

/// Full review with its moderation trail (who, when, why).
class ReviewDetailsDialog extends StatelessWidget {
  const ReviewDetailsDialog({super.key, required this.review});

  final Review review;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return AlertDialog(
      titlePadding: const EdgeInsets.fromLTRB(24, 16, 8, 0),
      title: Row(
        children: [
          Expanded(child: Text(review.destinationName)),
          IconButton(
            tooltip: 'Zatvori',
            icon: const Icon(Icons.close),
            onPressed: () => Navigator.pop(context),
          ),
        ],
      ),
      content: SizedBox(
        width: 520,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                RatingStars(rating: review.rating),
                const SizedBox(width: 12),
                ReviewStatusChip(status: review.status),
              ],
            ),
            const SizedBox(height: 12),
            Text(
              '${review.reviewerDisplayName} · ${formatDateTime(review.createdAt)}',
              style: theme.textTheme.bodySmall,
            ),
            const SizedBox(height: 12),
            Text(
              (review.comment?.trim().isNotEmpty ?? false)
                  ? review.comment!
                  : 'Korisnik nije ostavio komentar.',
            ),
            if (review.status != ReviewStatus.pending) ...[
              const Divider(height: 32),
              Text('Moderacija', style: theme.textTheme.titleSmall),
              const SizedBox(height: 8),
              Text('Moderator: ${review.moderatedByDisplayName ?? '-'}'),
              Text('Vrijeme: ${formatDateTime(review.moderatedAt)}'),
              if (review.moderationReason != null)
                Text('Razlog odbijanja: ${review.moderationReason}'),
            ],
          ],
        ),
      ),
      actions: [
        OutlinedButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Nazad'),
        ),
      ],
    );
  }
}
