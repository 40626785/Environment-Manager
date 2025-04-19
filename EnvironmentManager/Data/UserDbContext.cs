using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data;
public class UserDbContext : DbContext
{
    public UserDbContext()
    { }
    
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    { }

    public virtual DbSet<User> User { get; set; }
}