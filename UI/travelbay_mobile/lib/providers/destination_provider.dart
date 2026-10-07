import '../models/destination.dart';
import 'base_provider.dart';

class DestinationProvider extends BaseProvider<Destination> {
  DestinationProvider() : super('Destinations');

  @override
  Destination fromJson(Map<String, dynamic> json) => Destination.fromJson(json);

  /// Most viewed destinations, then by approved reviews and average rating.
  Future<List<Destination>> popular({int top = 10}) async {
    final data = await getJson('$endpoint/Popular', {'top': top});
    return (data['items'] as List)
        .map((e) => fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
