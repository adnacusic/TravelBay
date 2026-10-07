import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/category.dart';
import '../providers/notification_provider.dart';
import '../screens/home/home_screen.dart';
import '../screens/profile/profile_screen.dart';
import '../screens/saved/saved_screen.dart';
import '../screens/search/search_screen.dart';
import '../screens/trips/trip_list_screen.dart';

enum AppTab { home, search, trips, saved, profile }

/// Main shell after login: bottom navigation with the five tabs from the app plan.
/// Tabs keep their state (IndexedStack) while the user switches between them; the tabs
/// that show data changed elsewhere reload when they are opened again. While this screen
/// is open, the unread notification count is polled.
class ContainerScreen extends StatefulWidget {
  const ContainerScreen({super.key, this.welcomeMessage});

  /// Shown once after a new account was created.
  final String? welcomeMessage;

  @override
  State<ContainerScreen> createState() => _ContainerScreenState();
}

class _ContainerScreenState extends State<ContainerScreen> {
  AppTab _tab = AppTab.home;

  /// Category picked on the home screen; a new request rebuilds the search tab with it.
  int? _searchCategoryId;
  int _searchRequest = 0;

  /// How many times each tab was opened; a tab reloads its data when its number changes.
  final Map<AppTab, int> _activations = {for (final tab in AppTab.values) tab: 0};

  late final NotificationProvider _notifications;

  @override
  void initState() {
    super.initState();
    _notifications = context.read<NotificationProvider>()..startPolling();
    final message = widget.welcomeMessage;
    if (message != null) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(message)));
        }
      });
    }
  }

  @override
  void dispose() {
    _notifications.stopPolling();
    super.dispose();
  }

  void _selectTab(AppTab tab) {
    setState(() {
      if (tab != _tab) {
        _activations[tab] = _activations[tab]! + 1;
      }
      _tab = tab;
    });
  }

  void _openCategory(Category category) {
    setState(() {
      _searchCategoryId = category.id;
      _searchRequest++;
      _tab = AppTab.search;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(
        index: _tab.index,
        children: [
          HomeScreen(onOpenCategory: _openCategory, activation: _activations[AppTab.home]!),
          SearchScreen(
            key: ValueKey(_searchRequest),
            initialCategoryId: _searchCategoryId,
          ),
          TripListScreen(activation: _activations[AppTab.trips]!),
          SavedScreen(activation: _activations[AppTab.saved]!),
          ProfileScreen(activation: _activations[AppTab.profile]!),
        ],
      ),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _tab.index,
        onDestinationSelected: (index) => _selectTab(AppTab.values[index]),
        destinations: const [
          NavigationDestination(
            icon: Icon(Icons.home_outlined),
            selectedIcon: Icon(Icons.home),
            label: 'Početna',
          ),
          NavigationDestination(
            icon: Icon(Icons.search),
            label: 'Pretraga',
          ),
          NavigationDestination(
            icon: Icon(Icons.map_outlined),
            selectedIcon: Icon(Icons.map),
            label: 'Putovanja',
          ),
          NavigationDestination(
            icon: Icon(Icons.bookmark_border),
            selectedIcon: Icon(Icons.bookmark),
            label: 'Sačuvano',
          ),
          NavigationDestination(
            icon: Icon(Icons.person_outline),
            selectedIcon: Icon(Icons.person),
            label: 'Profil',
          ),
        ],
      ),
    );
  }
}
