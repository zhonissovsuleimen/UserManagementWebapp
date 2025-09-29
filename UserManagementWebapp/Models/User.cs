using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserManagementWebapp.Data;

namespace UserManagementWebapp.Models
{
    public class User
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public byte[]? PasswordHash { get; set; }

        public Status Status { get; set; } = Status.Unverified;
        public DateTime? LastLogin { get; set; }
    }
}
