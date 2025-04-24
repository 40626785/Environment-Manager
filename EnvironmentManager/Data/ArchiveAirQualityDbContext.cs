using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class ArchiveAirQualityDbContext : DbContext
    {
        public ArchiveAirQualityDbContext(DbContextOptions<ArchiveAirQualityDbContext> options)
            : base(options)
        {
        }

        public DbSet<ArchiveAirQuality> ArchiveAirQuality { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArchiveAirQuality>().ToTable("Archive_Air_Quality");
            modelBuilder.Entity<ArchiveAirQuality>().HasKey(a => a.Id);
        }
    }
}

