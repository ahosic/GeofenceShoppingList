using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ShoppingListWPApp.Converter
{
    /// <summary>
    /// The <c>ShoppingListsToVisibilityConverter</c> is used for getting the Visibility status 
    /// based on the Count value of a <c>ItemCollection</c> object.
    /// </summary>
    class ShoppingListsToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets the Count value of a <c>ItemCollection</c> object and returns the Visibility Status based on this value.
        /// </summary>
        /// <param name="value">The <c>ItemCollection</c> object for which a Visiblity status should be returned.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Parameters that can be passed to the Converter.</param>
        /// <param name="language">Cultural information that can be passed to the Converter.</param>
        /// <returns>Visibility.Collapsed, if the <c>ItemCollection</c> object is empty, otherwise Visibility.Visible</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ItemCollection col = (ItemCollection)value;

            // Check if Collection is empty
            if (col.Count == 0)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
