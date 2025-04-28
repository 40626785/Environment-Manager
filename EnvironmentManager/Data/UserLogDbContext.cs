using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data;

/// <summary>
/// Database context for user change logging
/// </summary>
public class UserLogDbContext : DbContext
{
    public UserLogDbContext(DbContextOptions<UserLogDbContext> options) : base(options)
    {
    }
    
    public virtual DbSet<UserLog> UserLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
    }
} 