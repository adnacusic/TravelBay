namespace TravelBay.Model.Responses
{
    public class DashboardStatsResponse
    {
        public int TotalDestinations { get; set; }
        public int TotalUsers { get; set; }
        public int DestinationsWithImages { get; set; }
        public int DestinationsWithoutImages { get; set; }
        public int DestinationsWithKeywords { get; set; }
        public int DestinationsWithoutKeywords { get; set; }

        /// <summary>Destinations that have both keywords and at least one image.</summary>
        public int ProcessedDestinations { get; set; }

        /// <summary>ProcessedDestinations / TotalDestinations * 100, one decimal (0 when there are no destinations).</summary>
        public double ProcessedPercent { get; set; }

        public List<RecentDestinationResponse> RecentDestinations { get; set; } = new List<RecentDestinationResponse>();
    }

    public class RecentDestinationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool HasKeywords { get; set; }
        public bool HasImages { get; set; }
    }
}
