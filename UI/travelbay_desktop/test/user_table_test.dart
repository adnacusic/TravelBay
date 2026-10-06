import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:travelbay_desktop/models/user.dart';
import 'package:travelbay_desktop/screens/users/user_table.dart';

/// A RenderFlex overflow is reported as a test failure, so every size below
/// must lay out the users table without the "OVERFLOWED" indicator.
void main() {
  final users = [
    User(
      id: 1,
      firstName: 'Desktop',
      lastName: 'Admin',
      email: 'desktop@travelbay.local',
      username: 'desktop',
      role: 'Admin',
      createdAt: DateTime.utc(2026, 1, 15),
      lastLoginAt: DateTime.utc(2026, 10, 6, 9, 58),
    ),
    User(
      id: 2,
      firstName: 'Aleksandra-Marija',
      lastName: 'Hadžiabdulahović-Kovačević',
      email: 'aleksandra.marija.hadziabdulahovic.kovacevic@primjer-dugog-domena.com',
      username: 'aleksandra.marija.hadziabdulahovic',
      role: 'User',
      isActive: false,
      createdAt: DateTime.utc(2026, 2, 1),
    ),
  ];

  for (final size in const [Size(1600, 900), Size(1280, 800), Size(1024, 700), Size(800, 600)]) {
    testWidgets('no overflow at ${size.width.toInt()}x${size.height.toInt()}', (tester) async {
      tester.view.physicalSize = size;
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);

      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: SingleChildScrollView(
              child: UserTable(
                users: users,
                currentUserId: 1,
                onOpenDetails: (_) {},
                onToggleActive: (_) {},
              ),
            ),
          ),
        ),
      );

      expect(tester.takeException(), isNull);
      expect(find.text('Korisničko ime'), findsOneWidget);
    });
  }
}
