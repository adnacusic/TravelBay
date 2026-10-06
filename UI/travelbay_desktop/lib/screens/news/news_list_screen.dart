import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/news.dart';
import '../../providers/news_provider.dart';
import '../../utils/app_navigator.dart';
import '../../utils/dialogs.dart';
import '../../utils/formatters.dart';
import '../../widgets/network_thumbnail.dart';
import '../../widgets/pagination_bar.dart';
import '../../widgets/search_field.dart';
import 'news_form_screen.dart';

class NewsListScreen extends StatefulWidget {
  const NewsListScreen({super.key});

  @override
  State<NewsListScreen> createState() => _NewsListScreenState();
}

class _NewsListScreenState extends State<NewsListScreen> {
  static const _pageSize = 10;

  final _searchController = TextEditingController();

  List<News> _news = [];
  int _totalCount = 0;
  int _page = 1;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadPage(1);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  /// The API orders news by publication date, newest first.
  Future<void> _loadPage(int page) async {
    setState(() => _isLoading = true);
    try {
      final result = await context.read<NewsProvider>().get(filter: {
        'title': _searchController.text.trim(),
        'includeTotalCount': true,
        'page': page,
        'pageSize': _pageSize,
      });
      if (mounted) {
        setState(() {
          _news = result.items;
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

  Future<void> _openForm([News? news]) async {
    final message = await openPage<String>(context, NewsFormScreen(newsId: news?.id));
    if (message != null && mounted) {
      showSuccessMessage(context, message);
      _loadPage(news == null ? 1 : _page);
    }
  }

  Future<void> _delete(News news) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Brisanje novosti',
      message: 'Da li ste sigurni da želite trajno obrisati novost "${news.title}"? '
          'Brisanje se ne može poništiti.',
    );
    if (!confirmed || !mounted) {
      return;
    }

    try {
      await context.read<NewsProvider>().remove(news.id);
      if (mounted) {
        showSuccessMessage(context, 'Novost "${news.title}" je obrisana.');
        _loadPage(_news.length == 1 && _page > 1 ? _page - 1 : _page);
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
      section: AdminSection.news,
      title: 'Novosti',
      actions: [
        FilledButton.icon(
          onPressed: () => _openForm(),
          icon: const Icon(Icons.add),
          label: const Text('Nova novost'),
        ),
      ],
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                SearchField(
                  label: 'Pretraga po naslovu',
                  controller: _searchController,
                  onSearch: (_) => _loadPage(1),
                ),
              ],
            ),
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

  Widget _buildTable() {
    if (_isLoading && _news.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_news.isEmpty) {
      return const Center(child: Text('Nema novosti koje odgovaraju pretrazi.'));
    }

    return Card(
      clipBehavior: Clip.antiAlias,
      child: Stack(
        children: [
          SingleChildScrollView(
            child: SizedBox(
              width: double.infinity,
              child: DataTable(
                showCheckboxColumn: false,
                dataRowMinHeight: 64,
                dataRowMaxHeight: 72,
                columns: const [
                  DataColumn(label: Text('Slika')),
                  DataColumn(label: Text('Naslov')),
                  DataColumn(label: Text('Tekst')),
                  DataColumn(label: Text('Objavljeno')),
                  DataColumn(label: Text('Akcije')),
                ],
                rows: [
                  for (final news in _news)
                    DataRow(
                      onSelectChanged: (_) => _openForm(news),
                      cells: [
                        DataCell(
                          NetworkThumbnail(imageUrl: news.imageUrl, width: 92, height: 52),
                        ),
                        DataCell(
                          ConstrainedBox(
                            constraints: const BoxConstraints(maxWidth: 280),
                            child: Text(news.title, overflow: TextOverflow.ellipsis),
                          ),
                        ),
                        DataCell(
                          ConstrainedBox(
                            constraints: const BoxConstraints(maxWidth: 360),
                            child: Text(
                              news.content,
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                        ),
                        DataCell(Text(formatDateTime(news.publishedAt))),
                        DataCell(
                          Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              IconButton(
                                tooltip: 'Uredi',
                                icon: const Icon(Icons.edit_outlined),
                                onPressed: () => _openForm(news),
                              ),
                              IconButton(
                                tooltip: 'Obriši',
                                icon: Icon(
                                  Icons.delete_outline,
                                  color: Theme.of(context).colorScheme.error,
                                ),
                                onPressed: () => _delete(news),
                              ),
                            ],
                          ),
                        ),
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
}
