using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class SavedDestinationInsertRequest
    {
        [Required]
        public int DestinationId { get; set; }
    }
}
