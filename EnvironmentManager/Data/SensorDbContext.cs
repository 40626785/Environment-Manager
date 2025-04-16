using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class SensorDbContext : DbContext
    {
        public SensorDbContext()
        { }
        
        public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options)
        { }

        public virtual DbSet<Sensor> Sensors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.ToTable("Sensors");
                entity.HasKey(e => e.SensorId);
                entity.HasOne(e => e.Location)
                      .WithMany(e => e.Sensors)
                      .HasForeignKey(e => e.LocationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EnvironmentManager.Models.Location>(entity =>
            {
                entity.ToTable("Locations");
                entity.HasKey(e => e.LocationId);
            });






        }
    }
} 