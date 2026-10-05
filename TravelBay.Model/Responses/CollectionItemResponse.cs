namespace TravelBay.Model.Responses
{
    public class CollectionItemResponse
    {
        public int Id { get; set; }
        public int DestinationId { get; set; }
        public string DestinationName { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
    }
}
