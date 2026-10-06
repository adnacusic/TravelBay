import 'package:flutter/material.dart';

/// Lets a DataTable fill the available width, but scroll horizontally instead of
/// overflowing when its columns need more room (e.g. a narrow window).
/// DataTable never shrinks a column below its content, so a fixed width overflows.
class ScrollableTable extends StatefulWidget {
  const ScrollableTable({super.key, required this.child});

  final Widget child;

  @override
  State<ScrollableTable> createState() => _ScrollableTableState();
}

class _ScrollableTableState extends State<ScrollableTable> {
  final _horizontalController = ScrollController();

  @override
  void dispose() {
    _horizontalController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) => Scrollbar(
        controller: _horizontalController,
        thumbVisibility: true,
        child: SingleChildScrollView(
          controller: _horizontalController,
          scrollDirection: Axis.horizontal,
          child: ConstrainedBox(
            constraints: BoxConstraints(minWidth: constraints.maxWidth),
            child: widget.child,
          ),
        ),
      ),
    );
  }
}
