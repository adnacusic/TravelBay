import 'dart:convert';

/// An API error that can be shown to the user: [message] is the summary line,
/// [fieldErrors] maps request property names (camelCase) to their messages.
class ApiClientException implements Exception {
  ApiClientException(this.message, {this.fieldErrors = const {}});

  final String message;
  final Map<String, List<String>> fieldErrors;

  @override
  String toString() => message;
}

/// The session could not be renewed; the app is already back on the login screen,
/// so screens should not show an extra error for it.
class SessionExpiredException extends ApiClientException {
  SessionExpiredException() : super('Sesija je istekla. Prijavite se ponovo.');
}

/// Parses the TravelBay ExceptionFilter body: `{ "message": "...", "errors": { "Key": ["..."] } }`.
class ApiErrorParser {
  ApiErrorParser._();

  static const _generalErrorKeys = {
    'clientError',
    'notFound',
    'businessError',
    'serverError',
  };

  static ApiClientException parse(String body, {required String fallback}) {
    final trimmed = body.trim();
    if (trimmed.isEmpty) {
      return ApiClientException(fallback);
    }

    try {
      final decoded = jsonDecode(trimmed);
      if (decoded is! Map) {
        return ApiClientException(fallback);
      }

      final fieldErrors = <String, List<String>>{};
      final errors = decoded['errors'];
      if (errors is Map) {
        errors.forEach((key, value) {
          final name = key.toString();
          if (_generalErrorKeys.contains(name) || name.isEmpty) {
            return;
          }
          final messages = value is List
              ? value.map((e) => e.toString()).toList()
              : [value.toString()];
          fieldErrors[_camelCase(name)] = messages;
        });
      }

      // ASP.NET model validation (ProblemDetails) has no "message", only "errors".
      final message = decoded['message'];
      final summary = message is String && message.trim().isNotEmpty
          ? message.trim()
          : fieldErrors.values.expand((m) => m).firstOrNull ?? fallback;
      return ApiClientException(summary, fieldErrors: fieldErrors);
    } on FormatException {
      return ApiClientException(fallback);
    }
  }

  static String _camelCase(String name) =>
      name[0].toLowerCase() + name.substring(1);
}
