import 'package:flutter/material.dart';

import '../models/enums.dart';

class TripStatusChip extends StatelessWidget {
  const TripStatusChip({super.key, required this.status});

  final TripPlanStatus status;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final (background, foreground) = switch (status) {
      TripPlanStatus.draft => (scheme.surfaceContainerHighest, scheme.onSurfaceVariant),
      TripPlanStatus.active => (scheme.primaryContainer, scheme.onPrimaryContainer),
      TripPlanStatus.completed => (scheme.secondaryContainer, scheme.onSecondaryContainer),
      TripPlanStatus.cancelled => (scheme.errorContainer, scheme.onErrorContainer),
    };

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 3),
      decoration: BoxDecoration(color: background, borderRadius: BorderRadius.circular(12)),
      child: Text(
        status.label,
        style: TextStyle(color: foreground, fontSize: 12, fontWeight: FontWeight.w600),
      ),
    );
  }
}
