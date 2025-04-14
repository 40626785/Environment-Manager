using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace EnvironmentManager.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert back from color to boolean
            if (value is Color color)
            {
                // Check if the color is closer to the active color (#22C55E) than the inactive color (#EF4444)
                // This is a simple implementation - in a real app you might want more sophisticated color comparison
                if (color.ToHex().Equals("#22C55E", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (color.ToHex().Equals("#EF4444", StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            
            // Default fallback
            return false;
        }
    }
} 