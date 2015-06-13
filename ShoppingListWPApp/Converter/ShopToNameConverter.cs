using ShoppingListWPApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ShoppingListWPApp.Converter
{
    class ShopToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = value as ObservableCollection<Shop>;
            List<string> list = new List<string>();
            if (args == null) return null;
            foreach(Shop s in args)
            {
                list.Add(s.Name);
            }

            return list;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
