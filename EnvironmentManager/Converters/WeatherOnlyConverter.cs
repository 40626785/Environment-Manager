using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Converters
{
    public class WeatherOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "Weather";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}