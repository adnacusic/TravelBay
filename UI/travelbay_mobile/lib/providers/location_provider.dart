import '../models/city.dart';
import '../models/country.dart';
import 'base_provider.dart';

class CountryProvider extends BaseProvider<Country> {
  CountryProvider() : super('Countries');

  @override
  Country fromJson(Map<String, dynamic> json) => Country.fromJson(json);
}

class CityProvider extends BaseProvider<City> {
  CityProvider() : super('Cities');

  @override
  City fromJson(Map<String, dynamic> json) => City.fromJson(json);
}
