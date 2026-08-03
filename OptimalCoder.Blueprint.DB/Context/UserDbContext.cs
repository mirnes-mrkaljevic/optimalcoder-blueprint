using Microsoft.EntityFrameworkCore;
using OptimalCoder.Blueprint.DB.Entities;

namespace OptimalCoder.Blueprint.DB.Context
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
       : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                 .HasMany(u => u.Roles)
                 .WithMany(r => r.Users)
                 .UsingEntity<Dictionary<string, object>>(
                     "UserRole",
                     right => right.HasOne<Role>()
                                   .WithMany()
                                   .HasForeignKey("RoleId"),
                     left => left.HasOne<User>()
                                 .WithMany()
                                 .HasForeignKey("UserId"),
                     join =>
                     {
                         join.HasKey("UserId", "RoleId");
                     });
        }

    }
}
