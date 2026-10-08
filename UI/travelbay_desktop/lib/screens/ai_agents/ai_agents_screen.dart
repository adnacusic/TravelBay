import 'dart:async';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/ai_agent_run.dart';
import '../../models/enums.dart';
import '../../providers/ai_agent_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/pagination_bar.dart';
import '../../widgets/scrollable_table.dart';
import '../../widgets/tone_chip.dart';

/// Starts the two AI agents and follows their runs. "Pokreni" only sends a RabbitMQ message
/// through the API; the separate worker container processes it and writes progress into the
/// run, which this screen polls (every 2 s while a run is active), so nothing needs a manual refresh.
class AiAgentsScreen extends StatefulWidget {
  const AiAgentsScreen({super.key});

  @override
  State<AiAgentsScreen> createState() => _AiAgentsScreenState();
}

class _AiAgentsScreenState extends State<AiAgentsScreen> {
  static const _activePollInterval = Duration(seconds: 2);
  static const _idlePollInterval = Duration(seconds: 15);
  static const _historyPageSize = 10;

  AiAgentStatus? _status;
  List<AiAgentRun> _history = [];
  int _historyPage = 1;
  int _historyTotal = 0;
  String? _error;
  final Set<AiAgentType> _starting = {};
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    _refresh();
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  Future<void> _refresh() async {
    _timer?.cancel();
    final provider = context.read<AiAgentProvider>();
    try {
      final status = await provider.getStatus();
      final history = await provider.getRuns(page: _historyPage, pageSize: _historyPageSize);
      if (mounted) {
        setState(() {
          _status = status;
          _history = history.items;
          _historyTotal = history.totalCount ?? history.items.length;
          _error = null;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
    if (mounted) {
      final active = _status?.hasActiveRun ?? false;
      _timer = Timer(active ? _activePollInterval : _idlePollInterval, _refresh);
    }
  }

  Future<void> _start(AiAgentType agent, int waiting) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Pokretanje ${agent.label}',
      message: '${agent.description}\n\nAgent će obraditi $waiting destinacija. Upisani podaci '
          'se mogu kasnije ručno izmijeniti na formi destinacije.',
      confirmLabel: 'Pokreni',
      destructive: false,
    );
    if (!confirmed || !mounted) {
      return;
    }

    setState(() => _starting.add(agent));
    try {
      await context.read<AiAgentProvider>().start(agent);
      if (mounted) {
        showSuccessMessage(context, '${agent.label} je pokrenut — worker preuzima posao iz reda poruka.');
        _historyPage = 1;
        await _refresh();
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _starting.remove(agent));
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      section: AdminSection.aiAgents,
      title: 'AI Agenti',
      child: _buildBody(),
    );
  }

  Widget _buildBody() {
    final status = _status;
    if (status == null) {
      return Center(child: _error == null ? const CircularProgressIndicator() : Text(_error!));
    }

    final theme = Theme.of(context);
    return ListView(
      padding: const EdgeInsets.all(24),
      children: [
        Text(
          'Agenti rade u odvojenom worker servisu (Python kontejner). Pokretanje šalje poruku '
          'preko RabbitMQ-a; napredak i log se ovdje osvježavaju sami.',
          style: theme.textTheme.bodyMedium,
        ),
        if (_error != null) ...[
          const SizedBox(height: 8),
          Text(_error!, style: TextStyle(color: theme.colorScheme.error)),
        ],
        const SizedBox(height: 16),
        Wrap(
          spacing: 24,
          runSpacing: 24,
          children: [
            for (final agent in AiAgentType.values)
              SizedBox(
                width: 520,
                child: _AgentCard(
                  agent: agent,
                  waiting: status.waitingFor(agent),
                  lastRun: status.lastRunOf(agent),
                  isStarting: _starting.contains(agent),
                  onStart: () => _start(agent, status.waitingFor(agent)),
                ),
              ),
          ],
        ),
        const SizedBox(height: 32),
        Text('Historija pokretanja', style: theme.textTheme.titleMedium),
        const SizedBox(height: 8),
        _buildHistory(),
      ],
    );
  }

  Widget _buildHistory() {
    if (_history.isEmpty) {
      return const Text('Agenti još nisu pokretani.');
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        ScrollableTable(
          child: DataTable(
            columns: const [
              DataColumn(label: Text('Agent')),
              DataColumn(label: Text('Status')),
              DataColumn(label: Text('Pokrenuo')),
              DataColumn(label: Text('Pokrenut')),
              DataColumn(label: Text('Završen')),
              DataColumn(label: Text('Uspješno'), numeric: true),
              DataColumn(label: Text('Neuspješno'), numeric: true),
            ],
            rows: [
              for (final run in _history)
                DataRow(cells: [
                  DataCell(Text(run.agentType.label)),
                  DataCell(_RunStatusChip(status: run.status)),
                  DataCell(Text(run.requestedByDisplayName)),
                  DataCell(Text(formatDateTime(run.requestedAt))),
                  DataCell(Text(formatDateTime(run.finishedAt))),
                  DataCell(Text('${run.succeededCount}')),
                  DataCell(Text('${run.failedCount}')),
                ]),
            ],
          ),
        ),
        PaginationBar(
          page: _historyPage,
          pageSize: _historyPageSize,
          totalCount: _historyTotal,
          onPageChanged: (page) {
            _historyPage = page;
            _refresh();
          },
        ),
      ],
    );
  }
}

class _AgentCard extends StatelessWidget {
  const _AgentCard({
    required this.agent,
    required this.waiting,
    required this.lastRun,
    required this.isStarting,
    required this.onStart,
  });

