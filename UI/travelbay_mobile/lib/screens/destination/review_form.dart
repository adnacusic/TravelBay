import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../providers/review_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';

/// New review for a destination: rating 1–5 (required) and an optional comment.
/// The review waits for admin approval before it becomes public.
class ReviewForm extends StatefulWidget {
  const ReviewForm({super.key, required this.destinationId, required this.onSubmitted});

  final int destinationId;
  final VoidCallback onSubmitted;

  @override
  State<ReviewForm> createState() => _ReviewFormState();
}

class _ReviewFormState extends State<ReviewForm> {
  /// Same limit as the API ReviewInsertValidator.
  static const _maxCommentLength = 1000;
  static const _maxRating = 5;

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  Future<void> _submit() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final comment = (form.value['comment'] as String?)?.trim() ?? '';
    setState(() => _isSaving = true);
    try {
      await context.read<ReviewProvider>().insert({
        'destinationId': widget.destinationId,
        'rating': form.value['rating'] as int,
        'comment': comment.isEmpty ? null : comment,
      });
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Hvala! Recenzija je poslana i biće vidljiva nakon odobrenja.'),
          ),
        );
        widget.onSubmitted();
      }
    } on Exception catch (e) {
      if (mounted) {
        showFormErrors(context, _formKey.currentState, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSaving = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: FormBuilder(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text('Ocijeni destinaciju', style: theme.textTheme.titleSmall),
              FormBuilderField<int>(
                name: 'rating',
                validator: (value) => value == null ? 'Odaberite ocjenu od 1 do 5 zvjezdica.' : null,
                builder: (field) => Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        for (var star = 1; star <= _maxRating; star++)
                          IconButton(
                            tooltip: '$star / $_maxRating',
                            onPressed: _isSaving ? null : () => field.didChange(star),
                            icon: Icon(
                              (field.value ?? 0) >= star ? Icons.star : Icons.star_border,
                              color: theme.colorScheme.tertiary,
                              size: 32,
                            ),
                          ),
                      ],
                    ),
                    if (field.errorText != null)
                      Text(
                        field.errorText!,
                        style: TextStyle(color: theme.colorScheme.error, fontSize: 12),
                      ),
                  ],
                ),
              ),
              const SizedBox(height: 8),
              FormBuilderTextField(
                name: 'comment',
                minLines: 2,
                maxLines: 5,
                maxLength: _maxCommentLength,
                decoration: const InputDecoration(
                  labelText: 'Komentar (opcionalno)',
                  alignLabelWithHint: true,
                ),
                validator: Validators.optionalText('Komentar', maxLength: _maxCommentLength),
              ),
              const SizedBox(height: 8),
              FilledButton(
                onPressed: _isSaving ? null : _submit,
                child: _isSaving
                    ? const SizedBox.square(
                        dimension: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Text('Pošalji recenziju'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
