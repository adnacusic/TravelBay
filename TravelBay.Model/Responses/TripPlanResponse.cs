using TravelBay.Model.Enums;

namespace TravelBay.Model.Responses
{
    public class TripPlanResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TripPlanStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TripPlanItemResponse> Items { get; set; } = new List<TripPlanItemResponse>();
    }
}
