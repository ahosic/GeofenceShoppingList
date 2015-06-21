using System;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace ShoppingListWPApp.Converter
{
    /// <summary>
    /// The <c>RadiusToRadiusWithUnitConverter</c> is used for converting the 
    /// <c>Radius</c>-Value of a <c>Shop</c>-Object into a formatted, regional-specific string.
    /// </summary>
    class RadiusToRadiusWithUnitConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <c>Radius</c>-Value of a <c>Shop</c>-Object to a formatted, regional-specifig string.
        /// </summary>
        /// <param name="value">The <c>double</c>-Value that should be converted.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Parameters that can be passed to the Converter.</param>
        /// <param name="language">Cultural information that can be passed to the Converter.</param>
        /// <returns>Returns a formatted, regional-specific string representation of <c>value</c>.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double radius = (double)value;

            // Check, if imperial system is present
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("en"))
            {
                radius = radius * 0.62137;
            }

            return radius.ToString("0.00") + " " + ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusUnit");
        }

        /// <summary>
        /// Converts back a formatted, regional-specifig string to a <c>Radius</c>-Value of a <c>Shop</c>-Object.
        /// </summary>
        /// <param name="value">The <c>string</c>-Value that should be converted.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Parameters that can be passed to the Converter.</param>
        /// <param name="language">Cultural information that can be passed to the Converter.</param>
        /// <returns>Returns a <c>double</c>-Value.</returns>
        /// <exception cref="System.NotImplementedException">Thrown because this method is not implemented (not used).</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
