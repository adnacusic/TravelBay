import '../models/collection.dart';
import 'base_provider.dart';

/// The signed-in user's collections of destinations.
class CollectionProvider extends BaseProvider<Collection> {
  CollectionProvider() : super('Collections');

  @override
  Collection fromJson(Map<String, dynamic> json) => Collection.fromJson(json);

  Future<CollectionItem> addItem(int collectionId, int destinationId) async =>
      CollectionItem.fromJson(
        (await sendAction('POST', '$endpoint/$collectionId/Items', {'destinationId': destinationId}))!,
      );
}
