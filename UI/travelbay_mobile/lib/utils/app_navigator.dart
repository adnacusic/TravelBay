import 'package:flutter/material.dart';

/// Lets non-widget code (e.g. an expired session in a provider) navigate.
final GlobalKey<NavigatorState> appNavigatorKey = GlobalKey<NavigatorState>();

/// Switching between main menu sections replaces the whole stack (no back to the previous section).
void openSection(BuildContext context, Widget screen) {
  Navigator.of(context).pushAndRemoveUntil(
    PageRouteBuilder<void>(
      pageBuilder: (context, animation, secondaryAnimation) => screen,
      transitionDuration: Duration.zero,
      reverseTransitionDuration: Duration.zero,
    ),
    (route) => false,
  );
}

/// Forms and details open on top of the current section and return a result when closed.
Future<T?> openPage<T>(BuildContext context, Widget screen) {
  return Navigator.of(context).push<T>(
    MaterialPageRoute<T>(builder: (context) => screen),
  );
}
