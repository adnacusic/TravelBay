import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';

import 'api_client_exception.dart';
import 'dialogs.dart';

/// Shows API errors where they belong: field errors under their control,
/// anything not tied to a visible field in a dialog.
void showFormErrors(BuildContext context, FormBuilderState? form, Object error) {
  if (error is! ApiClientException || form == null) {
    showErrorDialog(context, error);
    return;
  }

  final unmatched = <String>[];
  error.fieldErrors.forEach((key, messages) {
    final field = form.fields[key];
    if (field != null) {
      field.invalidate(messages.join(' '));
    } else {
      unmatched.addAll(messages);
    }
  });

  if (error.fieldErrors.isEmpty || unmatched.isNotEmpty) {
    showErrorDialog(
      context,
      unmatched.isEmpty ? error.message : unmatched.join('\n'),
    );
  }
}
