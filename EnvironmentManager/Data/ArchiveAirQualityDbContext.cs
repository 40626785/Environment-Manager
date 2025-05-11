using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    /// <summary>
    /// Represents the ArchiveAirQualityDbContext database context.
    /// </summary>
    public class ArchiveAirQualityDbContext : DbContext
    {
        public ArchiveAirQualityDbContext(DbContextOptions<ArchiveAirQualityDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the ArchiveAirQuality table in the database.
        /// </summary>
        public DbSet<ArchiveAirQuality> ArchiveAirQuality { get; set; }

        /// <summary>
        /// Configures the model and relationships using the ModelBuilder.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArchiveAirQuality>().ToTable("Archive_Air_Quality");
            /// <summary>
            /// Sets the primary key for the ArchiveAirQuality entity.
            /// </summary>
            modelBuilder.Entity<ArchiveAirQuality>().HasKey(a => a.Id);
        }
    }
}
