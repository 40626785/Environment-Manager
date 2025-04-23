using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data;
/// <summary>
/// This class keeps application user data in sync with the database by maintaining a DbSet of User objects.
/// </summary>
public class UserDbContext : DbContext
{
    public UserDbContext()
    { }
    
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    { }

    public virtual DbSet<User> User { get; set; }
}