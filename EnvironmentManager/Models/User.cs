using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models;

[Table("users")]
[PrimaryKey(nameof(Username))]
public class User
{
    [Required]
    public string? Username { get; set; }
    
    [Required]
    public string? Password { get; set; }
    
    // Foreign Key property (maps to the database column)
    [Column("Role")] // Keep mapping to the existing 'Role' column
    public int RoleId { get; set; } 

    // Enum property for convenience in code (not mapped directly)
    [NotMapped]
    public Roles Role 
    {
        get => (Roles)RoleId; 
        set => RoleId = (int)value;
    }
    
    // Navigation property using the correct integer FK
    [ForeignKey("RoleId")]
    public virtual Role RoleNavigation { get; set; }
}
