using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    /// <summary>
    /// Represents the LogDbContext database context.
    /// </summary>
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the LogEntry table in the database.
        /// </summary>
        public DbSet<LogEntry> Logs { get; set; }

        /// <summary>
        /// Configures the model and relationships using the ModelBuilder.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>().ToTable("LogTable");
            /// <summary>
            /// Sets the primary key for the entity using l.LogID).
            /// </summary>
            modelBuilder.Entity<LogEntry>().HasKey(l => l.LogID);
        }
    }
}