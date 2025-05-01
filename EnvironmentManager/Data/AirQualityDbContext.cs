using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class AirQualityDbContext : DbContext
    {
        public AirQualityDbContext(DbContextOptions<AirQualityDbContext> options)
            : base(options)
        {
        }

        public DbSet<AirQualityRecord> AirQuality { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AirQualityRecord>().ToTable("Air_Quality");
            modelBuilder.Entity<AirQualityRecord>().HasKey(a => a.Id);
        }
    }
}
