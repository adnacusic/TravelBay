import '../models/destination.dart';
import 'base_provider.dart';

class DestinationProvider extends BaseProvider<Destination> {
  DestinationProvider() : super('Destinations');

  @override
  Destination fromJson(Map<String, dynamic> json) => Destination.fromJson(json);
}
