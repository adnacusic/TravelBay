using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBay.Services.Database
{
    /// <summary>
    /// An emailed password reset code. Only its PBKDF2 hash is stored (like a password), it expires,
    /// it works once, and too many wrong attempts lock it.
    /// </summary>
    public class PasswordResetCode
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public string CodeHash { get; set; } = string.Empty;

        [Required]
        public string CodeSalt { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        public int FailedAttempts { get; set; }
    }
}
