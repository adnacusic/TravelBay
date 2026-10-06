namespace TravelBay.Model.Responses
{
    public class NewsResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
