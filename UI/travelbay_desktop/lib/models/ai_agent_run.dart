import 'package:json_annotation/json_annotation.dart';

import 'enums.dart';
import 'utc_date_time_converter.dart';

part 'ai_agent_run.g.dart';

/// One start of an AI agent; the worker container fills status, counters and log.
@JsonSerializable()
@UtcDateTimeConverter()
class AiAgentRun {
  AiAgentRun({
    required this.id,
    required this.agentType,
    required this.status,
    this.requestedByDisplayName = '',
    required this.requestedAt,
    this.startedAt,
    this.finishedAt,
    this.totalCount,
    this.processedCount = 0,
    this.succeededCount = 0,
    this.failedCount = 0,
    this.log = '',
  });

  final int id;
  final AiAgentType agentType;
  final AiAgentRunStatus status;
  final String requestedByDisplayName;
  final DateTime requestedAt;
  final DateTime? startedAt;
  final DateTime? finishedAt;

  /// Null until the worker has counted the destinations to process.
  final int? totalCount;
  final int processedCount;
  final int succeededCount;
  final int failedCount;
  final String log;

  /// 0–1, or null while the total is not known yet.
  double? get progress {
    final total = totalCount;
    if (total == null) {
      return null;
    }
    return total == 0 ? 1 : processedCount / total;
  }

  factory AiAgentRun.fromJson(Map<String, dynamic> json) => _$AiAgentRunFromJson(json);

  Map<String, dynamic> toJson() => _$AiAgentRunToJson(this);
}

/// What each agent still has to do, with its latest run.
@JsonSerializable(explicitToJson: true)
class AiAgentStatus {
  AiAgentStatus({
    required this.destinationsWithoutKeywords,
    required this.destinationsWithoutImages,
    this.lastKeywordsRun,
    this.lastImagesRun,
  });

  final int destinationsWithoutKeywords;
  final int destinationsWithoutImages;
  final AiAgentRun? lastKeywordsRun;
  final AiAgentRun? lastImagesRun;

  int waitingFor(AiAgentType agent) => switch (agent) {
        AiAgentType.keywords => destinationsWithoutKeywords,
        AiAgentType.images => destinationsWithoutImages,
      };

  AiAgentRun? lastRunOf(AiAgentType agent) => switch (agent) {
        AiAgentType.keywords => lastKeywordsRun,
        AiAgentType.images => lastImagesRun,
      };

  bool get hasActiveRun =>
      (lastKeywordsRun?.status.isActive ?? false) || (lastImagesRun?.status.isActive ?? false);

  factory AiAgentStatus.fromJson(Map<String, dynamic> json) => _$AiAgentStatusFromJson(json);

  Map<String, dynamic> toJson() => _$AiAgentStatusToJson(this);
}
