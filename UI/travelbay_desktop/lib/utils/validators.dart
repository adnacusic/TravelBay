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

  static String? Function(String?) requiredTextRange(
    String fieldLabel, {
    required int minLength,
    required int maxLength,
  }) {
    final required = requiredText(fieldLabel, maxLength: maxLength);
    return (value) {
      final error = required(value);
      if (error != null) {
        return error;
      }
      final length = value!.trim().length;
      return length < minLength
          ? '$fieldLabel mora imati najmanje $minLength znaka (trenutno $length).'
          : null;
    };
  }

  static final _emailPattern = RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$');

  /// Same format the API accepts: optional leading +, then 6–20 digits, spaces, "/" or "-".
  static final _phonePattern = RegExp(r'^\+?[0-9 /-]{6,20}$');

  static String? email(String? value) {
    final text = value?.trim() ?? '';
    if (text.isEmpty) {
      return 'Email je obavezno polje.';
    }
    if (text.length > 100) {
      return 'Email može imati najviše 100 znakova.';
    }
    return _emailPattern.hasMatch(text)
        ? null
        : 'Unesite ispravan email, npr. ime@primjer.com';
  }

  static String? optionalPhone(String? value) {
    final text = value?.trim() ?? '';
    if (text.isEmpty) {
      return null;
    }
    return _phonePattern.hasMatch(text)
        ? null
        : 'Unesite broj u formatu +387 61 234 567 (samo cifre, razmaci, "/" i "-").';
  }

  static const minPasswordLength = 6;

  static String? newPassword(String? value) {
    final text = value ?? '';
    if (text.isEmpty) {
      return 'Nova lozinka je obavezna.';
    }
    if (text.length < minPasswordLength) {
      return 'Lozinka mora imati najmanje $minPasswordLength znakova.';
    }
    if (text.length > 100) {
      return 'Lozinka može imati najviše 100 znakova.';
    }
    return null;
  }

  /// [password] returns the current value of the "new password" field.
  static String? Function(String?) passwordConfirmation(
    String? Function() password,
  ) {
    return (value) {
      if (value == null || value.isEmpty) {
        return 'Potvrdite novu lozinku.';
      }
      return value == password() ? null : 'Lozinke se ne podudaraju.';
    };
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
