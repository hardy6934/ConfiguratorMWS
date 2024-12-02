
using System.Globalization; 
using System.Windows.Data;
using System.Windows;

namespace ConfiguratorMWS.src.Behaviors
{ 
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Проверяем, есть ли параметр (например, "Invert")
                if (parameter != null && parameter.ToString() == "Invert")
                    boolValue = !boolValue;

                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BoolToVisibilityConverter can only be used for one-way binding.");
        }
    }
}
