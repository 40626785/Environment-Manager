using System.Globalization;
using EnvironmentManager.Models;

namespace EnvironmentManager.Converters
{
    public class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Roles role)
            {
                return role switch
                {
                    Roles.Administrator => "Administrator",
                    Roles.EnvironmentalScientist => "Environmental Scientist",
                    Roles.OperationsManager => "Operations Manager",
                    _ => $"Unknown Role ({(int)role})"
                };
            }
            
            if (value is int roleId)
            {
                return roleId switch
                {
                    0 => "Administrator",
                    1 => "Environmental Scientist", 
                    2 => "Operations Manager",
                    _ => $"Unknown Role ({roleId})"
                };
            }
            
            return "Unknown Role";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string roleName)
            {
                return roleName switch
                {
                    "Administrator" => Roles.Administrator,
                    "Environmental Scientist" => Roles.EnvironmentalScientist,
                    "Operations Manager" => Roles.OperationsManager,
                    _ => Roles.EnvironmentalScientist // Default
                };
            }
            
            return Roles.EnvironmentalScientist; // Default
        }
    }
} 