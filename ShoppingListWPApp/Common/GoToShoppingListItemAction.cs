using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xaml.Interactivity;
using ShoppingListWPApp.Models;
using ShoppingListWPApp.ViewModels;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
namespace ShoppingListWPApp.Common
{
    class GoToShoppingListItemAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            // Get sender of the Action
            FrameworkElement senderElement = sender as FrameworkElement;

            ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("addShoppingListItem", (ShoppingList)senderElement.DataContext);

            return null;
        }
    }
}
