import 'package:flutter/material.dart';

class PaginationBar extends StatelessWidget {
  const PaginationBar({
    super.key,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.onPageChanged,
  });

  /// 1-based, like the API.
  final int page;
  final int pageSize;
  final int totalCount;
  final ValueChanged<int> onPageChanged;

  int get _pageCount => totalCount == 0 ? 1 : (totalCount / pageSize).ceil();

  @override
  Widget build(BuildContext context) {
    final from = totalCount == 0 ? 0 : (page - 1) * pageSize + 1;
    final to = (page * pageSize).clamp(0, totalCount);

    return Row(
      mainAxisAlignment: MainAxisAlignment.end,
      children: [
        Text('$from–$to od ukupno $totalCount'),
        const SizedBox(width: 16),
        IconButton(
          tooltip: 'Prethodna strana',
          onPressed: page > 1 ? () => onPageChanged(page - 1) : null,
          icon: const Icon(Icons.chevron_left),
        ),
        Text('Strana $page / $_pageCount'),
        IconButton(
          tooltip: 'Sljedeća strana',
          onPressed: page < _pageCount ? () => onPageChanged(page + 1) : null,
          icon: const Icon(Icons.chevron_right),
        ),
      ],
    );
  }
}
