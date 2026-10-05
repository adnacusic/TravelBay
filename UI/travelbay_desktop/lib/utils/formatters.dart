String _twoDigits(int value) => value.toString().padLeft(2, '0');

/// API dates are UTC; the admin sees local time in dd.MM.yyyy. format.
String formatDate(DateTime? value) {
  if (value == null) {
    return '-';
  }
  final local = value.toLocal();
  return '${_twoDigits(local.day)}.${_twoDigits(local.month)}.${local.year}.';
}

String formatDateTime(DateTime? value) {
  if (value == null) {
    return '-';
  }
  final local = value.toLocal();
  return '${formatDate(local)} ${_twoDigits(local.hour)}:${_twoDigits(local.minute)}';
}

/// The API serializes DateTime without an offset; its values are always UTC.
DateTime? parseUtc(String? value) {
  if (value == null) {
    return null;
  }
  final hasOffset = value.endsWith('Z') || RegExp(r'[+-]\d{2}:\d{2}$').hasMatch(value);
  return DateTime.parse(hasOffset ? value : '${value}Z');
}
