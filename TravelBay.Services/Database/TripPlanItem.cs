using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    public class TripPlanItem
    {
        [Key]
        public int Id { get; set; }

        public int TripPlanId { get; set; }

        [ForeignKey("TripPlanId")]
        public TripPlan TripPlan { get; set; } = null!;

        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public Destination Destination { get; set; } = null!;

        public int DayNumber { get; set; }

        public int OrderIndex { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
