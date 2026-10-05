import 'package:flutter/material.dart';

/// Compact done / missing indicator (e.g. "KW ✓", "IMG ✗") with an explaining tooltip.
class StatusChip extends StatelessWidget {
  const StatusChip({
    super.key,
    required this.label,
    required this.isDone,
    required this.tooltip,
  });

  final String label;
  final bool isDone;
  final String tooltip;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final background =
        isDone ? scheme.secondaryContainer : scheme.errorContainer;
    final foreground =
        isDone ? scheme.onSecondaryContainer : scheme.onErrorContainer;

    return Tooltip(
      message: tooltip,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
        decoration: BoxDecoration(
          color: background,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              label,
              style: TextStyle(
                color: foreground,
                fontSize: 12,
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(width: 3),
            Icon(isDone ? Icons.check : Icons.close, size: 14, color: foreground),
          ],
        ),
      ),
    );
  }
}

/// The two AI-enrichment indicators shown for every destination.
class AiStatusChips extends StatelessWidget {
  const AiStatusChips({
    super.key,
    required this.hasKeywords,
    required this.hasImages,
  });

  final bool hasKeywords;
  final bool hasImages;

  @override
  Widget build(BuildContext context) {
    return Wrap(
      spacing: 6,
      runSpacing: 4,
      children: [
        StatusChip(
          label: 'KW',
          isDone: hasKeywords,
          tooltip: hasKeywords ? 'Ima ključne riječi' : 'Nema ključnih riječi',
        ),
        StatusChip(
          label: 'IMG',
          isDone: hasImages,
          tooltip: hasImages ? 'Ima bar jednu sliku' : 'Nema nijednu sliku',
        ),
      ],
    );
  }
}
