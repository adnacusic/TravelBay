using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class CollectionItemInsertRequest
    {
        [Required]
        public int DestinationId { get; set; }
    }
}
