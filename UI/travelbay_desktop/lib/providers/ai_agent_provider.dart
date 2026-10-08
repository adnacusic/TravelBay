import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/ai_agent_run.dart';
import '../models/enums.dart';
import '../models/search_result.dart';
import 'api_provider.dart';

/// AI agents: starting one only publishes a RabbitMQ message (API), the worker container does the work.
class AiAgentProvider extends ApiProvider {
  AiAgentProvider() : super('AiAgents');

  Future<AiAgentStatus> getStatus() async => AiAgentStatus.fromJson(await getJson('$endpoint/Status'));

  Future<AiAgentRun> start(AiAgentType agent) async {
    final path = switch (agent) {
      AiAgentType.keywords => '$endpoint/Keywords/Run',
      AiAgentType.images => '$endpoint/Images/Run',
    };
    return AiAgentRun.fromJson((await sendAction('POST', path))!);
  }

  Future<SearchResult<AiAgentRun>> getRuns({required int page, required int pageSize}) async {
    final response = await send(
      (headers) => http.get(
        buildUri('$endpoint/Runs', {'page': page, 'pageSize': pageSize, 'includeTotalCount': true}),
        headers: headers,
      ),
    );
    final data = jsonDecode(response.body) as Map<String, dynamic>;
    return SearchResult(
      items: (data['items'] as List).map((e) => AiAgentRun.fromJson(e as Map<String, dynamic>)).toList(),
      totalCount: data['totalCount'] as int?,
    );
  }
}
