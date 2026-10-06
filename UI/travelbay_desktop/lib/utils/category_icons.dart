import 'package:flutter/material.dart';

class CategoryIcon {
  const CategoryIcon(this.name, this.label, this.icon);

  /// Value stored in Category.IconName (shared with the mobile app).
  final String name;
  final String label;
  final IconData icon;
}

/// The icon set a category can use; seed data uses the first five names.
const categoryIcons = <CategoryIcon>[
  CategoryIcon('beach', 'Plaža', Icons.beach_access),
  CategoryIcon('mountain', 'Planina', Icons.landscape),
  CategoryIcon('landmark', 'Znamenitost', Icons.account_balance),
  CategoryIcon('restaurant', 'Hrana', Icons.restaurant),
  CategoryIcon('nature', 'Priroda', Icons.park),
  CategoryIcon('city', 'Grad', Icons.location_city),
  CategoryIcon('museum', 'Muzej', Icons.museum),
  CategoryIcon('hiking', 'Planinarenje', Icons.hiking),
  CategoryIcon('spa', 'Wellness', Icons.spa),
  CategoryIcon('nightlife', 'Noćni život', Icons.nightlife),
];

IconData categoryIconData(String? name) {
  for (final icon in categoryIcons) {
    if (icon.name == name) {
      return icon.icon;
    }
  }
  return Icons.category_outlined;
}
