import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/dashboard_stats.dart';
import 'base_provider.dart';

class DashboardProvider extends BaseProvider<DashboardStats> {
  DashboardProvider() : super('Dashboard');

  @override
  DashboardStats fromJson(Map<String, dynamic> json) =>
      DashboardStats.fromJson(json);

  Future<DashboardStats> getStats() async {
    final response = await send(
      (headers) => http.get(buildUri('$endpoint/Stats'), headers: headers),
    );
    return fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }
}
