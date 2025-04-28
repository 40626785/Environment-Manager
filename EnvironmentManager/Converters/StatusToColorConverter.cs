using System.Globalization;

namespace EnvironmentManager.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                bool isBackground = parameter is string paramStr && 
                    paramStr.Equals("Background", StringComparison.OrdinalIgnoreCase);
                
                return status.ToLower() switch
                {
                    "online" => isBackground ? new Color(0x4A/255f, 0xDE/255f, 0x80/255f) : Colors.Green,
                    "offline" => isBackground ? new Color(0xF8/255f, 0x71/255f, 0x71/255f) : Colors.Red,
                    "degraded" => isBackground ? new Color(0xFB/255f, 0xBF/255f, 0x24/255f) : Colors.Orange,
                    "maintenance" => isBackground ? new Color(0x60/255f, 0xA5/255f, 0xFA/255f) : Colors.Blue,
                    _ => isBackground ? new Color(0x9C/255f, 0xA3/255f, 0xAF/255f) : Colors.Gray
                };
            }
            
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 