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
                    Roles.BasicUser => "Basic User",
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
                    0 => "Basic User",
                    1 => "Administrator",
                    2 => "Environmental Scientist",
                    3 => "Operations Manager",
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
                    "Basic User" => Roles.BasicUser,
                    "Administrator" => Roles.Administrator,
                    "Environmental Scientist" => Roles.EnvironmentalScientist,
                    "Operations Manager" => Roles.OperationsManager,
                    _ => Roles.BasicUser
                };
            }
            
            return Roles.BasicUser;
        }
    }
} 