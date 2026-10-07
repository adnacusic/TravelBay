class SearchResult<T> {
  SearchResult({this.items = const [], this.totalCount});

  final List<T> items;
  final int? totalCount;
}
