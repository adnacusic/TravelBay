using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class ReviewRejectRequest
    {
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}
