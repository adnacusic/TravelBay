import '../models/destination_image.dart';
import 'base_provider.dart';

/// Only insert (by URL or uploaded file) and remove are exposed by the API;
/// images are read as part of a destination.
class DestinationImageProvider extends BaseProvider<DestinationImage> {
  DestinationImageProvider() : super('DestinationImages');

  @override
  DestinationImage fromJson(Map<String, dynamic> json) =>
      DestinationImage.fromJson(json);

  Future<DestinationImage> addFromUrl(int destinationId, String imageUrl) =>
      insert({'destinationId': destinationId, 'imageUrl': imageUrl});

  Future<DestinationImage> upload(
    int destinationId, {
    required String fileName,
    required String contentType,
    required String base64Content,
  }) =>
      insert({
        'destinationId': destinationId,
        'fileName': fileName,
        'contentType': contentType,
        'base64Content': base64Content,
      });
}
