using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace UserManagementWebapp.Models
{
    public class EmailVerification
    {
        [NotMapped]
        public const double DurationHours = 1.0;

        public int Id { get; set; }
        public User User { get; set; }
        public DateTime Expiration { get; set; } = DateTime.UtcNow.AddHours(DurationHours);
        public bool Used { get; set; } = false;
        public byte[] TokenHash { get; set; }
    
    
        public static string GenVerificationToken()
        {
            const int tokenSize = 256 / 8;
            byte[] tokenBytes = new byte[tokenSize];
            RandomNumberGenerator.Fill(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}
