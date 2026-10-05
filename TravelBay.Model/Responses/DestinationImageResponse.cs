namespace TravelBay.Model.Responses
{
    public class DestinationImageResponse
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Source { get; set; }
        public int OrderIndex { get; set; }
        public bool IsAiGenerated { get; set; }
    }
}
