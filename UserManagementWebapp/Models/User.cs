using System.ComponentModel.DataAnnotations;
using UserManagementWebapp.Data;

namespace UserManagementWebapp.Models
{
    public class User
    {
        public required int Id { get; set; }
        public required Guid Guid { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }
        public Status Status { get; set; } = Status.Unverified;
        public DateTime? LastLogin { get; set; }
    }
}
