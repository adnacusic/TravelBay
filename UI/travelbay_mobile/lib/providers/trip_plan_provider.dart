import '../models/trip_plan.dart';
import 'base_provider.dart';

/// The signed-in user's trip plans. Status changes go only through the API
/// state machine (Activate / Complete / Cancel), never through a plain update.
class TripPlanProvider extends BaseProvider<TripPlan> {
  TripPlanProvider() : super('TripPlans');

  @override
  TripPlan fromJson(Map<String, dynamic> json) => TripPlan.fromJson(json);

  Future<TripPlan> activate(int id) async =>
      fromJson((await sendAction('POST', '$endpoint/$id/Activate'))!);

  Future<TripPlan> complete(int id) async =>
      fromJson((await sendAction('POST', '$endpoint/$id/Complete'))!);

  Future<TripPlan> cancel(int id) async =>
      fromJson((await sendAction('POST', '$endpoint/$id/Cancel'))!);

  Future<TripPlanItem> addItem(int planId, Map<String, dynamic> request) async =>
      TripPlanItem.fromJson((await sendAction('POST', '$endpoint/$planId/Items', request))!);

  Future<TripPlanItem> updateItem(int planId, int itemId, Map<String, dynamic> request) async =>
      TripPlanItem.fromJson(
        (await sendAction('PUT', '$endpoint/$planId/Items/$itemId', request))!,
      );
}
