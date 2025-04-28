using Microsoft.EntityFrameworkCore; 
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class ReadingsDbContext : DbContext
    {
        public ReadingsDbContext(DbContextOptions<ReadingsDbContext> options) : base(options) { }

        public DbSet<Reading> Readings { get; set; }
    }
}