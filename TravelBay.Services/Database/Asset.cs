using System;
using System.ComponentModel.DataAnnotations;

namespace TravelBay.Services.Database
{
    /// <summary>
    /// Generic blob store with no owning entity of its own — consumers (e.g. User.ProfileImageId)
    /// reference it by a nullable FK instead.
    /// </summary>
    public class Asset
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public string Base64Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}