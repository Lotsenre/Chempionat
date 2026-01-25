using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VendingMachineDesktop.Utilities.Converters;

public class BoolToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isChecked && isChecked)
        {
            return new SolidColorBrush(Color.FromRgb(232, 245, 253)); // Светло-голубой фон когда активно
        }
        return new SolidColorBrush(Colors.White); // Белый фон когда неактивно
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return false;
    }
}
