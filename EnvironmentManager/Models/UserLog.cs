using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models;

[Table("UserLogs")]
public class UserLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string ActionType { get; set; }  // "CREATE", "UPDATE", "DELETE"
    
    public string? ChangedFields { get; set; }  // JSON or comma-separated list of changed fields
    
    public string? OldValues { get; set; }  // JSON representation of old values
    
    public string? NewValues { get; set; }  // JSON representation of new values
    
    public string? PerformedBy { get; set; }  // Username of person who made the change
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.Now;
} 