using CC.Model;
using System;
using Windows.UI.Xaml.Data;

namespace CC.Converters
{
    public class FillingGridWordToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
                return (FillingGridWordModel)value;
            return value;
        }
    }
}
