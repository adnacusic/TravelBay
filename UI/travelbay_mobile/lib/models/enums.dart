import 'package:json_annotation/json_annotation.dart';

/// Mirrors TravelBay.Model.Enums.ReviewStatus (serialized as int).
enum ReviewStatus {
  @JsonValue(0)
  pending('Na čekanju'),
  @JsonValue(1)
  approved('Odobrena'),
  @JsonValue(2)
  rejected('Odbijena');

  const ReviewStatus(this.label);
  final String label;
}

/// Mirrors TravelBay.Model.Enums.NotificationType (serialized as int).
enum NotificationType {
  @JsonValue(0)
  reviewApproved('Recenzija odobrena'),
  @JsonValue(1)
  reviewRejected('Recenzija odbijena'),
  @JsonValue(2)
  tripStatusChanged('Status plana putovanja'),
  @JsonValue(3)
  news('Novost'),
  @JsonValue(4)
  general('Obavještenje');

  const NotificationType(this.label);
  final String label;
}

/// Mirrors TravelBay.Model.Enums.TripPlanStatus (serialized as int).
enum TripPlanStatus {
  @JsonValue(0)
  draft('U pripremi'),
  @JsonValue(1)
  active('Aktivan'),
  @JsonValue(2)
  completed('Završen'),
  @JsonValue(3)
  cancelled('Otkazan');

  const TripPlanStatus(this.label);
  final String label;
}
