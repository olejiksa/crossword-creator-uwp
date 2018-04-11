using CC.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace CC.Converters
{
    public class StringToCharArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            char[] charArray = ((string)value).ToCharArray();
            List<SymbolItem> symbolItems = new List<SymbolItem>();

            foreach (char item in charArray)
                symbolItems.Add(new SymbolItem { Symbol = item } );

            return symbolItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
