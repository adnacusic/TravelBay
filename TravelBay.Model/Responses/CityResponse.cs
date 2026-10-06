namespace TravelBay.Model.Responses
{
    public class CityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;

        /// <summary>Filled in lists only: destinations (not deleted) in this city.</summary>
        public int DestinationCount { get; set; }
    }
}
