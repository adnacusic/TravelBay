import 'package:flutter/material.dart';

/// Page opened on top of the tabs (details, forms): app bar with the system
/// "Back" arrow and a title. Forms also get an "X" in the top right corner.
class MasterScreen extends StatelessWidget {
  const MasterScreen({
    super.key,
    required this.title,
    required this.child,
    this.actions = const [],
    this.isForm = false,
  });

  final String title;
  final Widget child;
  final List<Widget> actions;
  final bool isForm;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(title),
        actions: [
          ...actions,
          if (isForm)
            IconButton(
              tooltip: 'Zatvori',
              icon: const Icon(Icons.close),
              onPressed: () => Navigator.maybePop(context),
            ),
        ],
      ),
      body: SafeArea(child: child),
    );
  }
}
