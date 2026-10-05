using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class TripPlanUpdateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
