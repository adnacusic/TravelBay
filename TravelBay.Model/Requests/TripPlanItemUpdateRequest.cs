using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class TripPlanItemUpdateRequest
    {
        [Required]
        public int DayNumber { get; set; }

        public int OrderIndex { get; set; }

        public string? Notes { get; set; }
    }
}
