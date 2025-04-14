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
        // Location entities are now managed by LocationDbContext to follow SRP
        public virtual DbSet<SensorReading> SensorReadings { get; set; }
        public virtual DbSet<SensorSetting> SensorSettings { get; set; }
        // EnvironmentalParameters are now managed by EnvironmentalParameterDbContext to follow SRP

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

            modelBuilder.Entity<SensorReading>(entity =>
            {
                entity.ToTable("SensorReadings");
                entity.HasKey(e => e.ReadingId);
                entity.HasOne(e => e.Sensor)
                      .WithMany(e => e.Readings)
                      .HasForeignKey(e => e.SensorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // EnvironmentalParameter configuration moved to EnvironmentalParameterDbContext

            modelBuilder.Entity<SensorSetting>(entity =>
            {
                entity.ToTable("SensorSettings");
                entity.HasKey(e => e.SettingId);
                entity.HasOne(e => e.Sensor)
                      .WithMany(e => e.Settings)
                      .HasForeignKey(e => e.SensorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 