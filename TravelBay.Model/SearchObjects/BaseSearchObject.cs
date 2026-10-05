using TravelBay.Model.Constants;

namespace TravelBay.Model.SearchObjects
{
    public class BaseSearchObject
    {
        private int? _pageSize = PagingDefaults.DefaultPageSize;

        public int? Page { get; set; } = 1;

        /// <summary>Missing value falls back to the default; anything above the max is reduced to the max.</summary>
        public int? PageSize
        {
            get => _pageSize;
            set => _pageSize = value.HasValue
                ? Math.Clamp(value.Value, 1, PagingDefaults.MaxPageSize)
                : PagingDefaults.DefaultPageSize;
        }

        public bool? IncludeTotalCount { get; set; } = false;
        public string? SortBy { get; set; }
    }
}
