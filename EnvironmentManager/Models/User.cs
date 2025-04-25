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
    
    [Column("Role")]
    public Roles Role { get; set; }
    
    // Navigation property for the relationship with Role entity
    [ForeignKey("Role")]
    public virtual Role RoleNavigation { get; set; }
}
