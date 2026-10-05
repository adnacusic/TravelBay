import 'package:json_annotation/json_annotation.dart';

part 'city.g.dart';

@JsonSerializable()
class City {
  City({
    required this.id,
    required this.name,
    required this.countryId,
    this.countryName = '',
  });

  final int id;
  final String name;
  final int countryId;
  final String countryName;

  factory City.fromJson(Map<String, dynamic> json) => _$CityFromJson(json);

  Map<String, dynamic> toJson() => _$CityToJson(this);
}
