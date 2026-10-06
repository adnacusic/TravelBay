import '../models/dashboard_stats.dart';
import 'api_provider.dart';

class DashboardProvider extends ApiProvider {
  DashboardProvider() : super('Dashboard');

  Future<DashboardStats> getStats() async =>
      DashboardStats.fromJson(await getJson('$endpoint/Stats'));
}
