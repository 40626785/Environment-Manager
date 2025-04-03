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
            // Not needed for one-way binding
            throw new NotImplementedException();
        }
    }
}
