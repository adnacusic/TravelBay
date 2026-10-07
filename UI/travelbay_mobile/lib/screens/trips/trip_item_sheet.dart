import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:provider/provider.dart';

import '../../models/destination.dart';
import '../../models/trip_plan.dart';
import '../../providers/destination_provider.dart';
import '../../providers/trip_plan_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/form_errors.dart';
import '../../utils/trip_dates.dart';
import '../../utils/validators.dart';
import 'trip_plan_form_screen.dart';

/// Adds a destination to a plan (or edits an existing item): which plan / destination,
/// on which day, with optional notes. Resolves to a success message when saved.
///
/// - [plan] set, [destination] null: pick a destination for this plan.
/// - [destination] set, [plan] null: pick one of the user's current plans.
/// - [plan] and [item] set: change the day or notes of the item.
class TripItemSheet extends StatefulWidget {
  const TripItemSheet({super.key, this.plan, this.destination, this.item})
      : assert(plan != null || destination != null);

  final TripPlan? plan;
  final Destination? destination;
  final TripPlanItem? item;

  static Future<String?> show(
    BuildContext context, {
    TripPlan? plan,
    Destination? destination,
    TripPlanItem? item,
  }) {
    return showModalBottomSheet<String>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      builder: (context) => Padding(
        padding: EdgeInsets.only(bottom: MediaQuery.viewInsetsOf(context).bottom),
        child: TripItemSheet(plan: plan, destination: destination, item: item),
      ),
    );
  }

  @override
  State<TripItemSheet> createState() => _TripItemSheetState();
}

class _TripItemSheetState extends State<TripItemSheet> {
  static const _maxNotesLength = 500;
  static const _choiceListSize = 100;

  final _formKey = GlobalKey<FormBuilderState>();

  List<TripPlan> _plans = [];
  List<Destination> _destinations = [];
  TripPlan? _selectedPlan;

  bool _isLoading = true;
  bool _isSaving = false;
  String? _loadError;

  bool get _isEdit => widget.item != null;

