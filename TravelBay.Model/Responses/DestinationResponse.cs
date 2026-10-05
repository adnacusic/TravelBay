namespace TravelBay.Model.Responses
{
    public class DestinationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public CategoryResponse? Category { get; set; }
        public int CityId { get; set; }
        public string? Keywords { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<DestinationImageResponse> Images { get; set; } = new List<DestinationImageResponse>();

        /// <summary>Computed from Approved reviews only — never stored on the entity.</summary>
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
