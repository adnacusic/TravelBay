import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../layouts/master_screen.dart';
import '../../models/news.dart';
import '../../providers/news_provider.dart';
import '../../utils/app_navigator.dart';
import '../../widgets/empty_state.dart';
import '../../widgets/paged_list.dart';
import 'news_details_screen.dart';
import 'news_widgets.dart';

/// All news, newest first, loaded page by page.
class NewsListScreen extends StatelessWidget {
  const NewsListScreen({super.key});

  static const _pageSize = 10;

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      title: 'Novosti',
      child: PagedList<News>(
        padding: const EdgeInsets.all(12),
        fetchPage: (page) => context.read<NewsProvider>().latest(page: page, pageSize: _pageSize),
        empty: const EmptyState(
          icon: Icons.newspaper_outlined,
          text: 'Još nema objavljenih novosti.',
        ),
        itemBuilder: (context, news) => NewsCard(
          news: news,
          onTap: () => openPage<void>(context, NewsDetailsScreen(news: news)),
        ),
      ),
    );
  }
}
