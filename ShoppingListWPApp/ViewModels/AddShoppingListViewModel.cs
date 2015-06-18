using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Mutzl.MvvmLight;
using ShoppingListWPApp.Models;
using Microsoft.Practices.ServiceLocation;
using Windows.ApplicationModel.Resources;

namespace ShoppingListWPApp.ViewModels
{
    /// <summary>
    /// This is the ViewModel for the <c>AddShoppingList</c>-View.
    /// </summary>
    class AddShoppingListViewModel : ViewModelBase
    {
        #region *** private Members ***
        /// <summary>
        /// This <c>INavigationService</c>-Object is used for navigating between pages.
        /// </summary>
        private INavigationService navigationService;
        /// <summary>
        /// This <c>IDialogService</c>-Object is used for displaying Dialogs on the Device-Screen.
        /// </summary>
        private IDialogService dialogService;
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
        public ICommand CreateShoppingListCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to cancel the creation-process of a new ShoppingList.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        #endregion

        public AddShoppingListViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = dialogService;

            //Commands
            CreateShoppingListCommand = new DependentRelayCommand(CreateShoppingList, isDataValid, this, () => ListName, () => SelectedShop);
            CancelCommand = new RelayCommand(Cancel);

            // Initialize all Fields with standard values
            InitializeFields();

            // Gets the Shops-Collection from the MainPageViewModel
            Shops = ServiceLocator.Current.GetInstance<MainPageViewModel>().Shops;

            showDialogShop();
        }

        #region *** command methods ***

        /// <summary>
        /// Creates a new <c>ShoppingList</c>-Object, adds it to the <c>ShoppingLists</c>-Collection (located in the <c>MainPageViewModel</c>).
        /// </summary>
        public void CreateShoppingList()
        {
            ShoppingList shList = new ShoppingList(ListName,SelectedShop);
            ServiceLocator.Current.GetInstance<MainPageViewModel>().AddShoppingList(shList);

            InitializeFields();

            navigationService.GoBack();
        }

        /// <summary>
        /// Cancels the Creation-Process of a new <c>ShoppingList-Object</c> and navigates back to the previous page.
        /// </summary>
        public async void Cancel()
        {
            bool result = true;

            if (!string.IsNullOrWhiteSpace(ListName) || SelectedShop != null)
            {
                // Show dialog
                result = await dialogService.ShowMessage(
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogText"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogTitle"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonProceed"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonCancel"),
                    null);
            }
            // Check if user pressed the "Proceed-Button"
            if(result)
            {
                navigationService.GoBack();
            }
            
        }

        /// <summary>
        /// Checks, if all required values are inputted and valid.
        /// </summary>
        /// <returns>Returns <c>true</c> if all inputted values are valid, <c>false</c> if the provided data is invalid.</returns>
        public bool isDataValid()
        {
            return !string.IsNullOrWhiteSpace(ListName) && SelectedShop != null;
        }

        /// <summary>
        /// Checks, if Shops-Collection has items
        /// </summary>
        public async void showDialogShop()
        {
            if (Shops.Count == 0)
            {
                bool result = true;

                // Show dialog
                result = await dialogService.ShowMessage(
                    ResourceLoader.GetForCurrentView().GetString("AddShopDialogContent"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopDialogTitle"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopDialogButtonYes"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopDialogButtonNo"),
                    null);

                // Check, if user pressed the "Yes-Button"
                if (result)
                {   
                    navigationService.NavigateTo("addShop");
                }else
                {
                    navigationService.NavigateTo("main");
                }
            }
        }

        #endregion

        #region *** private methods ***

        /// <summary>
        /// Initializes all Fields with standard values.
        /// </summary>
        private void InitializeFields()
        {
            ListName = string.Empty;
        }

        #endregion
    }
}
