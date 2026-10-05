using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    public class Destination
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; } = null!;

        /// <summary>Comma-separated keywords filled in by the AI worker agent.</summary>
        public string? Keywords { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<DestinationImage> Images { get; set; } = new List<DestinationImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<TripPlanItem> TripPlanItems { get; set; } = new List<TripPlanItem>();
        public ICollection<CollectionItem> CollectionItems { get; set; } = new List<CollectionItem>();
        public ICollection<SavedDestination> SavedByUsers { get; set; } = new List<SavedDestination>();
        public ICollection<ViewHistory> ViewHistoryEntries { get; set; } = new List<ViewHistory>();
    }
}
