import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/category.dart';
import '../../models/destination.dart';
import '../../models/recommendation.dart';
import '../../providers/auth_provider.dart';
import '../../providers/category_provider.dart';
import '../../providers/destination_provider.dart';
import '../../providers/recommendation_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/category_icons.dart';
import '../../widgets/destination_widgets.dart';
import '../../widgets/network_thumbnail.dart';
import '../../widgets/notification_bell.dart';
import '../destination/destination_details_screen.dart';

/// Start tab: personal recommendations, quick category access and popular destinations.
class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key, required this.onOpenCategory, required this.activation});

  /// Opens the search tab filtered by the category.
  final ValueChanged<Category> onOpenCategory;

  /// Changes every time the tab is opened again; preferences or views changed elsewhere
  /// change the recommendations, so they are loaded again.
  final int activation;

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  static const _recommendationCount = 10;
  static const _popularCount = 10;
  static const _categoryListSize = 100;

  List<Recommendation>? _recommendations;
  List<Category>? _categories;
  List<Destination>? _popular;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _error = null);
    try {
      final recommendationsFuture =
          context.read<RecommendationProvider>().get(pageSize: _recommendationCount);
      final categoriesFuture = context
          .read<CategoryProvider>()
          .get(filter: {'pageSize': _categoryListSize, 'sortBy': 'Name'});
      final popularFuture = context.read<DestinationProvider>().popular(top: _popularCount);

      final recommendations = await recommendationsFuture;
      final categories = await categoriesFuture;
      final popular = await popularFuture;
      if (mounted) {
        setState(() {
          _recommendations = recommendations.items;
          _categories = categories.items.where((c) => c.isActive).toList();
          _popular = popular;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    }
  }

  @override
  void didUpdateWidget(HomeScreen oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.activation != widget.activation) {
      _load();
    }
  }

  /// After a destination was viewed, the recommendations and popularity change.
  Future<void> _openDestination(Destination destination) async {
    await openPage<void>(context, DestinationDetailsScreen(destinationId: destination.id));
    if (mounted) {
      _load();
    }
  }

  @override
  Widget build(BuildContext context) {
    final firstName = context.watch<AuthProvider>().displayName.split(' ').first;

    return Scaffold(
      appBar: AppBar(
        title: Text(firstName.isEmpty ? 'TravelBay' : 'Zdravo, $firstName'),
        actions: const [NotificationBell()],
      ),
      body: RefreshIndicator(
        onRefresh: _load,
        child: _buildBody(),
      ),
    );
  }

  Widget _buildBody() {
    if (_error != null) {
      return ListView(
        padding: const EdgeInsets.all(24),
        children: [
          Text(_error!, textAlign: TextAlign.center),
          const SizedBox(height: 16),
          Center(
            child: OutlinedButton(onPressed: _load, child: const Text('Pokušaj ponovo')),
          ),
        ],
      );
    }
    if (_recommendations == null) {
      return const Center(child: CircularProgressIndicator());
    }

    return ListView(
      padding: const EdgeInsets.only(bottom: 24),
      children: [
        const _SectionTitle('Preporuke za tebe'),
        _buildRecommendations(_recommendations!),
        const _SectionTitle('Kategorije'),
        _buildCategories(_categories ?? []),
        const _SectionTitle('Popularne destinacije'),
        for (final destination in _popular ?? <Destination>[])
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 2),
            child: DestinationTile(
              destination: destination,
              onTap: () => _openDestination(destination),
            ),
          ),
      ],
    );
  }

  Widget _buildRecommendations(List<Recommendation> recommendations) {
    if (recommendations.isEmpty) {
      return const Padding(
        padding: EdgeInsets.symmetric(horizontal: 16),
        child: Text(
          'Još nemamo preporuke za tebe. Pregledaj nekoliko destinacija ili odaberi '
          'omiljene kategorije u profilu.',
        ),
      );
    }

    return SizedBox(
      height: 292,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 12),
        itemCount: recommendations.length,
        separatorBuilder: (context, index) => const SizedBox(width: 10),
        itemBuilder: (context, index) => _RecommendationCard(
          recommendation: recommendations[index],
          onTap: () => _openDestination(recommendations[index].destination),
        ),
      ),
    );
  }

  Widget _buildCategories(List<Category> categories) {
    return SizedBox(
      height: 48,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 12),
        itemCount: categories.length,
        separatorBuilder: (context, index) => const SizedBox(width: 8),
        itemBuilder: (context, index) {
          final category = categories[index];
          return ActionChip(
            avatar: Icon(categoryIconData(category.iconName), size: 18),
            label: Text(category.name),
            onPressed: () => widget.onOpenCategory(category),
          );
        },
      ),
    );
  }
}

class _SectionTitle extends StatelessWidget {
  const _SectionTitle(this.text);

  final String text;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 20, 16, 10),
      child: Text(text, style: Theme.of(context).textTheme.titleMedium),
    );
  }
}

/// Recommended destination with its match percentage and the recommender's explanation.
class _RecommendationCard extends StatelessWidget {
  const _RecommendationCard({required this.recommendation, required this.onTap});

  static const _width = 250.0;

  final Recommendation recommendation;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final destination = recommendation.destination;

    return SizedBox(
      width: _width,
      child: Card(
        clipBehavior: Clip.antiAlias,
        margin: EdgeInsets.zero,
        child: InkWell(
          onTap: onTap,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Stack(
                children: [
                  NetworkThumbnail(
                    imageUrl: destination.coverImage?.imageUrl,
                    width: _width,
                    height: 130,
                    borderRadius: 0,
                  ),
                  Positioned(
                    top: 8,
                    right: 8,
                    child: Container(
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                      decoration: BoxDecoration(
                        color: theme.colorScheme.primary,
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Text(
                        '${recommendation.matchPercent}% podudaranje',
                        style: TextStyle(
                          color: theme.colorScheme.onPrimary,
                          fontSize: 12,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
              Padding(
                padding: const EdgeInsets.all(10),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      destination.name,
                      style: theme.textTheme.titleSmall,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    Text(
                      [destination.cityName, ?destination.category?.name].join(' · '),
                      style: theme.textTheme.bodySmall,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 4),
                    RatingLabel(
                      averageRating: destination.averageRating,
                      reviewCount: destination.reviewCount,
                    ),
                    const SizedBox(height: 6),
                    Text(
                      recommendation.explanation,
                      style: theme.textTheme.bodySmall?.copyWith(fontStyle: FontStyle.italic),
                      maxLines: 3,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
