using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    public class ErrorDbContext : DbContext
    {
        public ErrorDbContext(DbContextOptions<ErrorDbContext> options)
            : base(options)
        {
        }

        public DbSet<ErrorEntry> Errors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ErrorEntry>().ToTable("ErrorTable");
            modelBuilder.Entity<ErrorEntry>().HasKey(e => e.ErrorID);
        }
    }
}