  static const _logHeight = 220.0;

  final AiAgentType agent;
  final int waiting;
  final AiAgentRun? lastRun;
  final bool isStarting;
  final VoidCallback onStart;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final run = lastRun;
    final isActive = run?.status.isActive ?? false;
    final canStart = !isActive && !isStarting && waiting > 0;

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Icon(
                  agent == AiAgentType.keywords ? Icons.sell_outlined : Icons.image_search,
                  color: theme.colorScheme.primary,
                ),
                const SizedBox(width: 8),
                Expanded(child: Text(agent.label, style: theme.textTheme.titleMedium)),
                if (run != null) _RunStatusChip(status: run.status),
              ],
            ),
            const SizedBox(height: 8),
            Text(agent.description, style: theme.textTheme.bodyMedium),
            const SizedBox(height: 8),
            Text(
              waiting == 0
                  ? 'Sve destinacije su obrađene — nema šta pokrenuti.'
                  : 'Čeka obradu: $waiting destinacija.',
              style: theme.textTheme.bodySmall?.copyWith(fontWeight: FontWeight.w600),
            ),
            const SizedBox(height: 16),
            Align(
              alignment: Alignment.centerLeft,
              child: FilledButton.icon(
                onPressed: canStart ? onStart : null,
                icon: isStarting || isActive
                    ? const SizedBox.square(
                        dimension: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.play_arrow),
                label: Text(isActive ? '${agent.label} radi…' : 'Pokreni ${agent.label}'),
              ),
            ),
            if (run != null) ...[
              const SizedBox(height: 16),
              _buildProgress(context, run),
              const SizedBox(height: 12),
              Text('Log zadnjeg pokretanja', style: theme.textTheme.labelLarge),
              const SizedBox(height: 4),
              Container(
                height: _logHeight,
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: theme.colorScheme.surfaceContainerHighest,
                  borderRadius: BorderRadius.circular(6),
                ),
                child: SingleChildScrollView(
                  reverse: true,
                  child: SelectableText(
                    run.log,
                    style: const TextStyle(fontFamily: 'Consolas', fontSize: 12),
                  ),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildProgress(BuildContext context, AiAgentRun run) {
    final theme = Theme.of(context);
    final total = run.totalCount;
    final summary = total == null
        ? 'Pokrenuo ${run.requestedByDisplayName}, ${formatDateTime(run.requestedAt)} — čeka worker.'
        : 'Obrađeno ${run.processedCount} / $total · uspješno ${run.succeededCount} · '
            'neuspješno ${run.failedCount}';
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        LinearProgressIndicator(
          value: run.status.isActive ? run.progress : 1,
          minHeight: 6,
          borderRadius: BorderRadius.circular(3),
        ),
        const SizedBox(height: 6),
        Text(summary, style: theme.textTheme.bodySmall),
      ],
    );
  }
}

class _RunStatusChip extends StatelessWidget {
  const _RunStatusChip({required this.status});

  final AiAgentRunStatus status;

  @override
  Widget build(BuildContext context) {
    final tone = switch (status) {
      AiAgentRunStatus.queued => ChipTone.neutral,
      AiAgentRunStatus.running => ChipTone.warning,
      AiAgentRunStatus.completed => ChipTone.positive,
      AiAgentRunStatus.failed => ChipTone.negative,
    };
    return ToneChip(label: status.label, tone: tone);
  }
}
