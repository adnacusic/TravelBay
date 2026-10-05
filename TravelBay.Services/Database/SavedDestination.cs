using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    /// <summary>Unique (UserId, DestinationId) — see TravelBayConfiguration.</summary>
    public class SavedDestination
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public Destination Destination { get; set; } = null!;

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
