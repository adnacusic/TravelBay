import 'dart:async';

import 'package:flutter/material.dart';

/// Search box that reports the text once typing pauses (or immediately on Enter).
class SearchField extends StatefulWidget {
  const SearchField({
    super.key,
    required this.label,
    required this.onSearch,
    this.controller,
    this.width = 320,
  });

  final String label;
  final ValueChanged<String> onSearch;
  final TextEditingController? controller;
  final double width;

  @override
  State<SearchField> createState() => _SearchFieldState();
}

class _SearchFieldState extends State<SearchField> {
  static const _debounce = Duration(milliseconds: 400);

  Timer? _timer;

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  void _onChanged(String text) {
    _timer?.cancel();
    _timer = Timer(_debounce, () => widget.onSearch(text.trim()));
  }

  void _onSubmitted(String text) {
    _timer?.cancel();
    widget.onSearch(text.trim());
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: widget.width,
      child: TextField(
        controller: widget.controller,
        onChanged: _onChanged,
        onSubmitted: _onSubmitted,
        decoration: InputDecoration(
          labelText: widget.label,
          prefixIcon: const Icon(Icons.search),
        ),
      ),
    );
  }
}
