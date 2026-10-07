import 'package:flutter/material.dart';

import '../models/search_result.dart';

/// Vertical list that loads its data page by page from the API; the next page is
/// fetched when the user scrolls near the end. Give it a new key to load it again.
class PagedList<T> extends StatefulWidget {
  const PagedList({
    super.key,
    required this.fetchPage,
    required this.itemBuilder,
    required this.empty,
    this.header,
    this.padding = const EdgeInsets.fromLTRB(12, 8, 12, 88),
  });

  /// [page] starts at 1.
  final Future<SearchResult<T>> Function(int page) fetchPage;
  final Widget Function(BuildContext context, T item) itemBuilder;

  /// Shown instead of the list when there is nothing to show.
  final Widget empty;

  /// Optional widget above the first item (e.g. a summary line).
  final Widget? header;
  final EdgeInsets padding;

  @override
  State<PagedList<T>> createState() => _PagedListState<T>();
}

class _PagedListState<T> extends State<PagedList<T>> {
  static const _loadMoreExtent = 300.0;

  final List<T> _items = [];
  int _totalCount = 0;
  int _page = 0;
  bool _isLoading = false;
  String? _error;

  bool get _hasMore => _items.length < _totalCount;

  @override
  void initState() {
    super.initState();
    _loadNextPage();
  }

  Future<void> _loadNextPage() async {
    if (_isLoading) {
      return;
    }
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final result = await widget.fetchPage(_page + 1);
      if (mounted) {
        setState(() {
          _items.addAll(result.items);
          _totalCount = result.totalCount ?? _items.length;
          _page++;
        });
      }
    } on Exception catch (e) {
      if (mounted) {
        setState(() => _error = e.toString());
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  bool _onScroll(ScrollNotification notification) {
    if (_hasMore &&
        !_isLoading &&
        _error == null &&
        notification.metrics.extentAfter < _loadMoreExtent) {
      _loadNextPage();
    }
    return false;
  }

  @override
  Widget build(BuildContext context) {
    if (_items.isEmpty) {
      if (_error != null) {
        return _message(_error!, retry: true);
      }
      if (_isLoading) {
        return const Center(child: CircularProgressIndicator());
      }
      return widget.empty;
    }

    final header = widget.header;
    final extra = (header == null ? 0 : 1);
    return NotificationListener<ScrollNotification>(
      onNotification: _onScroll,
      child: ListView.builder(
        padding: widget.padding,
        itemCount: _items.length + extra + 1,
        itemBuilder: (context, index) {
          if (header != null && index == 0) {
            return header;
          }
          final itemIndex = index - extra;
          if (itemIndex < _items.length) {
            return widget.itemBuilder(context, _items[itemIndex]);
          }
          return _footer();
        },
      ),
    );
  }

  Widget _footer() {
    if (_isLoading) {
      return const Padding(
        padding: EdgeInsets.all(16),
        child: Center(child: CircularProgressIndicator()),
      );
    }
    if (_error != null) {
      return _message(_error!, retry: true);
    }
    if (_hasMore) {
      return Center(
        child: TextButton(onPressed: _loadNextPage, child: const Text('Učitaj još')),
      );
    }
    return const SizedBox.shrink();
  }

  Widget _message(String text, {bool retry = false}) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(text, textAlign: TextAlign.center),
            if (retry) ...[
              const SizedBox(height: 8),
              OutlinedButton(onPressed: _loadNextPage, child: const Text('Pokušaj ponovo')),
            ],
          ],
        ),
      ),
    );
  }
}
