import 'package:flutter/material.dart';

import '../utils/app_config.dart';

/// Image from the API or an external URL, with a neutral placeholder when it is missing or broken.
class NetworkThumbnail extends StatelessWidget {
  const NetworkThumbnail({
    super.key,
    required this.imageUrl,
    required this.width,
    required this.height,
    this.borderRadius = 6,
  });

  final String? imageUrl;
  final double width;
  final double height;
  final double borderRadius;

  @override
  Widget build(BuildContext context) {
    final url = imageUrl;
    return ClipRRect(
      borderRadius: BorderRadius.circular(borderRadius),
      child: SizedBox(
        width: width,
        height: height,
        child: url == null
            ? _placeholder(context, Icons.image_not_supported_outlined)
            : Image.network(
                AppConfig.resolveImageUrl(url),
                fit: BoxFit.cover,
                errorBuilder: (context, error, stackTrace) =>
                    _placeholder(context, Icons.broken_image_outlined),
                loadingBuilder: (context, child, progress) => progress == null
                    ? child
                    : _placeholder(context, Icons.image_outlined),
              ),
      ),
    );
  }

  Widget _placeholder(BuildContext context, IconData icon) {
    final scheme = Theme.of(context).colorScheme;
    return ColoredBox(
      color: scheme.surfaceContainerHighest,
      child: Icon(icon, color: scheme.onSurfaceVariant, size: height / 2.5),
    );
  }
}
