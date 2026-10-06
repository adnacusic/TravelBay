import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../models/review.dart';
import '../../providers/review_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/validators.dart';
import '../../widgets/form_dialog.dart';

/// Rejecting needs a reason (stored in the audit trail and sent to the author).
/// Resolves to true when the review was rejected.
class RejectReviewDialog extends StatefulWidget {
  const RejectReviewDialog({super.key, required this.review});

  final Review review;

  @override
  State<RejectReviewDialog> createState() => _RejectReviewDialogState();
}

class _RejectReviewDialogState extends State<RejectReviewDialog> {
  static const _maxReasonLength = 500;

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  Future<void> _submit() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    setState(() => _isSaving = true);
    try {
      await context.read<ReviewProvider>().reject(
            widget.review.id,
            (form.value['reason'] as String).trim(),
          );
      if (mounted) {
        Navigator.pop(context, true);
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
    return FormDialog(
      title: 'Odbijanje recenzije',
      submitLabel: 'Odbij recenziju',
      isSubmitting: _isSaving,
      onSubmit: _submit,
      width: 520,
      child: FormBuilder(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              '${widget.review.reviewerDisplayName} · ${widget.review.destinationName}',
              style: Theme.of(context).textTheme.titleSmall,
            ),
            const SizedBox(height: 4),
            Text(widget.review.comment ?? 'Bez komentara.'),
            const SizedBox(height: 16),
            FormBuilderTextField(
              name: 'reason',
              autofocus: true,
              minLines: 3,
              maxLines: 6,
              maxLength: _maxReasonLength,
              decoration: const InputDecoration(
                labelText: 'Razlog odbijanja *',
                helperText: 'Razlog vidi autor recenzije u obavještenju.',
                alignLabelWithHint: true,
              ),
              validator: Validators.requiredText(
                'Razlog odbijanja',
                maxLength: _maxReasonLength,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
