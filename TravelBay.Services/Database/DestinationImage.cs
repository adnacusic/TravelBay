using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    public class DestinationImage
    {
        [Key]
        public int Id { get; set; }

        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public Destination Destination { get; set; } = null!;

        /// <summary>Absolute URL for external images, or the API-relative content path for uploaded ones.</summary>
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Source { get; set; }

        public int OrderIndex { get; set; }

        public bool IsAiGenerated { get; set; }

        /// <summary>Set only for images uploaded by an admin; the bytes live in the Asset blob store.</summary>
        public int? AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
