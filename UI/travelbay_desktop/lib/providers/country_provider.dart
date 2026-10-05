import '../models/country.dart';
import 'base_provider.dart';

class CountryProvider extends BaseProvider<Country> {
  CountryProvider() : super('Countries');

  @override
  Country fromJson(Map<String, dynamic> json) => Country.fromJson(json);
}