  @override
  void initState() {
    super.initState();
    _selectedPlan = widget.plan;
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _loadError = null;
    });
    try {
      if (widget.plan == null) {
        final result = await context
            .read<TripPlanProvider>()
            .list(finished: false, pageSize: _choiceListSize);
        _plans = result.items;
      } else if (widget.destination == null && !_isEdit) {
        final result = await context
            .read<DestinationProvider>()
            .get(filter: {'pageSize': _choiceListSize, 'sortBy': 'Name'});
        _destinations = result.items;
      }
    } on Exception catch (e) {
      _loadError = e.toString();
    }
    if (mounted) {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _createPlan() async {
    final message = await openPage<String>(context, const TripPlanFormScreen());
    if (message != null && mounted) {
      await _load();
    }
  }

  Future<void> _save() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final values = form.value;
    final plan = _selectedPlan!;
    final dayNumber = values['dayNumber'] as int;
    final notes = (values['notes'] as String?)?.trim();
    final provider = context.read<TripPlanProvider>();

    setState(() => _isSaving = true);
    try {
      final String message;
      if (_isEdit) {
        await provider.updateItem(plan.id, widget.item!, dayNumber: dayNumber, notes: notes);
        message = 'Izmjene su sačuvane.';
      } else {
        final destinationId = widget.destination?.id ?? values['destinationId'] as int;
        final item = await provider.addItem(
          plan.id,
          destinationId: destinationId,
          dayNumber: dayNumber,
          notes: notes,
        );
        message = '${item.destinationName} je dodana u plan "${plan.name}" (dan $dayNumber).';
      }
      if (mounted) {
        Navigator.pop(context, message);
      }
    } on Exception catch (e) {
      if (mounted) {
        showFormErrors(context, _formKey.currentState, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isSaving = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final title = _isEdit
        ? 'Uredi: ${widget.item!.destinationName}'
        : widget.destination != null
            ? 'Dodaj "${widget.destination!.name}" u plan'
            : 'Dodaj destinaciju u plan';

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.fromLTRB(20, 0, 20, 20),
        child: _isLoading
            ? const SizedBox(height: 160, child: Center(child: CircularProgressIndicator()))
            : _loadError != null
                ? Padding(padding: const EdgeInsets.all(16), child: Text(_loadError!))
                : widget.plan == null && _plans.isEmpty
                    ? _buildNoPlans(theme)
                    : FormBuilder(
                        key: _formKey,
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          crossAxisAlignment: CrossAxisAlignment.stretch,
                          children: [
                            Text(title, style: theme.textTheme.titleMedium),
                            const SizedBox(height: 16),
                            if (widget.plan == null) ...[
                              FormBuilderDropdown<int>(
                                name: 'planId',
                                decoration: const InputDecoration(labelText: 'Plan putovanja *'),
                                items: [
                                  for (final plan in _plans)
                                    DropdownMenuItem(
                                      value: plan.id,
                                      child: Text(plan.name, overflow: TextOverflow.ellipsis),
                                    ),
                                ],
                                validator: Validators.requiredSelection<int>('plan putovanja'),
                                onChanged: (id) {
                                  setState(() => _selectedPlan =
                                      _plans.where((p) => p.id == id).firstOrNull);
                                  _formKey.currentState?.fields['dayNumber']?.didChange(null);
                                },
                              ),
                              const SizedBox(height: 16),
                            ],
                            if (widget.plan != null && widget.destination == null && !_isEdit) ...[
                              FormBuilderDropdown<int>(
                                name: 'destinationId',
                                isExpanded: true,
                                decoration: const InputDecoration(labelText: 'Destinacija *'),
                                items: [
                                  for (final destination in _destinations)
                                    DropdownMenuItem(
                                      value: destination.id,
                                      child: Text(
                                        '${destination.name} · ${destination.cityName}',
                                        overflow: TextOverflow.ellipsis,
                                      ),
                                    ),
                                ],
                                validator: Validators.requiredSelection<int>('destinaciju'),
                              ),
                              const SizedBox(height: 16),
                            ],
                            _buildDayField(),
                            const SizedBox(height: 16),
                            FormBuilderTextField(
                              name: 'notes',
                              initialValue: widget.item?.notes,
                              minLines: 2,
                              maxLines: 4,
                              maxLength: _maxNotesLength,
                              decoration: const InputDecoration(
                                labelText: 'Bilješke',
                                hintText: 'npr. ulaznice, vrijeme polaska, šta ponijeti',
                                alignLabelWithHint: true,
                              ),
                              validator: Validators.optionalText(
                                'Bilješke',
                                maxLength: _maxNotesLength,
                              ),
                            ),
                            const SizedBox(height: 16),
                            Row(
                              children: [
                                Expanded(
                                  child: OutlinedButton(
                                    onPressed: _isSaving ? null : () => Navigator.pop(context),
                                    child: const Text('Odustani'),
                                  ),
                                ),
                                const SizedBox(width: 12),
                                Expanded(
                                  child: FilledButton(
                                    onPressed: _isSaving ? null : _save,
                                    child: _isSaving
                                        ? const SizedBox.square(
                                            dimension: 18,
                                            child: CircularProgressIndicator(strokeWidth: 2),
                                          )
                                        : Text(_isEdit ? 'Sačuvaj' : 'Dodaj u plan'),
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
      ),
    );
  }

  /// Day choices follow the selected plan's period ("Dan 2 · sri 14.10.2026.").
  Widget _buildDayField() {
    final plan = _selectedPlan;
    return FormBuilderDropdown<int>(
      key: ValueKey('day-${plan?.id}'),
      name: 'dayNumber',
      enabled: plan != null,
      initialValue: widget.item?.dayNumber ?? (plan?.dayCount == 1 ? 1 : null),
      decoration: InputDecoration(
        labelText: 'Dan *',
        helperText: plan == null ? 'Prvo odaberite plan.' : null,
      ),
      items: [
        if (plan != null)
          for (var day = 1; day <= plan.dayCount; day++)
            DropdownMenuItem(value: day, child: Text(dayLabel(plan, day))),
      ],
      validator: Validators.requiredSelection<int>('dan'),
    );
  }

  Widget _buildNoPlans(ThemeData theme) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text('Nemate plan u pripremi ni aktivan plan', style: theme.textTheme.titleMedium),
        const SizedBox(height: 8),
        const Text('Destinaciju možete dodati samo u plan koji nije završen ni otkazan.'),
        const SizedBox(height: 16),
        FilledButton.icon(
          onPressed: _createPlan,
          icon: const Icon(Icons.add),
          label: const Text('Napravi novi plan'),
        ),
      ],
    );
  }
}
