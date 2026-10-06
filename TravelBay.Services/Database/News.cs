using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>Absolute URL for external images, or the API-relative path of an uploaded one.</summary>
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>Set only for images uploaded by an admin; the bytes live in the Asset blob store.</summary>
        public int? ImageAssetId { get; set; }

        [ForeignKey("ImageAssetId")]
        public Asset? ImageAsset { get; set; }

        /// <summary>Publication date chosen by the admin; lists are ordered by it.</summary>
        public DateTime PublishedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
