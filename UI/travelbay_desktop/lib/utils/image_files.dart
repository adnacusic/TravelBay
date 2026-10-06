import 'dart:convert';
import 'dart:typed_data';

import 'package:file_picker/file_picker.dart';

/// An image file chosen on this computer, ready to be uploaded as base64.
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

class PickedImages {
  PickedImages(this.images, this.rejected);

  final List<PickedImage> images;

  /// One line per file that was not accepted, with the reason.
  final List<String> rejected;
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

Future<PickedImages> pickImageFiles({bool allowMultiple = true}) async {
  final result = await FilePicker.pickFiles(
    allowMultiple: allowMultiple,
    type: FileType.image,
  );
  if (result == null) {
    return PickedImages([], []);
  }

  final images = <PickedImage>[];
  final rejected = <String>[];
  for (final file in result.files) {
    final contentType = _contentTypes[file.extension?.toLowerCase() ?? ''];
    if (contentType == null) {
      rejected.add('${file.name}: format nije podržan (JPG, PNG, GIF, WEBP, BMP).');
      continue;
    }
    if (file.size > maxImageBytes) {
      rejected.add('${file.name}: slika je veća od 5 MB.');
      continue;
    }

    images.add(
      PickedImage(
        fileName: file.name,
        contentType: contentType,
        bytes: await file.xFile.readAsBytes(),
      ),
    );
  }
  return PickedImages(images, rejected);
}
