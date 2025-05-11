using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    /// <summary>
    /// Represents the HistoricalDataDbContext database context.
    /// </summary>
    public class HistoricalDataDbContext : DbContext
    {
        public HistoricalDataDbContext()
        {
        }

        public HistoricalDataDbContext(DbContextOptions<HistoricalDataDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the ArchiveAirQuality table in the database.
        /// </summary>
        public virtual DbSet<ArchiveAirQuality> ArchiveAirQuality { get; set; }
        /// <summary>
        /// Gets or sets the ArchiveWaterQuality table in the database.
        /// </summary>
        public virtual DbSet<ArchiveWaterQuality> ArchiveWaterQuality { get; set; }
        /// <summary>
        /// Gets or sets the ArchiveWeatherData table in the database.
        /// </summary>
        public virtual DbSet<ArchiveWeatherData> ArchiveWeatherData { get; set; }

        /// <summary>
        /// Configures the model and relationships using the ModelBuilder.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /// <summary>
            /// Configures the model and relationships using the ModelBuilder.
            /// </summary>
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArchiveAirQuality>(entity =>
            {
                entity.ToTable("Archive_Air_Quality");
                /// <summary>
                /// Sets the primary key for the entity using e.Id).
                /// </summary>
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ArchiveWaterQuality>(entity =>
            {
                entity.ToTable("Archive_Water_Quality");
                /// <summary>
                /// Sets the primary key for the entity using e.Id).
                /// </summary>
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ArchiveWeatherData>(entity =>
            {
                entity.ToTable("archive_weather_data");
                /// <summary>
                /// Sets the primary key for the entity using e.Date_Time).
                /// </summary>
                entity.HasKey(e => e.Date_Time); // Composite keys aren't used here, Date_Time is the primary
            });
        }
    }
}