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
    
    private Roles _role;
    
    [NotMapped] // This will not be directly mapped to/from the database
    public Roles Role
    {
        get => _role;
        set 
        {
            // Ensure role value is valid
            if (!Enum.IsDefined(typeof(Roles), value))
            {
                // If it's an integer that might map to a valid role, try to convert it
                int intValue = (int)value;
                if (intValue >= 0 && intValue <= 2)
                {
                    if (Enum.IsDefined(typeof(Roles), intValue))
                    {
                        Roles mappedRole = (Roles)intValue;
                        _role = mappedRole;
                        return;
                    }
                }
                
                _role = Roles.EnvironmentalScientist;
            }
            else
            {
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
            // Direct mapping between enum value and database ID (both use 0, 1, 2)
            return (int)_role;
        }
        set 
        {
            // Direct mapping between database ID and enum value (both use 0, 1, 2)
            if (value < 0 || value > 2)
            {
                _role = Roles.EnvironmentalScientist;
                return;
            }
            
            // Direct mapping
            _role = (Roles)value;
        }
    }
}
