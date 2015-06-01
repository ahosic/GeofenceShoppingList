using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace ShoppingListWPApp.Converter
{
    class RadiusToRadiusWithUnitConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            return ((double)value).ToString("0.00") + " " + ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusUnit");
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
