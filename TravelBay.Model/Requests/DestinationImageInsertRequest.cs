namespace TravelBay.Model.Requests
{
    /// <summary>Either ImageUrl (external image) or FileName/ContentType/Base64Content (uploaded file) is set, never both.</summary>
    public class DestinationImageInsertRequest
    {
        public int DestinationId { get; set; }
        public string? ImageUrl { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public string? Base64Content { get; set; }
    }
}
