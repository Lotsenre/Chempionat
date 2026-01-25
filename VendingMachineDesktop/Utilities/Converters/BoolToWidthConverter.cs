using System.Globalization;
using System.Windows.Data;

namespace VendingMachineDesktop.Utilities.Converters;

public class BoolToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isOpen && parameter is string width && double.TryParse(width, out double widthValue))
        {
            return isOpen ? widthValue : 60.0; // Collapsed width shows only icons
        }
        return 250.0; // Default width
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
