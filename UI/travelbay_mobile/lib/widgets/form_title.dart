import 'package:flutter/material.dart';

/// Title row for dialogs and bottom sheets with a form: the title on the left and
/// an "X" in the top right corner that closes without saving.
class FormTitle extends StatelessWidget {
  const FormTitle({super.key, required this.title, this.enabled = true});

  final String title;

  /// False while saving, so the form cannot be closed halfway.
  final bool enabled;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(child: Text(title, style: Theme.of(context).textTheme.titleMedium)),
        IconButton(
          tooltip: 'Zatvori',
          icon: const Icon(Icons.close),
          onPressed: enabled ? () => Navigator.pop(context) : null,
        ),
      ],
    );
  }
}
