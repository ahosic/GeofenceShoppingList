using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.Models;

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

            // Initialize all Fields with standard values
            InitializeFields();

            //Commands
            CreateShoppingListCommand = new RelayCommand(CreateShoppingList);
            CancelCommand = new RelayCommand(Cancel);

            // Gets the Shops-Collection from the MainPageViewModel
            Shops = ServiceLocator.Current.GetInstance<MainPageViewModel>().Shops;

        }

        #region *** command methods ***

        /// <summary>
        /// Creates a new <c>ShoppingList</c>-Object, adds it to the <c>ShoppingLists</c>-Collection (located in the <c>MainPageViewModel</c>).
        /// </summary>
        public async void CreateShoppingList()
        {
            if (IsDataValid())
            {
                ShoppingList shList = new ShoppingList(Guid.NewGuid().ToString(), ListName.Trim(), SelectedShop);
                ServiceLocator.Current.GetInstance<MainPageViewModel>().AddShoppingList(shList);

                InitializeFields();

                navigationService.GoBack();
            }
            else
            {
                await dialogService.ShowMessage(
                        ResourceLoader.GetForCurrentView().GetString("AddShoppingListValidationErrorContent"),
                        ResourceLoader.GetForCurrentView().GetString("ErrorTitle"));
            }
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
                    ResourceLoader.GetForCurrentView().GetString("WarningTitle"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonProceed"),
                    ResourceLoader.GetForCurrentView().GetString("CancelText"),
                    null);
            }
            // Check if user pressed the "Proceed-Button"
            if (result)
            {
                navigationService.GoBack();
            }

        }

        /// <summary>
        /// Checks, if all required values are inputted and valid.
        /// </summary>
        /// <returns>Returns <c>true</c> if all inputted values are valid, <c>false</c> if the provided data is invalid.</returns>
        public bool IsDataValid()
        {
            if (ListName.Trim().Equals(string.Empty) || SelectedShop == null)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region *** private methods ***

        /// <summary>
        /// Initializes all Fields with standard values.
        /// </summary>
        private void InitializeFields()
        {
            ListName = string.Empty;
            SelectedShop = null;
        }

        #endregion
    }
}
