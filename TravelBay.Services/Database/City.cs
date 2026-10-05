using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; } = null!;

        public ICollection<Destination> Destinations { get; set; } = new List<Destination>();
    }
}
