import 'package:http/http.dart' as http;

import '../models/collection.dart';
import '../models/search_result.dart';
import 'base_provider.dart';

/// The signed-in user's collections of destinations.
class CollectionProvider extends BaseProvider<Collection> {
  CollectionProvider() : super('Collections');

  @override
  Collection fromJson(Map<String, dynamic> json) => Collection.fromJson(json);

  Future<SearchResult<Collection>> list({int page = 1, int pageSize = 50}) => get(filter: {
        'sortBy': 'Name',
        'includeTotalCount': true,
        'page': page,
        'pageSize': pageSize,
      });

  Future<Collection> create(String name) => insert({'name': name});

  Future<Collection> rename(int collectionId, String name) => update(collectionId, {'name': name});

  Future<CollectionItem> addItem(int collectionId, int destinationId) async =>
      CollectionItem.fromJson(
        (await sendAction('POST', '$endpoint/$collectionId/Items', {'destinationId': destinationId}))!,
      );

  Future<void> removeItem(int collectionId, int itemId) async {
    await send(
      (headers) => http.delete(buildUri('$endpoint/$collectionId/Items/$itemId'), headers: headers),
    );
  }
}
