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

/// Mirrors TravelBay.Model.Enums.AiAgentType (serialized as int).
enum AiAgentType {
  @JsonValue(0)
  keywords('AIAgentKeywords', 'Groq LLM generiše 3–5 ključnih riječi za destinacije koje ih nemaju.'),
  @JsonValue(1)
  images('AIAgentSlike', 'Pronalazi slobodnu sliku na Wikimedia Commons za destinacije koje nemaju nijednu.');

  const AiAgentType(this.label, this.description);
  final String label;
  final String description;
}

/// Mirrors TravelBay.Model.Enums.AiAgentRunStatus (serialized as int).
enum AiAgentRunStatus {
  @JsonValue(0)
  queued('Na čekanju'),
  @JsonValue(1)
  running('U toku'),
  @JsonValue(2)
  completed('Završen'),
  @JsonValue(3)
  failed('Neuspješan');

  const AiAgentRunStatus(this.label);
  final String label;

  bool get isActive => this == queued || this == running;
}
