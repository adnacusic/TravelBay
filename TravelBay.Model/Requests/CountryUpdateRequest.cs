using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class CountryUpdateRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
