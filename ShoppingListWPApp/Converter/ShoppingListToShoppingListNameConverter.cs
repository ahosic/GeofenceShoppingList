using Windows.UI.Xaml.Data;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.Converter
{
    /// <summary>
    /// The <c>ShoppingListToShoppingListNameConverter</c> is used for converting the 
    /// <c>Shop</c> object of a <c>ShoppingList</c> object into a formatted string representation of the object.
    /// </summary>
    class ShoppingListToShoppingListNameConverter : IValueConverter
    {
        /// <summary>
        /// Gets the <c>Shop</c> object out of a <c>ShoppingList</c> object and returns a string representation of its information.
        /// </summary>
        /// <param name="value">The <c>ShoppingList</c> object that should be converted.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Defines which information of the <c>Shop</c> object should be returned. 
        /// Use either "name" (without asterisks) as parameter for returning the name of the <c>Shop</c> object, 
        /// or "address" for returning a string representation of the <c>Shop</c> object.</param>
        /// <param name="language">Cultural information that can be passed to the Converter.</param>
        /// <returns>Returns information about the <c>Shop</c> object of a <c>ShoppingList</c> object depending on the parameter passed to the method.</returns>
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            ShoppingList list = (ShoppingList)value;

            switch ((string)parameter)
            {
                case "name":
                    return list.ListName;
                case "address":
                    return list.Shop.ToString();
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
