using System.ComponentModel.DataAnnotations;
using TravelBay.Model.Constants;

namespace TravelBay.Model.SearchObjects
{
    public class BaseSearchObject
    {
        private int? _pageSize = PagingDefaults.DefaultPageSize;

        /// <summary>1-based; 0 or less is rejected with 400 by model validation (it used to end in a 500).</summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be 1 or greater.")]
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
        /// <summary>
        /// Comma-separated "Field [asc|desc]" items, e.g. "LastName, FirstName" or "StartDate desc".
        /// Only real columns of the listed entity are accepted (see BaseReadService).
        /// </summary>
        public string? SortBy { get; set; }
    }
}
