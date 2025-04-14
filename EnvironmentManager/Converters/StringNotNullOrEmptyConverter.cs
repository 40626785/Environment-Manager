using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Converters
{
    public class StringNotNullOrEmptyConverter : IValueConverter
    {
        // Update signature to match IValueConverter with nullable types
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Returns true if the string is not null or empty, false otherwise.
            return !string.IsNullOrEmpty(value as string);
        }

        // Update signature to match IValueConverter with nullable types
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
                // Convert back from boolean to string
            // When converting from a boolean back to a string:
            // - If true, we return a non-empty placeholder string
            // - If false, we return an empty string
            if (value is bool boolValue)
            {
                return boolValue ? "Value" : string.Empty;
            }
            
            // Default case - return empty string
            return string.Empty;
        }
    }
}
