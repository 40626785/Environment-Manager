using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? Colors.Green : Colors.Red;
            }
            
            return Colors.Gray; // Default color for null or invalid value
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Not implemented as we don't need to convert from Color to bool
            throw new NotImplementedException();
        }
    }
} 