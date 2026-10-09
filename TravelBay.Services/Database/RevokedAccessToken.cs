using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    /// <summary>
    /// An access token (by its jti) that was signed out before it expired. JWTs are stateless, so
    /// without this list a stolen or "logged out" token would stay valid until its expiry.
    /// Entries are only needed until the token expires and are cleaned up on the next logout.
    /// </summary>
    public class RevokedAccessToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Jti { get; set; } = string.Empty;

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public DateTime RevokedAt { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
