import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/enums.dart';
import '../../models/trip_plan.dart';
import '../../providers/trip_plan_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../utils/trip_dates.dart';
import '../../widgets/trip_status_chip.dart';
import '../destination/destination_details_screen.dart';
import 'trip_item_sheet.dart';
import 'trip_plan_form_screen.dart';

/// Itinerary of one plan, day by day. Status changes go through the API state
/// machine; completed and cancelled plans are read-only.
/// Pops with true when the plan changed, so the list reloads.
class TripPlanDetailsScreen extends StatefulWidget {
  const TripPlanDetailsScreen({super.key, required this.planId});

  final int planId;

  @override
  State<TripPlanDetailsScreen> createState() => _TripPlanDetailsScreenState();
}

/// A status change offered for the plan's current status.
class _Transition {
  const _Transition(this.label, this.icon, this.confirmTitle, this.confirmMessage, this.run,
      {this.destructive = false});

  final String label;
  final IconData icon;
  final String confirmTitle;
  final String confirmMessage;
  final Future<TripPlan> Function(TripPlanProvider provider, int planId) run;
  final bool destructive;
}

class _TripPlanDetailsScreenState extends State<TripPlanDetailsScreen> {
  TripPlan? _plan;
  String? _error;
  bool _isBusy = false;
  bool _changed = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final plan = await context.read<TripPlanProvider>().getById(widget.planId);
      if (mounted) {
        setState(() => _plan = plan);
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  bool _isEditable(TripPlan plan) =>
      plan.status == TripPlanStatus.draft || plan.status == TripPlanStatus.active;

  /// Mirrors the API state machine: Draft -> Active -> Completed, Draft/Active -> Cancelled.
  List<_Transition> _transitions(TripPlan plan) {
    const cancel = _Transition(
      'Otkaži plan',
      Icons.cancel_outlined,
      'Otkazivanje plana',
      'Otkazan plan ostaje u historiji, ali se više ne može mijenjati ni ponovo aktivirati.',
      _cancel,
      destructive: true,
    );
    return switch (plan.status) {
      TripPlanStatus.draft => const [
          _Transition(
            'Aktiviraj plan',
            Icons.play_arrow,
            'Aktivacija plana',
            'Plan postaje aktivan. Destinacije i dalje možete dodavati i mijenjati.',
            _activate,
          ),
          cancel,
        ],
      TripPlanStatus.active => const [
          _Transition(
            'Označi kao završen',
            Icons.flag_outlined,
            'Završetak putovanja',
            'Završen plan prelazi u prethodna putovanja i više se ne može mijenjati.',
            _complete,
          ),
          cancel,
        ],
      TripPlanStatus.completed || TripPlanStatus.cancelled => const [],
    };
  }

  static Future<TripPlan> _activate(TripPlanProvider p, int id) => p.activate(id);
  static Future<TripPlan> _complete(TripPlanProvider p, int id) => p.complete(id);
  static Future<TripPlan> _cancel(TripPlanProvider p, int id) => p.cancel(id);

  Future<void> _changeStatus(TripPlan plan, _Transition transition) async {
    final confirmed = await showConfirmDialog(
      context,
      title: transition.confirmTitle,
      message: transition.confirmMessage,
      confirmLabel: transition.label,
      destructive: transition.destructive,
    );
    if (!confirmed || !mounted) {
      return;
    }

    setState(() => _isBusy = true);
    try {
      final updated = await transition.run(context.read<TripPlanProvider>(), plan.id);
      if (mounted) {
        setState(() {
          _plan = updated;
          _changed = true;
        });
        showSuccessMessage(context, 'Plan "${plan.name}" je sada: ${updated.status.label}.');
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
        _load();
      }
    } finally {
      if (mounted) {
        setState(() => _isBusy = false);
      }
    }
  }

  Future<void> _edit(TripPlan plan) async {
    final message = await openPage<String>(context, TripPlanFormScreen(plan: plan));
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _changed = true;
      _load();
    }
  }

  Future<void> _delete(TripPlan plan) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje plana',
      message: 'Da li ste sigurni da želite obrisati plan "${plan.name}" '
          'sa svim njegovim destinacijama?',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<TripPlanProvider>().remove(plan.id);
      if (mounted) {
        showSuccessMessage(context, 'Plan "${plan.name}" je obrisan.');
        Navigator.pop(context, true);
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _addItem(TripPlan plan) async {
    final message = await TripItemSheet.show(context, plan: plan);
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _changed = true;
      _load();
    }
  }

