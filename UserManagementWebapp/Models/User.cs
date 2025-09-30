using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateTime? LastLogin { get; set; }
        public bool isBlocked { get; set; } = false;
        public bool isVerified { get; set; } = false;

        [NotMapped]
        public Status Status => isBlocked ? Status.Blocked : isVerified ? Status.Active : Status.Unverified;
    }
}
