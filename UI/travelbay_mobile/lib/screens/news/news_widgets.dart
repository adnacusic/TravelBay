import 'package:flutter/material.dart';

import '../../models/news.dart';
import '../../utils/formatters.dart';
import '../../widgets/network_thumbnail.dart';

/// News item in a list: image, title, publication date and the start of the text.
class NewsCard extends StatelessWidget {
  const NewsCard({super.key, required this.news, required this.onTap});

  static const _imageHeight = 160.0;
  static const _previewLines = 3;

  final News news;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
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
              padding: const EdgeInsets.fromLTRB(12, 10, 12, 12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(news.title, style: theme.textTheme.titleSmall),
                  const SizedBox(height: 2),
                  Text(formatDate(news.publishedAt), style: theme.textTheme.labelSmall),
                  const SizedBox(height: 6),
                  Text(
                    news.content,
                    maxLines: _previewLines,
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.bodyMedium,
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
