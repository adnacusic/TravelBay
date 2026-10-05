/// Form field validators; messages are shown under the control.
class Validators {
  Validators._();

  static String? Function(String?) requiredText(
    String fieldLabel, {
    int? maxLength,
  }) {
    return (value) {
      final text = value?.trim() ?? '';
      if (text.isEmpty) {
        return '$fieldLabel je obavezno polje.';
      }
      if (maxLength != null && text.length > maxLength) {
        return '$fieldLabel može imati najviše $maxLength znakova (trenutno ${text.length}).';
      }
      return null;
    };
  }

  static String? Function(String?) optionalText(
    String fieldLabel, {
    required int maxLength,
  }) {
    return (value) {
      final text = value?.trim() ?? '';
      if (text.length > maxLength) {
        return '$fieldLabel može imati najviše $maxLength znakova (trenutno ${text.length}).';
      }
      return null;
    };
  }

  /// [what] is the noun as it reads after "Odaberite", e.g. "kategoriju".
  static String? Function(T?) requiredSelection<T>(String what) {
    return (value) => value == null ? 'Odaberite $what.' : null;
  }

  static String? imageUrl(String? value) {
    final text = value?.trim() ?? '';
    if (text.isEmpty) {
      return 'URL slike je obavezan.';
    }
    final uri = Uri.tryParse(text);
    final valid = uri != null &&
        uri.isAbsolute &&
        (uri.scheme == 'http' || uri.scheme == 'https') &&
        uri.host.isNotEmpty;
    if (!valid) {
      return 'Unesite punu adresu, npr. https://primjer.com/slika.jpg';
    }
    if (text.length > 500) {
      return 'URL može imati najviše 500 znakova.';
    }
    return null;
  }
}
