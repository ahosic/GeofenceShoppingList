using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.Common;
using ShoppingListWPApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Mutzl.MvvmLight;

namespace ShoppingListWPApp.ViewModels
{
    /// <summary>
    /// This is the ViewModel for the <c>EditShoppingList</c>-View.
    /// </summary>
    class EditShoppingListViewModel : ViewModelBase
    {
        #region *** Private Members ***

        /// <summary>
        /// This <c>INavigationService</c>-Object is used for navigating between pages.
        /// </summary>
        private INavigationService navigationService;
        /// <summary>
        /// This <c>IDialogService</c>-Object is used for displaying Dialogs on the Device-Screen.
        /// </summary>
        private IDialogService dialogService;
        /// <summary>
        /// The <c>ShoppingList</c>-Object that should be modified.
        /// </summary>
        private ShoppingList oldShoppingList;

        #endregion

        #region *** Properties ***
        /// <summary>
        /// Gets or Sets the ListName of the new ShoppingListItem.
        /// </summary>
        public string ListName { get; set; }
        /// <summary>
        /// The currently selected <c>Shop</c>-Object in the Shops-<c>ListView</c> of the <c>MainPage</c>-View.
        /// </summary>
        public Shop SelectedShop { get; set; }
        /// <summary>
        /// Holds all <c>Shop</c>-Objects and notifies Views through Data Binding when Changes occur.
        /// </summary>
        public ObservableCollection<Shop> Shops { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to finish the creation-process of a new ShoppingList.
        /// </summary>
        public ICommand EditShoppingListCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to cancel the creation-process of a new ShoppingList.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        #endregion

        public EditShoppingListViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            //Service
            this.navigationService = navigationService;
            this.dialogService = dialogService;

            //Commands
            EditShoppingListCommand = new DependentRelayCommand(Edit, IsDataValid, this, () => ListName, () => SelectedShop);
            CancelCommand = new RelayCommand(Cancel);

            // Set Shops
            Shops = ServiceLocator.Current.GetInstance<MainPageViewModel>().Shops;
        }
        
        #region *** Public methods ***

        /// <summary>
        /// Sets the shoppinglist that should be modified and assigns its values to the corresponding values.
        /// </summary>
        /// <param name="oldShoppingList">The <c>ShoppingList</c>-Object that should be modified.</param>
        public void SetShoppingList(ShoppingList oldShoppingList)
        {
            // Set selected ShoppingList (selected on the previous Page)
            this.oldShoppingList = oldShoppingList;

            // Initialize all fields with the values of the selected Shoppinglist
            ListName = oldShoppingList.ListName;

        }

        #endregion

        #region *** Command methods ***

        /// <summary>
        /// Checks, if all required values are inputted and valid.
        /// </summary>
        /// <returns>Returns <c>true</c> if all inputted values are valid, <c>false</c> if the provided data is invalid.</returns>
        private bool IsDataValid()
        {
            return !string.IsNullOrWhiteSpace(ListName) && SelectedShop != null;
        }

        /// <summary>
        /// Creates a new <c>ShoppingList</c>-Object with the modified values of the previously selected shoppingList, 
        /// replaces the old object with the new one in the <c>Shops</c>-Collection (located in the <c>MainPageViewModel</c>) and navigates back to the previous page.
        /// </summary>
        private void Edit()
        {
            // Create new Shop object and replace old object with new one
            ShoppingList newShoppingList = new ShoppingList(Guid.NewGuid().ToString(), ListName.Trim(), SelectedShop);
            ServiceLocator.Current.GetInstance<MainPageViewModel>().EditShoppingList(oldShoppingList, newShoppingList);

            // Go back to previous Page
            navigationService.GoBack();
        }

        /// <summary>
        /// Cancels the Editting-Process of the selected <c>ShoppingList</c>-Object and navigates back to the previous page.
        /// </summary>
        private async void Cancel()
        {
            // Show dialog
            bool result = await dialogService.ShowMessage(ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogText"),
                    ResourceLoader.GetForCurrentView().GetString("WarningTitle"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonProceed"),
                    ResourceLoader.GetForCurrentView().GetString("CancelText"),
                    null);

            // Check, if user pressed the "Proceed-Button"
            if (result)
            {
                // Go back to the previous page
                navigationService.GoBack();
            }
        }

        #endregion
    }
}
