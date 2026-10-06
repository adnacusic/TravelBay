import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/destination.dart';
import '../../models/enums.dart';
import '../../models/review.dart';
import '../../models/user.dart';
import '../../providers/destination_provider.dart';
import '../../providers/review_provider.dart';
import '../../providers/user_provider.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/pagination_bar.dart';
import 'reject_review_dialog.dart';
import 'review_widgets.dart';
import '../../widgets/scrollable_table.dart';

/// Review moderation. Status only changes through the API state machine
/// (Pending -> Approved / Rejected); the actions are offered only for pending reviews.
class ReviewListScreen extends StatefulWidget {
  const ReviewListScreen({super.key});

  @override
  State<ReviewListScreen> createState() => _ReviewListScreenState();
}

class _ReviewListScreenState extends State<ReviewListScreen> {
  static const _pageSize = 10;
  static const _filterListSize = 100;

  List<Destination> _destinations = [];
  List<User> _users = [];

  int? _destinationFilter;
  int? _userFilter;

  /// Moderation work starts with the reviews that wait for a decision.
  ReviewStatus? _statusFilter = ReviewStatus.pending;

  /// Average of approved reviews for the destination in the filter.
  Destination? _selectedDestination;

  List<Review> _reviews = [];
  int _totalCount = 0;
  int _page = 1;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadFilterData();
    _loadPage(1);
  }

  Future<void> _loadFilterData() async {
    try {
      final destinationsFuture = context.read<DestinationProvider>().get(
        filter: {'pageSize': _filterListSize, 'sortBy': 'Name'},
      );
      final usersFuture = context.read<UserProvider>().get(
        filter: {'pageSize': _filterListSize, 'sortBy': 'LastName, FirstName'},
      );
      final destinations = await destinationsFuture;
      final users = await usersFuture;
      if (mounted) {
        setState(() {
          _destinations = destinations.items;
          _users = users.items;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  Future<void> _loadPage(int page) async {
    setState(() => _isLoading = true);
    try {
      final result = await context.read<ReviewProvider>().get(filter: {
        'destinationId': _destinationFilter,
        'userId': _userFilter,
        'status': _statusFilter?.index,
        'includeTotalCount': true,
        'page': page,
        'pageSize': _pageSize,
      });
      if (mounted) {
        setState(() {
          _reviews = result.items;
          _totalCount = result.totalCount ?? result.items.length;
          _page = page;
        });
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

  Future<void> _loadSelectedDestination() async {
    final id = _destinationFilter;
    if (id == null) {
      setState(() => _selectedDestination = null);
      return;
    }
    try {
      final destination = await context.read<DestinationProvider>().getById(id);
      if (mounted && _destinationFilter == id) {
        setState(() => _selectedDestination = destination);
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  void _applyFilters() {
    _loadPage(1);
    _loadSelectedDestination();
  }

  void _clearFilters() {
    setState(() {
      _destinationFilter = null;
      _userFilter = null;
      _statusFilter = null;
    });
    _applyFilters();
  }

  /// After a moderation action the average rating can change too.
  void _reloadAfterChange() {
    _loadPage(_reviews.length == 1 && _page > 1 ? _page - 1 : _page);
    _loadSelectedDestination();
  }

  Future<void> _approve(Review review) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Odobravanje recenzije',
      message: 'Recenzija korisnika ${review.reviewerDisplayName} za '
          '"${review.destinationName}" postaje javno vidljiva, a autor dobija obavještenje. '
          'Odluka se kasnije ne može promijeniti.',
      confirmLabel: 'Odobri',
      destructive: false,
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<ReviewProvider>().approve(review.id);
      if (mounted) {
        showSuccessMessage(context, 'Recenzija je odobrena i autor je obaviješten.');
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
    if (mounted) {
      _reloadAfterChange();
    }
  }

  Future<void> _reject(Review review) async {
    final rejected = await showDialog<bool>(
      context: context,
      builder: (context) => RejectReviewDialog(review: review),
    );
    if (rejected == true && mounted) {
      showSuccessMessage(context, 'Recenzija je odbijena i autor je obaviješten o razlogu.');
      _reloadAfterChange();
    }
  }

  Future<void> _delete(Review review) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje recenzije',
      message: 'Da li ste sigurni da želite obrisati recenziju korisnika '
          '${review.reviewerDisplayName} za "${review.destinationName}"? '
          'Recenzija više neće biti vidljiva niti uračunata u prosječnu ocjenu.',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<ReviewProvider>().remove(review.id);
      if (mounted) {
        showSuccessMessage(context, 'Recenzija je obrisana.');
        _reloadAfterChange();
      }
    } on Exception catch (e) {
      if (mounted) {
        await showErrorDialog(context, e);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      section: AdminSection.reviews,
      title: 'Moderacija recenzija',
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _buildFilters(),
            if (_selectedDestination != null) ...[
              const SizedBox(height: 12),
              _buildAverageRating(_selectedDestination!),
            ],
            const SizedBox(height: 16),
            Expanded(child: _buildTable()),
            const SizedBox(height: 8),
            PaginationBar(
              page: _page,
              pageSize: _pageSize,
              totalCount: _totalCount,
              onPageChanged: _loadPage,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFilters() {
    return Wrap(
      spacing: 16,
      runSpacing: 12,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: [
        SizedBox(
          width: 300,
          child: DropdownButtonFormField<int?>(
            key: ValueKey('destination-$_destinationFilter'),
            initialValue: _destinationFilter,
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'Destinacija'),
            items: [
              const DropdownMenuItem<int?>(value: null, child: Text('Sve destinacije')),
              for (final destination in _destinations)
                DropdownMenuItem<int?>(
                  value: destination.id,
                  child: Text(destination.name, overflow: TextOverflow.ellipsis),
                ),
            ],
            onChanged: (value) {
              setState(() => _destinationFilter = value);
              _applyFilters();
            },
          ),
        ),
        SizedBox(
          width: 260,
          child: DropdownButtonFormField<int?>(
            key: ValueKey('user-$_userFilter'),
            initialValue: _userFilter,
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'Korisnik'),
            items: [
              const DropdownMenuItem<int?>(value: null, child: Text('Svi korisnici')),
              for (final user in _users)
                DropdownMenuItem<int?>(
                  value: user.id,
                  child: Text(user.fullName, overflow: TextOverflow.ellipsis),
                ),
            ],
            onChanged: (value) {
              setState(() => _userFilter = value);
              _applyFilters();
            },
          ),
        ),
        SizedBox(
          width: 200,
          child: DropdownButtonFormField<ReviewStatus?>(
            key: ValueKey('status-$_statusFilter'),
            initialValue: _statusFilter,
            decoration: const InputDecoration(labelText: 'Status'),
            items: [
              const DropdownMenuItem<ReviewStatus?>(value: null, child: Text('Svi statusi')),
              for (final status in ReviewStatus.values)
                DropdownMenuItem<ReviewStatus?>(value: status, child: Text(status.label)),
            ],
            onChanged: (value) {
              setState(() => _statusFilter = value);
              _applyFilters();
            },
          ),
        ),
        TextButton.icon(
          onPressed: _clearFilters,
          icon: const Icon(Icons.filter_alt_off_outlined),
          label: const Text('Očisti filtere'),
        ),
      ],
    );
  }

  Widget _buildAverageRating(Destination destination) {
    final rating = destination.averageRating;
    return Card(
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        child: Row(
          children: [
            Icon(Icons.star, color: Theme.of(context).colorScheme.tertiary),
            const SizedBox(width: 8),
            Text(
              rating == null
                  ? '${destination.name}: još nema odobrenih recenzija.'
                  : '${destination.name}: prosječna ocjena ${rating.toStringAsFixed(1)} '
                      '(${destination.reviewCount} odobrenih recenzija)',
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildTable() {
    if (_isLoading && _reviews.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_reviews.isEmpty) {
      return Center(
        child: Text(
          _statusFilter == ReviewStatus.pending
              ? 'Nema recenzija koje čekaju moderaciju.'
              : 'Nema recenzija koje odgovaraju zadanim filterima.',
        ),
      );
    }

    return Card(
      clipBehavior: Clip.antiAlias,
      child: Stack(
        children: [
          SingleChildScrollView(
            child: ScrollableTable(
              child: DataTable(
                showCheckboxColumn: false,
                columns: const [
                  DataColumn(label: Text('Destinacija')),
                  DataColumn(label: Text('Korisnik')),
                  DataColumn(label: Text('Ocjena')),
                  DataColumn(label: Text('Komentar')),
                  DataColumn(label: Text('Datum')),
                  DataColumn(label: Text('Status')),
                  DataColumn(label: Text('Akcije')),
                ],
                rows: [
                  for (final review in _reviews)
                    DataRow(
                      onSelectChanged: (_) => showDialog<void>(
                        context: context,
                        builder: (context) => ReviewDetailsDialog(review: review),
                      ),
                      cells: [
                        DataCell(Text(review.destinationName)),
                        DataCell(Text(review.reviewerDisplayName)),
                        DataCell(RatingStars(rating: review.rating, size: 16)),
                        DataCell(
                          ConstrainedBox(
                            constraints: const BoxConstraints(maxWidth: 320),
                            child: Text(
                              review.comment ?? '-',
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                        ),
                        DataCell(Text(formatDate(review.createdAt))),
                        DataCell(ReviewStatusChip(status: review.status)),
                        DataCell(_buildActions(review)),
                      ],
                    ),
                ],
              ),
            ),
          ),
          if (_isLoading) const LinearProgressIndicator(),
        ],
      ),
    );
  }

  Widget _buildActions(Review review) {
    final blockedReason = moderationBlockedReason(review);
    final scheme = Theme.of(context).colorScheme;

    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Tooltip(
          message: blockedReason ?? 'Odobri',
          child: IconButton(
            icon: Icon(Icons.check_circle_outline,
                color: blockedReason == null ? scheme.primary : null),
            onPressed: blockedReason == null ? () => _approve(review) : null,
          ),
        ),
        Tooltip(
          message: blockedReason ?? 'Odbij (uz razlog)',
          child: IconButton(
            icon: Icon(Icons.block, color: blockedReason == null ? scheme.error : null),
            onPressed: blockedReason == null ? () => _reject(review) : null,
          ),
        ),
        IconButton(
          tooltip: 'Obriši',
          icon: Icon(Icons.delete_outline, color: scheme.error),
          onPressed: () => _delete(review),
        ),
      ],
    );
  }
}
