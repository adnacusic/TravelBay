import 'package:json_annotation/json_annotation.dart';

import '../utils/formatters.dart';

/// API DateTimes are UTC but serialized without an offset; parse them as UTC.
/// json_serializable also applies it to nullable DateTime fields.
class UtcDateTimeConverter implements JsonConverter<DateTime, String> {
  const UtcDateTimeConverter();

  @override
  DateTime fromJson(String json) => parseUtc(json)!;

  @override
  String toJson(DateTime object) => object.toUtc().toIso8601String();
}
