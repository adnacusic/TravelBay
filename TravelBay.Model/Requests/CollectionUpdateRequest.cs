using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class CollectionUpdateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
