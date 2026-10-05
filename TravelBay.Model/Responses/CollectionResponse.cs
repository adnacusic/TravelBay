namespace TravelBay.Model.Responses
{
    public class CollectionResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<CollectionItemResponse> Items { get; set; } = new List<CollectionItemResponse>();
    }
}
