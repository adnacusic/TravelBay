// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ai_agent_run.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AiAgentRun _$AiAgentRunFromJson(Map<String, dynamic> json) => AiAgentRun(
  id: (json['id'] as num).toInt(),
  agentType: $enumDecode(_$AiAgentTypeEnumMap, json['agentType']),
  status: $enumDecode(_$AiAgentRunStatusEnumMap, json['status']),
  requestedByDisplayName: json['requestedByDisplayName'] as String? ?? '',
  requestedAt: const UtcDateTimeConverter().fromJson(
    json['requestedAt'] as String,
  ),
  startedAt: _$JsonConverterFromJson<String, DateTime>(
    json['startedAt'],
    const UtcDateTimeConverter().fromJson,
  ),
  finishedAt: _$JsonConverterFromJson<String, DateTime>(
    json['finishedAt'],
    const UtcDateTimeConverter().fromJson,
  ),
  totalCount: (json['totalCount'] as num?)?.toInt(),
  processedCount: (json['processedCount'] as num?)?.toInt() ?? 0,
  succeededCount: (json['succeededCount'] as num?)?.toInt() ?? 0,
  failedCount: (json['failedCount'] as num?)?.toInt() ?? 0,
  log: json['log'] as String? ?? '',
);

Map<String, dynamic> _$AiAgentRunToJson(AiAgentRun instance) =>
    <String, dynamic>{
      'id': instance.id,
      'agentType': _$AiAgentTypeEnumMap[instance.agentType]!,
      'status': _$AiAgentRunStatusEnumMap[instance.status]!,
      'requestedByDisplayName': instance.requestedByDisplayName,
      'requestedAt': const UtcDateTimeConverter().toJson(instance.requestedAt),
      'startedAt': _$JsonConverterToJson<String, DateTime>(
        instance.startedAt,
        const UtcDateTimeConverter().toJson,
      ),
      'finishedAt': _$JsonConverterToJson<String, DateTime>(
        instance.finishedAt,
        const UtcDateTimeConverter().toJson,
      ),
      'totalCount': instance.totalCount,
      'processedCount': instance.processedCount,
      'succeededCount': instance.succeededCount,
      'failedCount': instance.failedCount,
      'log': instance.log,
    };

const _$AiAgentTypeEnumMap = {AiAgentType.keywords: 0, AiAgentType.images: 1};

const _$AiAgentRunStatusEnumMap = {
  AiAgentRunStatus.queued: 0,
  AiAgentRunStatus.running: 1,
  AiAgentRunStatus.completed: 2,
  AiAgentRunStatus.failed: 3,
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);

AiAgentStatus _$AiAgentStatusFromJson(
  Map<String, dynamic> json,
) => AiAgentStatus(
  destinationsWithoutKeywords: (json['destinationsWithoutKeywords'] as num)
      .toInt(),
  destinationsWithoutImages: (json['destinationsWithoutImages'] as num).toInt(),
  lastKeywordsRun: json['lastKeywordsRun'] == null
      ? null
      : AiAgentRun.fromJson(json['lastKeywordsRun'] as Map<String, dynamic>),
  lastImagesRun: json['lastImagesRun'] == null
      ? null
      : AiAgentRun.fromJson(json['lastImagesRun'] as Map<String, dynamic>),
);

Map<String, dynamic> _$AiAgentStatusToJson(AiAgentStatus instance) =>
    <String, dynamic>{
      'destinationsWithoutKeywords': instance.destinationsWithoutKeywords,
      'destinationsWithoutImages': instance.destinationsWithoutImages,
      'lastKeywordsRun': instance.lastKeywordsRun?.toJson(),
      'lastImagesRun': instance.lastImagesRun?.toJson(),
    };
