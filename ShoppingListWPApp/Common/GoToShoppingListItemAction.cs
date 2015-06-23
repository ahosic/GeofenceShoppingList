using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xaml.Interactivity;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.Common
{
    /// <summary>
    /// The class <c>GoToShoppingListItemAction</c> defines an Action that happens, if an Item in the 
    /// ShoppingLists-<c>ListView</c> on the <c>MainPage</c>-View gets tapped by the user.
    /// </summary>
    class GoToShoppingListItemAction : DependencyObject, IAction
    {
        // <summary>
        /// Gets the Index of the <c>ShoppingList</c>-Object of the tapped List-Item in the 
        /// ShoppingLists-<c>ListView</c> and navigates with this information to the <c>AddShoppingList</c>-View.
        /// </summary>
        /// <param name="sender">The Sender of the <c>Tapped</c>-Event (a List-Item in the <c>ListView</c>)</param>
        /// <param name="parameter">The parameters that the sender sends to the <c>Execute</c>-Method (specified by the caller)</param>
        /// <returns><c>null</c></returns>
        public object Execute(object sender, object parameter)
        {
            // Get sender of the Action
            FrameworkElement senderElement = sender as FrameworkElement;

            ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("addShoppingListItem", (ShoppingList)senderElement.DataContext);

            return null;
        }
    }
}
