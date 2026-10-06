namespace TravelBay.Model.Responses
{
    public class CountryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>Filled in lists only.</summary>
        public int CityCount { get; set; }
    }
}
