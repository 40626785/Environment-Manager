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
        
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            
            // Configure relationship with Role
            entity.HasOne(d => d.RoleNavigation)
                  .WithMany()
                  .HasForeignKey(d => d.RoleId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_Users_Roles");
        });
        
        // Configure Role entity
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
} 