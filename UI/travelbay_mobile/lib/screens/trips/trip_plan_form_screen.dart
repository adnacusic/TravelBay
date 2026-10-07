import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/trip_plan.dart';
import '../../providers/trip_plan_provider.dart';
import '../../utils/form_errors.dart';
import '../../utils/trip_dates.dart';
import '../../utils/validators.dart';

/// Create (no [plan]) or edit a trip plan: name and period.
/// Pops with a success message when saved.
class TripPlanFormScreen extends StatefulWidget {
  const TripPlanFormScreen({super.key, this.plan});

  final TripPlan? plan;

  @override
  State<TripPlanFormScreen> createState() => _TripPlanFormScreenState();
}

class _TripPlanFormScreenState extends State<TripPlanFormScreen> {
  static const _maxNameLength = 200;
  static final _dateFormat = DateFormat('dd.MM.yyyy.');

  final _formKey = GlobalKey<FormBuilderState>();
  bool _isSaving = false;

  bool get _isEdit => widget.plan != null;

  Future<void> _save() async {
    final form = _formKey.currentState!;
    if (!form.saveAndValidate()) {
      return;
    }

    final values = form.value;
    final request = {
      'name': (values['name'] as String).trim(),
      'startDate': toApiDate(values['startDate'] as DateTime),
      'endDate': toApiDate(values['endDate'] as DateTime),
    };

    setState(() => _isSaving = true);
    try {
      final provider = context.read<TripPlanProvider>();
      final saved = _isEdit
          ? await provider.update(widget.plan!.id, request)
          : await provider.insert(request);
      if (mounted) {
        Navigator.pop(
          context,
          _isEdit
              ? 'Izmjene plana "${saved.name}" su sačuvane.'
              : 'Plan "${saved.name}" je napravljen. Sada mu dodajte destinacije.',
        );
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
    final plan = widget.plan;
    final today = DateTime.now();

    return MasterScreen(
      title: _isEdit ? 'Uređivanje plana' : 'Novi plan putovanja',
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: FormBuilder(
          key: _formKey,
          initialValue: {
            'name': plan?.name,
            'startDate': plan == null ? null : calendarDay(plan.startDate),
            'endDate': plan == null ? null : calendarDay(plan.endDate),
          },
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              FormBuilderTextField(
                name: 'name',
                maxLength: _maxNameLength,
                decoration: const InputDecoration(
                  labelText: 'Naziv plana *',
                  hintText: 'npr. Vikend u Mostaru',
                ),
                validator: Validators.requiredText('Naziv plana', maxLength: _maxNameLength),
              ),
              const SizedBox(height: 8),
              FormBuilderDateTimePicker(
                name: 'startDate',
                inputType: InputType.date,
                format: _dateFormat,
                firstDate: DateTime(today.year - 5),
                lastDate: DateTime(today.year + 5),
                decoration: const InputDecoration(
                  labelText: 'Datum polaska *',
                  suffixIcon: Icon(Icons.calendar_month_outlined),
                ),
                validator: (value) => value == null ? 'Odaberite datum polaska.' : null,
              ),
              const SizedBox(height: 20),
              FormBuilderDateTimePicker(
                name: 'endDate',
                inputType: InputType.date,
                format: _dateFormat,
                firstDate: DateTime(today.year - 5),
                lastDate: DateTime(today.year + 5),
                decoration: const InputDecoration(
                  labelText: 'Datum povratka *',
                  suffixIcon: Icon(Icons.calendar_month_outlined),
                ),
                validator: (value) {
                  if (value == null) {
                    return 'Odaberite datum povratka.';
                  }
                  final start = _formKey.currentState?.fields['startDate']?.value as DateTime?;
                  if (start != null && value.isBefore(start)) {
                    return 'Datum povratka ne može biti prije datuma polaska.';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 28),
              FilledButton(
                onPressed: _isSaving ? null : _save,
                style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(50)),
                child: _isSaving
                    ? const SizedBox.square(
                        dimension: 20,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : Text(_isEdit ? 'Sačuvaj izmjene' : 'Napravi plan'),
              ),
              const SizedBox(height: 8),
              OutlinedButton(
                onPressed: _isSaving ? null : () => Navigator.pop(context),
                style: OutlinedButton.styleFrom(minimumSize: const Size.fromHeight(50)),
                child: const Text('Odustani'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
