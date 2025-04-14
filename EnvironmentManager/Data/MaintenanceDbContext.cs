using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data;
public class MaintenanceDbContext : DbContext
{
    public MaintenanceDbContext()
    { }
    
    public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : base(options)
    { }

    public virtual DbSet<Maintenance> Maintenance { get; set; }


}
