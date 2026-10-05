namespace TravelBay.Model.SearchObjects
{
    public class DestinationSearchObject : BaseSearchObject
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int? CityId { get; set; }
        public bool? IncludeCategory { get; set; }
        public bool? IncludeImages { get; set; }
    }
}
