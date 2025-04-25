using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.Models;

[Table("Roles")]
public class Role
{
    [Key]
    public int RoleId { get; set; }
    
    [Required]
    public string RoleName { get; set; }
    
    public string Description { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime LastModifiedDate { get; set; }
} 