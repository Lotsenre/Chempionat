using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VendingMachineDesktop.Utilities.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status.ToLower() switch
            {
                "working" or "работает" => new SolidColorBrush(Colors.Green),
                "notworking" or "не работает" => new SolidColorBrush(Colors.Red),
                "onmaintenance" or "на обслуживании" => new SolidColorBrush(Colors.Blue),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
