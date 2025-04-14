using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class EnvironmentalParameterDbContext : DbContext
    {
        public EnvironmentalParameterDbContext()
        { }
        
        public EnvironmentalParameterDbContext(DbContextOptions<EnvironmentalParameterDbContext> options) : base(options)
        { }

        public virtual DbSet<EnvironmentalParameter> EnvironmentalParameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EnvironmentalParameter>(entity =>
            {
                entity.ToTable("EnvironmentalParameters");
                entity.HasKey(e => e.ParameterId);
            });
        }
    }
}
