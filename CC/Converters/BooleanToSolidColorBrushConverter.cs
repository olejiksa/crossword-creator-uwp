using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace CC.Converters
{
    public class BooleanToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                SolidColorBrush brush = (SolidColorBrush)Application.Current.Resources["SystemControlBackgroundAccentBrush"];
                brush.Opacity = 0.5;
                return brush;
            }
            else
                return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
