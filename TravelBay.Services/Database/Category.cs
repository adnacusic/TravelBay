using System.ComponentModel.DataAnnotations;

namespace TravelBay.Services.Database
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? IconName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Destination> Destinations { get; set; } = new List<Destination>();
        public ICollection<UserPreference> UserPreferences { get; set; } = new List<UserPreference>();
    }
}
