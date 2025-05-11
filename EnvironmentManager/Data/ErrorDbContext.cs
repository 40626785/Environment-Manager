using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    /// <summary>
    /// Represents the ErrorDbContext database context.
    /// </summary>
    public class ErrorDbContext : DbContext
    {
        public ErrorDbContext(DbContextOptions<ErrorDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the ErrorEntry table in the database.
        /// </summary>
        public DbSet<ErrorEntry> Errors { get; set; }

        /// <summary>
        /// Configures the model and relationships using the ModelBuilder.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ErrorEntry>().ToTable("ErrorTable");
            /// <summary>
            /// Sets the primary key for the entity using e.ErrorID).
            /// </summary>
            modelBuilder.Entity<ErrorEntry>().HasKey(e => e.ErrorID);
        }
    }
}