import 'package:flutter/material.dart';

/// Small entity form shown as a dialog: title with "X" in the top-right corner,
/// content, and "Odustani" + primary action at the bottom.
class FormDialog extends StatelessWidget {
  const FormDialog({
    super.key,
    required this.title,
    required this.child,
    required this.submitLabel,
    required this.onSubmit,
    this.isSubmitting = false,
    this.width = 480,
  });

  final String title;
  final Widget child;
  final String submitLabel;
  final VoidCallback onSubmit;
  final bool isSubmitting;
  final double width;

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      titlePadding: const EdgeInsets.fromLTRB(24, 16, 8, 0),
      title: Row(
        children: [
          Expanded(child: Text(title)),
          IconButton(
            tooltip: 'Zatvori',
            icon: const Icon(Icons.close),
            onPressed: isSubmitting ? null : () => Navigator.pop(context),
          ),
        ],
      ),
      content: SizedBox(
        width: width,
        child: SingleChildScrollView(child: child),
      ),
      actions: [
        OutlinedButton(
          onPressed: isSubmitting ? null : () => Navigator.pop(context),
          child: const Text('Odustani'),
        ),
        FilledButton(
          onPressed: isSubmitting ? null : onSubmit,
          child: isSubmitting
              ? const SizedBox.square(
                  dimension: 18,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : Text(submitLabel),
        ),
      ],
    );
  }
}
