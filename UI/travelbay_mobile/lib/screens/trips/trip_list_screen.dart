import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/trip_plan.dart';
import '../../providers/trip_plan_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../utils/trip_dates.dart';
import '../../widgets/trip_status_chip.dart';
import 'trip_plan_details_screen.dart';
import 'trip_plan_form_screen.dart';

/// "Putovanja" tab: current plans (draft, active) and previous ones (completed, cancelled).
class TripListScreen extends StatefulWidget {
  const TripListScreen({super.key, required this.activation});

  /// Changes every time the tab is opened again (a destination may have been added from its details).
  final int activation;

  @override
  State<TripListScreen> createState() => _TripListScreenState();
}

class _TripListScreenState extends State<TripListScreen> {
  /// Bumped after any change so both tabs load fresh data.
  int _version = 0;

  void _reload() => setState(() => _version++);

  @override
  void didUpdateWidget(TripListScreen oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.activation != widget.activation) {
      _version++;
    }
  }

  Future<void> _create() async {
    final message = await openPage<String>(context, const TripPlanFormScreen());
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _reload();
    }
  }

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Putovanja'),
          bottom: const TabBar(
            tabs: [
              Tab(text: 'Aktuelni'),
              Tab(text: 'Prethodni'),
            ],
          ),
        ),
        floatingActionButton: FloatingActionButton.extended(
          onPressed: _create,
          icon: const Icon(Icons.add),
          label: const Text('Novi plan'),
        ),
        body: TabBarView(
          children: [
            _PlanList(key: ValueKey('current-$_version'), finished: false, onChanged: _reload),
            _PlanList(key: ValueKey('finished-$_version'), finished: true, onChanged: _reload),
          ],
        ),
      ),
    );
  }
}

class _PlanList extends StatefulWidget {
  const _PlanList({super.key, required this.finished, required this.onChanged});

  final bool finished;
  final VoidCallback onChanged;

  @override
  State<_PlanList> createState() => _PlanListState();
}

class _PlanListState extends State<_PlanList> with AutomaticKeepAliveClientMixin {
  static const _pageSize = 20;

  final List<TripPlan> _plans = [];
  int _totalCount = 0;
  int _page = 0;
  bool _isLoading = false;
  String? _error;

  @override
  bool get wantKeepAlive => true;

  @override
  void initState() {
    super.initState();
    _loadNextPage();
  }

  Future<void> _loadNextPage() async {
    if (_isLoading) {
      return;
    }
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final result = await context
          .read<TripPlanProvider>()
          .list(finished: widget.finished, page: _page + 1, pageSize: _pageSize);
      if (mounted) {
        setState(() {
          _plans.addAll(result.items);
          _totalCount = result.totalCount ?? _plans.length;
          _page++;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  Future<void> _open(TripPlan plan) async {
    final changed = await openPage<bool>(context, TripPlanDetailsScreen(planId: plan.id));
    if (changed == true) {
      widget.onChanged();
    }
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);
    if (_error != null && _plans.isEmpty) {
      return Center(child: Padding(padding: const EdgeInsets.all(24), child: Text(_error!)));
    }
    if (_isLoading && _plans.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_plans.isEmpty) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(32),
          child: Text(
            widget.finished
                ? 'Još nemate završenih ni otkazanih putovanja.'
                : 'Nemate planiranih putovanja. Napravite prvi plan dugmetom "Novi plan".',
            textAlign: TextAlign.center,
          ),
        ),
      );
    }

    final hasMore = _plans.length < _totalCount;
    return ListView.builder(
      padding: const EdgeInsets.fromLTRB(12, 8, 12, 96),
      itemCount: _plans.length + (hasMore ? 1 : 0),
      itemBuilder: (context, index) {
        if (index == _plans.length) {
          return Center(
            child: _isLoading
                ? const CircularProgressIndicator()
                : OutlinedButton(onPressed: _loadNextPage, child: const Text('Učitaj još')),
          );
        }
        final plan = _plans[index];
        return Card(
          child: ListTile(
            contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
            title: Text(plan.name),
            subtitle: Text('${planPeriod(plan)}\n${destinationCountLabel(plan.items.length)}'),
            isThreeLine: true,
            trailing: TripStatusChip(status: plan.status),
            onTap: () => _open(plan),
          ),
        );
      },
    );
  }
}
