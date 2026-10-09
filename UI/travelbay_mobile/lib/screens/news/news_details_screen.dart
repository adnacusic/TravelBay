import 'package:flutter/material.dart';

import '../../layouts/master_screen.dart';
import '../../models/news.dart';
import '../../utils/formatters.dart';
import '../../widgets/network_thumbnail.dart';

/// One news item in full: image, title, publication date and the whole text.
class NewsDetailsScreen extends StatelessWidget {
  const NewsDetailsScreen({super.key, required this.news});

  static const _imageHeight = 220.0;

  final News news;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return MasterScreen(
      title: 'Novost',
      child: ListView(
        children: [
          LayoutBuilder(
            builder: (context, constraints) => NetworkThumbnail(
              imageUrl: news.imageUrl.isEmpty ? null : news.imageUrl,
              width: constraints.maxWidth,
              height: _imageHeight,
              borderRadius: 0,
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(news.title, style: theme.textTheme.headlineSmall),
                const SizedBox(height: 4),
                Text('Objavljeno ${formatDate(news.publishedAt)}', style: theme.textTheme.bodySmall),
                const SizedBox(height: 16),
                Text(news.content, style: theme.textTheme.bodyLarge),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
