using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EnvironmentManager.Models;

[Table("users")]
[PrimaryKey(nameof(Username))]
public class User
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
    
    private Roles _role;
    
    [NotMapped] // This will not be directly mapped to/from the database
    public Roles Role
    {
        get => _role;
        set 
        {
            // Log the incoming role value
            Debug.WriteLine($"Setting role for user {Username}: Incoming value = {value} (int: {(int)value})");
            
            // Ensure role value is valid
            if (!Enum.IsDefined(typeof(Roles), value))
            {
                Debug.WriteLine($"WARNING: Invalid role value {value} for user {Username}. Setting to default EnvironmentalScientist.");
                
                // If it's an integer that might map to a valid role, try to convert it
                int intValue = (int)value;
                if (intValue >= 0 && intValue <= 2)
                {
                    if (Enum.IsDefined(typeof(Roles), intValue))
                    {
                        Roles mappedRole = (Roles)intValue;
                        Debug.WriteLine($"Mapped invalid role {value} to valid enum {mappedRole} using integer value {intValue}");
                        _role = mappedRole;
                        return;
                    }
                }
                
                _role = Roles.EnvironmentalScientist;
            }
            else
            {
                Debug.WriteLine($"Valid role {value} set for user {Username}");
                _role = value;
            }
        }
    }
    
    // This property will be used for database mapping
    [Column("Role")]
    public int DatabaseRoleId 
    { 
        get 
        {
            // Convert enum value (0-2) to database ID (1-3)
            int roleId = (int)_role + 1;
            Debug.WriteLine($"Converting enum role {_role} (value: {(int)_role}) to database ID {roleId}");
            return roleId;
        }
        set 
        {
            // Convert database ID (1-3) to enum value (0-2)
            Debug.WriteLine($"Setting role from database ID {value}");
            
            if (value < 1 || value > 3)
            {
                Debug.WriteLine($"WARNING: Invalid database role ID {value} for user {Username}. Setting to default EnvironmentalScientist.");
                _role = Roles.EnvironmentalScientist;
                return;
            }
            
            // Map database ID to enum (subtract 1)
            Roles mappedRole = (Roles)(value - 1);
            Debug.WriteLine($"Mapped database ID {value} to enum role {mappedRole}");
            _role = mappedRole;
        }
    }
}
