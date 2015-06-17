using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xaml.Interactivity;
using ShoppingListWPApp.Models;
using ShoppingListWPApp.ViewModels;

namespace ShoppingListWPApp.Common
{
    /// <summary>
    /// The class <c>OpenMenuFlyoutAction</c> defines an Action that happens, if an Item in the 
    /// Shops-<c>ListView</c> on the <c>MainPage</c>-View gets holded by the user.
    /// </summary>
    class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        /// <summary>
        /// Gets the <c>Shop</c>-Object of the holded List-Item in the 
        /// Shops-<c>ListView</c> and sets this <c>Shop</c>-Object as the <c>SelectedShop</c>-Object in the <c>MainPageViewModel</c>.
        /// </summary>
        /// <param name="sender">The Sender of the <c>Holding</c>-Event (a List-Item in the <c>ListView</c>)</param>
        /// <param name="parameter">The parameters that the sender sends to the <c>Execute</c>-Method (specified by the caller)</param>
        /// <returns><c>null</c></returns>
        public object Execute(object sender, object parameter)
        {
            // Get sender of the Action
            FrameworkElement senderElement = sender as FrameworkElement;

            if(senderElement.DataContext.GetType() == typeof(Shop))
            {
                // Set the in the ListView currently selected Shop in the MainViewModel
                ServiceLocator.Current.GetInstance<MainPageViewModel>().SelectedShop = (Shop)senderElement.DataContext;
            }else
            {
                // Set the in the ListView currently selected ShoppingList in the MainViewModel
                ServiceLocator.Current.GetInstance<MainPageViewModel>().SelectedShoppingList = (ShoppingList)senderElement.DataContext;

            }

            // Show the MenuFlyout
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);

            return null;
        }
    }
}
