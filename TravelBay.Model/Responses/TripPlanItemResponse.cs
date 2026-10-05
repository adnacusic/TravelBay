namespace TravelBay.Model.Responses
{
    public class TripPlanItemResponse
    {
        public int Id { get; set; }
        public int DestinationId { get; set; }
        public string DestinationName { get; set; } = string.Empty;
        public int DayNumber { get; set; }
        public int OrderIndex { get; set; }
        public string? Notes { get; set; }
    }
}
