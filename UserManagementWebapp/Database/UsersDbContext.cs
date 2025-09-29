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
        public DbSet<Salt> Salts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(u => u.Guid)
                    .IsUnique();
                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });
        }
    }
}
