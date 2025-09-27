using UserManagementWebapp.Data;

namespace UserManagementWebapp.Models
{
    public class User
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public Status status { get; set; } = Status.Unverified;
        public DateTime? LastLogin { get; set; }
    }
}
