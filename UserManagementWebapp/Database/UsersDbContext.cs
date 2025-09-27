using Microsoft.EntityFrameworkCore;
using UserManagementWebapp.Models;

namespace UserManagementWebapp.Database
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
