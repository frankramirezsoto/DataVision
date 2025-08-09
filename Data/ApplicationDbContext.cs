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
        }
    }
}
