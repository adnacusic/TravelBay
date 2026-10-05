using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class CollectionInsertRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
