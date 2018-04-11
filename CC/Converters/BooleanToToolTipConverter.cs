using CC.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace CC.Converters
{
    class BooleanToToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? StringHelper.ToString("HideMenu") : StringHelper.ToString("ShowMenu");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}