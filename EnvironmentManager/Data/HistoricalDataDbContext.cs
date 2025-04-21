using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class HistoricalDataDbContext : DbContext
    {
        public HistoricalDataDbContext()
        {
        }

        public HistoricalDataDbContext(DbContextOptions<HistoricalDataDbContext> options) : base(options)
        {
        }

        public virtual DbSet<ArchiveAirQuality> ArchiveAirQuality { get; set; }
        public virtual DbSet<ArchiveWaterQuality> ArchiveWaterQuality { get; set; }
        public virtual DbSet<ArchiveWeatherData> ArchiveWeatherData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArchiveAirQuality>(entity =>
            {
                entity.ToTable("Archive_Air_Quality");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ArchiveWaterQuality>(entity =>
            {
                entity.ToTable("Archive_Water_Quality");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ArchiveWeatherData>(entity =>
            {
                entity.ToTable("archive_weather_data");
                entity.HasKey(e => e.Date_Time); // Composite keys aren't used here, Date_Time is the primary
            });
        }
    }
}
