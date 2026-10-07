import '../models/trip_plan.dart';
import 'formatters.dart';

const _weekdays = ['pon', 'uto', 'sri', 'čet', 'pet', 'sub', 'ned'];

/// Trip dates are calendar days: they are sent as `yyyy-MM-dd` and read by their
/// UTC date parts, so no time zone can move a day.
String toApiDate(DateTime value) =>
    '${value.year}-${value.month.toString().padLeft(2, '0')}-${value.day.toString().padLeft(2, '0')}';

DateTime calendarDay(DateTime value) {
  final utc = value.toUtc();
  return DateTime(utc.year, utc.month, utc.day);
}

/// Date of a day number (1-based) in the plan.
DateTime planDay(TripPlan plan, int dayNumber) =>
    calendarDay(plan.startDate).add(Duration(days: dayNumber - 1));

/// e.g. "Dan 2 · sri 14.10.2026."
String dayLabel(TripPlan plan, int dayNumber) {
  final date = planDay(plan, dayNumber);
  return 'Dan $dayNumber · ${_weekdays[date.weekday - 1]} ${formatDate(date)}';
}

/// e.g. "12.10.2026. – 15.10.2026. (4 dana)"
String planPeriod(TripPlan plan) {
  final days = plan.dayCount;
  final unit = days == 1 ? 'dan' : 'dana';
  return '${formatDate(calendarDay(plan.startDate))} – ${formatDate(calendarDay(plan.endDate))} ($days $unit)';
}
