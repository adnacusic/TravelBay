import '../models/dashboard_stats.dart';
import 'base_provider.dart';

class DashboardProvider extends BaseProvider<DashboardStats> {
  DashboardProvider() : super('Dashboard');

  @override
  DashboardStats fromJson(Map<String, dynamic> json) =>
      DashboardStats.fromJson(json);

  Future<DashboardStats> getStats() async =>
      fromJson(await getJson('$endpoint/Stats'));
}
