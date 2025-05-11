using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    /// <summary>
    /// Represents the AirQualityDbContext database context.
    /// </summary>
    public class AirQualityDbContext : DbContext
    {
        public AirQualityDbContext(DbContextOptions<AirQualityDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the AirQualityRecord table in the database.
        /// </summary>
        public DbSet<AirQualityRecord> AirQuality { get; set; }

        /// <summary>
        /// Configures the model and relationships using the ModelBuilder.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AirQualityRecord>().ToTable("Air_Quality");
            /// <summary>
            /// Sets the primary key for the AirQualityRecord entity.
            /// </summary>
            modelBuilder.Entity<AirQualityRecord>().HasKey(a => a.Id);
        }
    }
}