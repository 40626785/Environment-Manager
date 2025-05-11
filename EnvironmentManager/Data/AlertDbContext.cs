using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    /// <summary>
    /// Represents the AlertDbContext database context.
    /// </summary>
    public class AlertDbContext : DbContext
    {
        public AlertDbContext(DbContextOptions<AlertDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the Alert table in the database.
        /// </summary>
        public DbSet<Alert> AlertTable { get; set; }
    }
}