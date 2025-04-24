using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data;

/// <summary>
/// Database context for user management
/// </summary>
public class UserManagementDbContext : DbContext
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
    {
    }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure any additional model relationships here
    }
} 