using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.SearchObjects
{
    public class DestinationSearchObject : BaseSearchObject
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }

        /// <summary>Free text matched against the destination name, its city and its country.</summary>
        public string? SearchText { get; set; }

        /// <summary>Minimum average of approved review ratings (1–5); destinations without ratings are left out.</summary>
        [Range(1, 5, ErrorMessage = "MinRating must be between 1 and 5.")]
        public double? MinRating { get; set; }
        public bool? IncludeCategory { get; set; }
        public bool? IncludeImages { get; set; }
    }
}
