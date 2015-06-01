using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xaml.Interactivity;
using ShoppingListWPApp.Models;
using ShoppingListWPApp.ViewModels;

namespace ShoppingListWPApp.Common
{
    class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            // Get sender of the Action
            FrameworkElement senderElement = sender as FrameworkElement;

            // Set the in the ListView currently selected Note in the MainViewModel
            ServiceLocator.Current.GetInstance<MainPageViewModel>().SelectedShop = (Shop)senderElement.DataContext;

            // Show the MenuFlyout
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);

            return null;
        }
    }
}