  Future<void> _editItem(TripPlan plan, TripPlanItem item) async {
    final message = await TripItemSheet.show(context, plan: plan, item: item);
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _load();
    }
  }

  Future<void> _removeItem(TripPlan plan, TripPlanItem item) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Uklanjanje iz plana',
      message: 'Ukloniti "${item.destinationName}" iz plana "${plan.name}"?',
      confirmLabel: 'Ukloni',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<TripPlanProvider>().removeItem(plan.id, item.id);
      if (mounted) {
        showSuccessMessage(context, '${item.destinationName} je uklonjena iz plana.');
        _changed = true;
        _load();
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final plan = _plan;

    return PopScope<bool>(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          Navigator.pop(context, _changed);
        }
      },
      child: Scaffold(
        appBar: AppBar(
          title: Text(plan?.name ?? 'Plan putovanja'),
          actions: [
            if (plan != null)
              PopupMenuButton<String>(
                onSelected: (action) => action == 'edit' ? _edit(plan) : _delete(plan),
                itemBuilder: (context) => [
                  PopupMenuItem(
                    value: 'edit',
                    enabled: _isEditable(plan),
                    child: const ListTile(
                      leading: Icon(Icons.edit_outlined),
                      title: Text('Uredi naziv i datume'),
                    ),
                  ),
                  const PopupMenuItem(
                    value: 'delete',
                    child: ListTile(
                      leading: Icon(Icons.delete_outline),
                      title: Text('Obriši plan'),
                    ),
                  ),
                ],
              ),
          ],
        ),
        floatingActionButton: plan != null && _isEditable(plan)
            ? FloatingActionButton.extended(
                onPressed: () => _addItem(plan),
                icon: const Icon(Icons.add_location_alt_outlined),
                label: const Text('Dodaj destinaciju'),
              )
            : null,
        body: SafeArea(child: _buildBody()),
      ),
    );
  }

  Widget _buildBody() {
    final plan = _plan;
    if (_error != null) {
      return Center(child: Padding(padding: const EdgeInsets.all(24), child: Text(_error!)));
    }
    if (plan == null) {
      return const Center(child: CircularProgressIndicator());
    }

    final theme = Theme.of(context);
    final transitions = _transitions(plan);

    return ListView(
      padding: const EdgeInsets.fromLTRB(16, 8, 16, 96),
      children: [
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Expanded(child: Text(planPeriod(plan), style: theme.textTheme.bodyMedium)),
                    TripStatusChip(status: plan.status),
                  ],
                ),
                const SizedBox(height: 8),
                Text('${plan.items.length} destinacija u planu', style: theme.textTheme.bodySmall),
                if (transitions.isEmpty) ...[
                  const SizedBox(height: 12),
                  Text(
                    'Plan je ${plan.status.label.toLowerCase()} i može se samo pregledati.',
                    style: theme.textTheme.bodyMedium?.copyWith(fontStyle: FontStyle.italic),
                  ),
                ] else ...[
                  const SizedBox(height: 12),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      for (final transition in transitions)
                        transition.destructive
                            ? OutlinedButton.icon(
                                onPressed: _isBusy ? null : () => _changeStatus(plan, transition),
                                icon: Icon(transition.icon),
                                label: Text(transition.label),
                              )
                            : FilledButton.icon(
                                onPressed: _isBusy ? null : () => _changeStatus(plan, transition),
                                icon: Icon(transition.icon),
                                label: Text(transition.label),
                              ),
                    ],
                  ),
                ],
              ],
            ),
          ),
        ),
        for (var day = 1; day <= plan.dayCount; day++) _buildDay(plan, day),
      ],
    );
  }

  Widget _buildDay(TripPlan plan, int day) {
    final theme = Theme.of(context);
    final items = plan.items.where((i) => i.dayNumber == day).toList()
      ..sort((a, b) => a.orderIndex.compareTo(b.orderIndex));
    final editable = _isEditable(plan);

    return Padding(
      padding: const EdgeInsets.only(top: 16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(dayLabel(plan, day), style: theme.textTheme.titleSmall),
          const SizedBox(height: 6),
          if (items.isEmpty)
            Text('Nema planiranih destinacija.', style: theme.textTheme.bodySmall)
          else
            for (final item in items)
              Card(
                margin: const EdgeInsets.only(bottom: 6),
                child: ListTile(
                  leading: const Icon(Icons.place_outlined),
                  title: Text(item.destinationName),
                  subtitle: item.notes == null ? null : Text(item.notes!),
                  onTap: () => openPage<void>(
                    context,
                    DestinationDetailsScreen(destinationId: item.destinationId),
                  ),
                  trailing: editable
                      ? PopupMenuButton<String>(
                          tooltip: 'Opcije',
                          onSelected: (action) => action == 'edit'
                              ? _editItem(plan, item)
                              : _removeItem(plan, item),
                          itemBuilder: (context) => const [
                            PopupMenuItem(value: 'edit', child: Text('Promijeni dan ili bilješke')),
                            PopupMenuItem(value: 'remove', child: Text('Ukloni iz plana')),
                          ],
                        )
                      : null,
                ),
              ),
        ],
      ),
    );
  }
}
