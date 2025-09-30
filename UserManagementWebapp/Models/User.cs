using System.ComponentModel.DataAnnotations;
using UserManagementWebapp.Data;

namespace UserManagementWebapp.Models
{
    public class User
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
        public byte[]? PasswordHash { get; set; }
        public Status Status { get; set; } = Status.Unverified;
        public DateTime? LastLogin { get; set; }
        public bool isVerified { get; set; } = false;
    }
}
