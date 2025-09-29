using System.ComponentModel.DataAnnotations.Schema;
using UserManagementWebapp.Data;
using static UserManagementWebapp.Helpers.Hasher;

namespace UserManagementWebapp.Models
{
    public class Salt
    {
        public int Id { get; set; }
        public byte[] SaltValue { get; set; } = GenSalt();
        [ForeignKey("UserId")]
        public required User User { get; set; }
        public required SaltPurpose Purpose { get; set; }
    }
}
