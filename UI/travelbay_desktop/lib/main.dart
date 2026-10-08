import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'providers/ai_agent_provider.dart';
import 'providers/auth_provider.dart';
import 'providers/category_provider.dart';
import 'providers/city_provider.dart';
import 'providers/country_provider.dart';
import 'providers/dashboard_provider.dart';
import 'providers/destination_image_provider.dart';
import 'providers/destination_provider.dart';
import 'providers/news_provider.dart';
import 'providers/notification_provider.dart';
import 'providers/report_provider.dart';
import 'providers/review_provider.dart';
import 'providers/user_provider.dart';
import 'screens/login_screen.dart';
import 'utils/app_navigator.dart';

void main() {
  AuthProvider.onSessionExpired = () {
    appNavigatorKey.currentState?.pushAndRemoveUntil(
      MaterialPageRoute<void>(
        builder: (context) => const LoginScreen(sessionExpired: true),
      ),
      (route) => false,
    );
  };

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => DashboardProvider()),
        ChangeNotifierProvider(create: (_) => AiAgentProvider()),
        ChangeNotifierProvider(create: (_) => DestinationProvider()),
        ChangeNotifierProvider(create: (_) => DestinationImageProvider()),
        ChangeNotifierProvider(create: (_) => CategoryProvider()),
        ChangeNotifierProvider(create: (_) => CountryProvider()),
        ChangeNotifierProvider(create: (_) => CityProvider()),
        ChangeNotifierProvider(create: (_) => ReviewProvider()),
        ChangeNotifierProvider(create: (_) => NewsProvider()),
        ChangeNotifierProvider(create: (_) => UserProvider()),
        ChangeNotifierProvider(create: (_) => NotificationProvider()),
        ChangeNotifierProvider(create: (_) => ReportProvider()),
      ],
      child: const TravelBayAdminApp(),
    ),
  );
}

class TravelBayAdminApp extends StatelessWidget {
  const TravelBayAdminApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'TravelBay Admin',
      navigatorKey: appNavigatorKey,
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: const Color(0xFF2F6F73),
          dynamicSchemeVariant: DynamicSchemeVariant.tonalSpot,
        ),
        inputDecorationTheme: const InputDecorationTheme(
          border: OutlineInputBorder(),
          isDense: true,
        ),
        cardTheme: const CardThemeData(elevation: 0.5),
      ),
      home: const LoginScreen(),
    );
  }
}
