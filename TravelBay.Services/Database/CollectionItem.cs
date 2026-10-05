using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    public class CollectionItem
    {
        [Key]
        public int Id { get; set; }

        public int CollectionId { get; set; }

        [ForeignKey("CollectionId")]
        public Collection Collection { get; set; } = null!;

        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public Destination Destination { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
