import 'package:flutter/material.dart';

enum ChipTone { neutral, positive, warning, negative }

/// Small read-only status label (e.g. "Aktivan", "Na čekanju").
class ToneChip extends StatelessWidget {
  const ToneChip({super.key, required this.label, required this.tone});

  final String label;
  final ChipTone tone;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final (background, foreground) = switch (tone) {
      ChipTone.positive => (scheme.secondaryContainer, scheme.onSecondaryContainer),
      ChipTone.warning => (scheme.tertiaryContainer, scheme.onTertiaryContainer),
      ChipTone.negative => (scheme.errorContainer, scheme.onErrorContainer),
      ChipTone.neutral => (scheme.surfaceContainerHighest, scheme.onSurfaceVariant),
    };

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 3),
      decoration: BoxDecoration(
        color: background,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(
        label,
        style: TextStyle(
          color: foreground,
          fontSize: 12,
          fontWeight: FontWeight.w600,
        ),
      ),
    );
  }
}
