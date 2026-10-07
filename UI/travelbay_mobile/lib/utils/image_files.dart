import 'dart:convert';
import 'dart:typed_data';

import 'package:file_picker/file_picker.dart';

/// An image chosen on the phone, ready to be uploaded as base64.
class PickedImage {
  PickedImage({
    required this.fileName,
    required this.contentType,
    required this.bytes,
  });

  final String fileName;
  final String contentType;
  final Uint8List bytes;

  String get base64Content => base64Encode(bytes);
}

/// Thrown when the chosen file cannot be used; the message is shown to the user.
class ImagePickException implements Exception {
  ImagePickException(this.message);

  final String message;

  @override
  String toString() => message;
}

/// Same limits as the API (ImageRules): image types only, at most 5 MB.
const int maxImageBytes = 5 * 1024 * 1024;

const _contentTypes = {
  'jpg': 'image/jpeg',
  'jpeg': 'image/jpeg',
  'png': 'image/png',
  'gif': 'image/gif',
  'webp': 'image/webp',
  'bmp': 'image/bmp',
};

/// Null when the user closed the picker without choosing.
Future<PickedImage?> pickImageFile() async {
  final file = await FilePicker.pickFile(type: FileType.image);
  if (file == null) {
    return null;
  }

  final contentType = _contentTypes[file.extension?.toLowerCase() ?? ''];
  if (contentType == null) {
    throw ImagePickException('Format slike nije podržan. Odaberite JPG, PNG, GIF, WEBP ili BMP.');
  }
  final length = await file.length();
  if (length != null && length > maxImageBytes) {
    throw ImagePickException('Slika je veća od 5 MB. Odaberite manju sliku.');
  }

  final bytes = await file.readAsBytes();
  if (bytes.length > maxImageBytes) {
    throw ImagePickException('Slika je veća od 5 MB. Odaberite manju sliku.');
  }
  return PickedImage(fileName: file.name, contentType: contentType, bytes: bytes);
}
