import 'package:http/http.dart' as http;

import '../models/search_result.dart';
import '../models/trip_plan.dart';
import 'base_provider.dart';

/// The signed-in user's trip plans. Status changes go only through the API
/// state machine (Activate / Complete / Cancel), never through a plain update.
class TripPlanProvider extends BaseProvider<TripPlan> {
  TripPlanProvider() : super('TripPlans');

  @override
  TripPlan fromJson(Map<String, dynamic> json) => TripPlan.fromJson(json);

  /// [finished]: false = draft and active plans, true = completed and cancelled.
  Future<SearchResult<TripPlan>> list({required bool finished, int page = 1, int pageSize = 20}) =>
      get(filter: {
        'isFinished': finished,
        'sortBy': finished ? 'StartDate desc' : 'StartDate',
        'includeTotalCount': true,
        'page': page,
        'pageSize': pageSize,
      });

  Future<TripPlan> activate(int id) async =>
      fromJson((await sendAction('POST', '$endpoint/$id/Activate'))!);

  Future<TripPlan> complete(int id) async =>
      fromJson((await sendAction('POST', '$endpoint/$id/Complete'))!);

  Future<TripPlan> cancel(int id) async =>
      fromJson((await sendAction('POST', '$endpoint/$id/Cancel'))!);

  /// The API puts a new destination at the end of its day.
  Future<TripPlanItem> addItem(
    int planId, {
    required int destinationId,
    required int dayNumber,
    String? notes,
  }) async =>
      TripPlanItem.fromJson((await sendAction('POST', '$endpoint/$planId/Items', {
        'destinationId': destinationId,
        'dayNumber': dayNumber,
        'notes': notes,
      }))!);

  Future<TripPlanItem> updateItem(
    int planId,
    TripPlanItem item, {
    required int dayNumber,
    String? notes,
  }) async =>
      TripPlanItem.fromJson((await sendAction('PUT', '$endpoint/$planId/Items/${item.id}', {
        'dayNumber': dayNumber,
        'orderIndex': item.orderIndex,
        'notes': notes,
      }))!);

  Future<void> removeItem(int planId, int itemId) async {
    await send(
      (headers) => http.delete(buildUri('$endpoint/$planId/Items/$itemId'), headers: headers),
    );
  }
}
