using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class HistoricalAirQualityDbContext : DbContext
    {
        public HistoricalAirQualityDbContext(DbContextOptions<HistoricalAirQualityDbContext> options) : base(options) { }

        // DbSet for ArchiveAirQuality table
        public DbSet<ArchiveAirQuality> ArchiveAirQuality { get; set; }
    }
}
