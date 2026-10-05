import '../models/city.dart';
import 'base_provider.dart';

class CityProvider extends BaseProvider<City> {
  CityProvider() : super('Cities');

  @override
  City fromJson(Map<String, dynamic> json) => City.fromJson(json);
}
