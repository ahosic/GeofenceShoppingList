using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.Common;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.ViewModels
{
    /// <summary>
    /// This is the ViewModel for the <c>DetailsShop</c>-View.
    /// </summary>
    class DetailsShopViewModel : ViewModelBase
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
        /// The <c>Shop</c>-Object, for which details should be displayed.
        /// </summary>
        private Shop shop;

        #endregion

        #region *** Properties ***

        /// <summary>
        /// Gets or Sets the Name of the selected Shop. 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the Address of the selected Shop.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Gets or Sets the Radius of Notification of the selected Shop.
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Gets or Sets the Geographical Position (Longitude, Latitude, Altitude) of the selected Shop.
        /// </summary>
        public BasicGeoposition? Location { get; set; }

        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to edit the selected Shop.
        /// </summary>
        public ICommand EditCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to delete the selected Shop.
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        #endregion

        public DetailsShopViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = dialogService;

            // Commands
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
        }

        #region *** Public methods ***

        /// <summary>
        /// Sets the selected Shop (Selected in the <c>MainPage</c>-View) and assigns its values to the corresponding Properties.
        /// </summary>
        /// <param name="idx">The index of the selected <c>Shop</c>-Object in the <c>Shops</c>-Collection (located in the <c>MainPageViewModel</c>)</param>
        public void SetShop(int idx)
        {
            // Set selected Shop (selected on the previous Page)
            this.shop = ServiceLocator.Current.GetInstance<MainPageViewModel>().GetShopByIndex(idx);

            // Initialize all fields with the values of the selected Shop
            Name = shop.Name;
            Address = shop.Address;
            Radius = shop.Radius;
            Location = shop.Location;
        }

        #endregion

        #region *** Command methods ***

        /// <summary>
        /// The <c>Edit</c> method navigates the user to the <c>EditShop</c>-View, in order to edit the selected shop.
        /// </summary>
        private void Edit()
        {
            navigationService.NavigateTo("editShop", this.shop);
        }

        /// <summary>
        /// The <c>Delete</c> method deletes the selected shop and navigates back to the previous page.
        /// </summary>
        private async void Delete()
        {
            // Show dialog
            bool result = await dialogService.ShowMessage(
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogContent"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogTitle"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogButtonYes"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogButtonNo"),
                null);


            // Check, if user pressed the "Proceed-Button"
            if (result)
            {
                // Delete Shop object and go back to the previous page
                ServiceLocator.Current.GetInstance<MainPageViewModel>().DeleteShop(this.shop);
                navigationService.GoBack();
            }
        }

        #endregion
    }
}
