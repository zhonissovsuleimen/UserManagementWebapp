using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static UserManagementWebapp.Helpers.PasswordHasher;

namespace UserManagementWebapp.Models
{
    public class Salt
    {
        public int Id { get; set; }
        public byte[] SaltValue { get; set; } = GenSalt();
        [ForeignKey("UserId")]
        public required User User { get; set; }
    }
}
