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

        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Models.Location> Locations { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<EnvironmentalParameter> EnvironmentalParameters { get; set; }
        public DbSet<SensorSetting> SensorSettings { get; set; }

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

            modelBuilder.Entity<EnvironmentalParameter>(entity =>
            {
                entity.ToTable("EnvironmentalParameters");
                entity.HasKey(e => e.ParameterId);
            });

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