using CC.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace CC.Converters
{
    public class ObjectToCharArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            FillingAnswerModel model = (FillingAnswerModel)value;
            List<SymbolItem> symbolItems = new List<SymbolItem>();

            if (model.Filled != null)
            {
                char[] charArray = model.Filled?.ToCharArray();

                foreach (char item in charArray)
                    symbolItems.Add(new SymbolItem { Symbol = item });
            }

            while (symbolItems.Count < model.Needed.Length)
                symbolItems.Add(new SymbolItem { Symbol = ' ' });

            return symbolItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}