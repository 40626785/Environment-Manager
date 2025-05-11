using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data
{
    /// <summary>
    /// Represents the UserDbContext database context.
    /// </summary>
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the User table in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Configures the model and relationships using the ModelBuilder.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            /// <summary>
            /// Sets the primary key for the entity using u.Username).
            /// </summary>
            modelBuilder.Entity<User>().HasKey(u => u.Username);
        }
    }
}