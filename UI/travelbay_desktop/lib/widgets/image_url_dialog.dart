import 'package:flutter/material.dart';

import '../utils/validators.dart';

/// Asks for an absolute image URL; resolves to the trimmed URL, or null when cancelled.
class ImageUrlDialog extends StatefulWidget {
  const ImageUrlDialog({super.key});

  @override
  State<ImageUrlDialog> createState() => _ImageUrlDialogState();
}

class _ImageUrlDialogState extends State<ImageUrlDialog> {
  final _formKey = GlobalKey<FormState>();
  final _controller = TextEditingController();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _submit() {
    if (_formKey.currentState!.validate()) {
      Navigator.pop(context, _controller.text.trim());
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Dodaj sliku preko URL-a'),
      content: SizedBox(
        width: 480,
        child: Form(
          key: _formKey,
          child: TextFormField(
            controller: _controller,
            autofocus: true,
            decoration: const InputDecoration(
              labelText: 'URL slike *',
              hintText: 'https://primjer.com/slika.jpg',
            ),
            validator: Validators.imageUrl,
            onFieldSubmitted: (_) => _submit(),
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Odustani'),
        ),
        FilledButton(onPressed: _submit, child: const Text('Dodaj')),
      ],
    );
  }
}
