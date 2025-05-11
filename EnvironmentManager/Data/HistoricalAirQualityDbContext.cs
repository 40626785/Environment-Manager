using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    /// <summary>
    /// Represents the HistoricalAirQualityDbContext database context.
    /// </summary>
    public class HistoricalAirQualityDbContext : DbContext
    {
        public HistoricalAirQualityDbContext(DbContextOptions<HistoricalAirQualityDbContext> options) : base(options) { }

        // DbSet for ArchiveAirQuality table
        /// <summary>
        /// Gets or sets the ArchiveAirQuality table in the database.
        /// </summary>
        public DbSet<ArchiveAirQuality> ArchiveAirQuality { get; set; }
    }
}