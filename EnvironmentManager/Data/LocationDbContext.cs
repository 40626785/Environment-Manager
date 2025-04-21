using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class LocationDbContext : DbContext
    {
        public LocationDbContext()
        { }

        public LocationDbContext(DbContextOptions<LocationDbContext> options) : base(options)
        { }

        public virtual DbSet<EnvironmentManager.Models.Location> Locations { get; set; }
    }
}
