using CC.Model;
using System;
using Windows.UI.Xaml.Data;

namespace CC.Converters
{
    public class StringToLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((FillingAnswerModel)value).Needed.Length;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
