import 'package:flutter/material.dart';

import '../models/category.dart';
import '../screens/home/home_screen.dart';
import '../screens/profile/profile_screen.dart';
import '../screens/search/search_screen.dart';
import '../widgets/coming_soon.dart';

enum AppTab { home, search, trips, saved, profile }

/// Main shell after login: bottom navigation with the five tabs from the app plan.
/// Tabs keep their state (IndexedStack) while the user switches between them.
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

  @override
  void initState() {
    super.initState();
    final message = widget.welcomeMessage;
    if (message != null) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(message)));
        }
      });
    }
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
          HomeScreen(onOpenCategory: _openCategory),
          SearchScreen(
            key: ValueKey(_searchRequest),
            initialCategoryId: _searchCategoryId,
          ),
          const ComingSoon(
            title: 'Putovanja',
            icon: Icons.map_outlined,
            text: 'Planovi putovanja s itinerarom po danima stižu u sljedećem koraku.',
          ),
          const ComingSoon(
            title: 'Sačuvano',
            icon: Icons.bookmark_border,
            text: 'Sačuvane destinacije i kolekcije stižu u sljedećem koraku.',
          ),
          const ProfileScreen(),
        ],
      ),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _tab.index,
        onDestinationSelected: (index) => setState(() => _tab = AppTab.values[index]),
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
