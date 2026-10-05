using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class TripPlanItemInsertRequest
    {
        [Required]
        public int DestinationId { get; set; }

        [Required]
        public int DayNumber { get; set; }

        public int OrderIndex { get; set; }

        public string? Notes { get; set; }
    }
}
