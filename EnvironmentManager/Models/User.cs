using Microsoft.EntityFrameworkCore;
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
    [Required]
    public Roles Role { get; set; } //represented as integer in database and returns string value in application
}
