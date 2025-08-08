using Microsoft.EntityFrameworkCore;
using DataVision.Models;

namespace DataVision.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("User");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.Username).IsUnique();

                // Relationship with Logs
                entity.HasMany(e => e.Logs)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Log configuration
            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.FechaConsulta).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.EndpointConsultado).IsRequired().HasMaxLength(255);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Create initial admin user
            // Password: admin123 (hashed with BCrypt)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2a$11$8GZQpZkf5I.YUPKpE8LESO.qTHFKg1O8QY5HTBGRxZFVGwJl1oFrq", // admin123
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    Username = "user1",
                    PasswordHash = "$2a$11$8GZQpZkf5I.YUPKpE8LESO.qTHFKg1O8QY5HTBGRxZFVGwJl1oFrq", // admin123
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
