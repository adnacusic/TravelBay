import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../layouts/master_screen.dart';
import '../models/dashboard_stats.dart';
import '../providers/dashboard_provider.dart';
import '../utils/app_navigator.dart';
import '../utils/dialogs.dart';
import '../utils/formatters.dart';
import '../widgets/status_chip.dart';
import 'destinations/destination_form_screen.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  DashboardStats? _stats;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    try {
      final stats = await context.read<DashboardProvider>().getStats();
      if (mounted) {
        setState(() => _stats = stats);
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  Future<void> _openDestination(RecentDestination destination) async {
    final message = await openPage<String>(
      context,
      DestinationFormScreen(destinationId: destination.id),
    );
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _load();
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      section: AdminSection.dashboard,
      title: 'Dashboard',
      actions: [
        OutlinedButton.icon(
          onPressed: _isLoading ? null : _load,
          icon: const Icon(Icons.refresh),
          label: const Text('Osvježi'),
        ),
      ],
      child: _buildBody(),
    );
  }

  Widget _buildBody() {
    final stats = _stats;
    if (stats == null) {
      return Center(
        child: _isLoading
            ? const CircularProgressIndicator()
            : const Text('Podaci nisu učitani. Pokušajte ponovo (Osvježi).'),
      );
    }

    return ListView(
      padding: const EdgeInsets.all(24),
      children: [
        Wrap(
          spacing: 16,
          runSpacing: 16,
          children: [
            _StatCard(
              icon: Icons.place_outlined,
              label: 'Destinacije',
              value: '${stats.totalDestinations}',
            ),
            _StatCard(
              icon: Icons.people_outline,
              label: 'Korisnici',
              value: '${stats.totalUsers}',
            ),
            _StatCard(
              icon: Icons.image_outlined,
              label: 'Sa slikom',
              value: '${stats.destinationsWithImages}',
              detail: 'Bez slike: ${stats.destinationsWithoutImages}',
            ),
            _StatCard(
              icon: Icons.label_outline,
              label: 'Sa ključnim riječima',
              value: '${stats.destinationsWithKeywords}',
              detail:
                  'Bez ključnih riječi: ${stats.destinationsWithoutKeywords}',
            ),
          ],
        ),
        const SizedBox(height: 16),
        _AiStatusCard(stats: stats),
        const SizedBox(height: 16),
        _buildRecentDestinations(stats.recentDestinations),
      ],
    );
  }

  Widget _buildRecentDestinations(List<RecentDestination> destinations) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Nedavno dodane destinacije',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 4),
            Text(
              'Klik na red otvara destinaciju za uređivanje.',
              style: Theme.of(context).textTheme.bodySmall,
            ),
            const SizedBox(height: 8),
            if (destinations.isEmpty)
              const Padding(
                padding: EdgeInsets.symmetric(vertical: 16),
                child: Text('Još nema dodanih destinacija.'),
              )
            else
              SizedBox(
                width: double.infinity,
                child: DataTable(
                  showCheckboxColumn: false,
                  columns: const [
                    DataColumn(label: Text('Naziv')),
                    DataColumn(label: Text('Kategorija')),
                    DataColumn(label: Text('Grad')),
                    DataColumn(label: Text('Dodano')),
                    DataColumn(label: Text('AI status')),
                    DataColumn(label: Text('Kompletnost')),
                  ],
                  rows: [
                    for (final destination in destinations)
                      DataRow(
                        onSelectChanged: (_) => _openDestination(destination),
                        cells: [
                          DataCell(Text(destination.name)),
                          DataCell(Text(destination.categoryName)),
                          DataCell(Text(destination.cityName)),
                          DataCell(Text(formatDate(destination.createdAt))),
                          DataCell(
                            AiStatusChips(
                              hasKeywords: destination.hasKeywords,
                              hasImages: destination.hasImages,
                            ),
                          ),
                          DataCell(
                            Text(
                              destination.isComplete ? 'Kompletna' : 'Nepotpuna',
                            ),
                          ),
                        ],
                      ),
                  ],
                ),
              ),
          ],
        ),
      ),
    );
  }
}

class _StatCard extends StatelessWidget {
  const _StatCard({
    required this.icon,
    required this.label,
    required this.value,
    this.detail,
  });

  final IconData icon;
  final String label;
  final String value;
  final String? detail;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return SizedBox(
      width: 240,
      child: Card(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              CircleAvatar(
                backgroundColor: theme.colorScheme.secondaryContainer,
                foregroundColor: theme.colorScheme.onSecondaryContainer,
                child: Icon(icon),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(label, style: theme.textTheme.bodyMedium),
                    Text(value, style: theme.textTheme.headlineSmall),
                    if (detail != null)
                      Text(detail!, style: theme.textTheme.bodySmall),
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

class _AiStatusCard extends StatelessWidget {
  const _AiStatusCard({required this.stats});

  final DashboardStats stats;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(Icons.smart_toy_outlined, color: theme.colorScheme.primary),
                const SizedBox(width: 8),
                Text('AI obrada sadržaja', style: theme.textTheme.titleMedium),
                const Spacer(),
                Text(
                  '${stats.processedPercent.toStringAsFixed(1)} %',
                  style: theme.textTheme.titleLarge,
                ),
              ],
            ),
            const SizedBox(height: 12),
            LinearProgressIndicator(
              value: stats.processedPercent / 100,
              minHeight: 10,
              borderRadius: BorderRadius.circular(5),
            ),
            const SizedBox(height: 8),
            Text(
              'Obrađeno ${stats.processedDestinations} od ${stats.totalDestinations} '
              'destinacija (imaju i ključne riječi i bar jednu sliku).',
              style: theme.textTheme.bodyMedium,
            ),
          ],
        ),
      ),
    );
  }
}
