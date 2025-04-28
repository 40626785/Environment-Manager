using System.Globalization;
using EnvironmentManager.Models;
using Microsoft.Maui.Graphics;

namespace EnvironmentManager.Converters
{
    public class RoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Roles role)
            {
                return role switch
                {
                    Roles.Administrator => Colors.DarkRed,
                    Roles.EnvironmentalScientist => Colors.DarkGreen,
                    Roles.OperationsManager => Colors.DarkBlue,
                    _ => Colors.Gray
                };
            }
            
            if (value is int roleId)
            {
                return roleId switch
                {
                    0 => Colors.DarkRed,      // Administrator
                    1 => Colors.DarkGreen,    // Environmental Scientist
                    2 => Colors.DarkBlue,     // Operations Manager
                    _ => Colors.Gray          // Unknown
                };
            }
            
            if (value is string roleStr && int.TryParse(roleStr, out int parsedRoleId))
            {
                return parsedRoleId switch
                {
                    0 => Colors.DarkRed,      // Administrator
                    1 => Colors.DarkGreen,    // Environmental Scientist
                    2 => Colors.DarkBlue,     // Operations Manager
                    _ => Colors.Gray          // Unknown
                };
            }
            
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Color to role conversion is not typically needed
            return Roles.EnvironmentalScientist;
        }
    }
} 