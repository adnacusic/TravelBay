using System.ComponentModel.DataAnnotations;

namespace TravelBay.Model.Requests
{
    public class NewsUpdateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
