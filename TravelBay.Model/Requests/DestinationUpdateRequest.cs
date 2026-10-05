using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class DestinationUpdateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int CityId { get; set; }

        public string? Keywords { get; set; }
    }
}
