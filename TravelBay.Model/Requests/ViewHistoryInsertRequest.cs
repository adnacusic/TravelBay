using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class ViewHistoryInsertRequest
    {
        [Required]
        public int DestinationId { get; set; }
    }
}
