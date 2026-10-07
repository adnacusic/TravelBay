namespace TravelBay.Model.Responses
{
    public class SavedDestinationResponse
    {
        public int Id { get; set; }
        public int DestinationId { get; set; }
        public string DestinationName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;

        /// <summary>Cover image of the destination (lowest OrderIndex), null when it has none.</summary>
        public string? ImageUrl { get; set; }
        public DateTime SavedAt { get; set; }
    }
}
