using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public int EntityId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        public int? PerformedByUserId { get; set; }

        [ForeignKey("PerformedByUserId")]
        public User? PerformedByUser { get; set; }

        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Details { get; set; }
    }
}
