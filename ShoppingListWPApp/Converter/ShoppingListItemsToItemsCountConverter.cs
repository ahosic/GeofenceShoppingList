using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.Converter
{
    /// <summary>
    /// The <c>ShoppingListItemsToItemsCountConverter</c> is used for getting the Count 
    /// property of an <c>Items</c> collection of a <c>ShoppingList</c> object.
    /// </summary>
    class ShoppingListItemsToItemsCountConverter : IValueConverter
    {
        /// <summary>
        /// Gets the Count of <c>ShoppingListItem</c> objects out of the <c>Items</c> collection of a <c>ShoppingList</c> object.
        /// </summary>
        /// <param name="value">The <c>Items</c> collection for which the Count should be retrieved.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Parameters that can be passed to the Converter.</param>
        /// <param name="language">Cultural information that can be passed to the Converter.</param>
        /// <returns>Returns a string representation of the Count property of the <c>Items</c> collection.</returns>
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            ObservableCollection<ShoppingListItem> items = (ObservableCollection<ShoppingListItem>)value;
            return items.Count.ToString();
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
