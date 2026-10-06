namespace TravelBay.Model.Requests
{
    /// <summary>The image is either ImageUrl (external) or FileName/ContentType/Base64Content (uploaded file).</summary>
    public class NewsInsertRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }

        public string? ImageUrl { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public string? Base64Content { get; set; }
    }
}
