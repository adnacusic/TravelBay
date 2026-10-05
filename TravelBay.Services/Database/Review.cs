using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelBay.Model.Enums;

namespace TravelBay.Services.Database
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public Destination Destination { get; set; } = null!;

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [Required]
        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? ModeratedByUserId { get; set; }

        [ForeignKey("ModeratedByUserId")]
        public User? ModeratedByUser { get; set; }

        public DateTime? ModeratedAt { get; set; }

        [MaxLength(500)]
        public string? ModerationReason { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
