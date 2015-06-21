using System;
using Windows.UI.Xaml.Data;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.Converter
{
    class ShopToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Shop shop = (Shop)value;
            return shop.Name + ", " + shop.Address;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
