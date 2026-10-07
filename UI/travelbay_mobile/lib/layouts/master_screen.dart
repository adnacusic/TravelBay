import 'package:flutter/material.dart';

/// Page opened on top of the tabs (details, forms): app bar with the system
/// "Back" arrow and a title.
class MasterScreen extends StatelessWidget {
  const MasterScreen({
    super.key,
    required this.title,
    required this.child,
    this.actions = const [],
  });

  final String title;
  final Widget child;
  final List<Widget> actions;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(title), actions: actions),
      body: SafeArea(child: child),
    );
  }
}
