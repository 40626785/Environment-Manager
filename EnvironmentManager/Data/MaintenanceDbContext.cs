using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data;
/// <summary>
/// This class keeps application user data in sync with the database by maintaining a DbSet of User objects.
/// </summary>
public class MaintenanceDbContext : DbContext
{
    public MaintenanceDbContext()
    { }
    
    public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : base(options)
    { }

    public virtual DbSet<Maintenance> Maintenance { get; set; }
}
