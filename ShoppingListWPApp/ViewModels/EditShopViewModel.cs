using System;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Mutzl.MvvmLight;
using ShoppingListWPApp.Common;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.ViewModels
{
    /// <summary>
    /// This is the ViewModel for the <c>EditShop</c>-View.
    /// </summary>
    class EditShopViewModel : ViewModelBase
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
        /// The <c>Shop</c>-Object that should be modified.
        /// </summary>
        private Shop oldShop;

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
        /// Gets or Sets the minimum value of the Radius of Notification.
        /// </summary>
        public double MinimumRadius { get; set; }
        /// <summary>
        /// Gets or Sets the maximum value of the Radius of Notification.
        /// </summary>
        public double MaximumRadius { get; set; }
        /// <summary>
        /// Gets or Sets the incrementation step value of the Radius of Notification.
        /// </summary>
        public double RadiusStepValue { get; set; }
        /// <summary>
        /// Gets or Sets the Frequency of Ticks that should be displayed on the Slider for the Radius of Notification.
        /// </summary>
        public double TickFrequency { get; set; }

        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to finish the editting-process of the selected Shop.
        /// 
        /// If the Properties <c>Name</c> and <c>Location</c> are not set, the Button for the <c>DoneCommand</c> is disabled.
        /// </summary>
        public ICommand DoneCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to cancel the editting-process of the selected Shop.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        #endregion

        public EditShopViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = new DialogService();

            // Commands
            DoneCommand = new DependentRelayCommand(Edit, IsDataValid, this, () => Name, () => Location);
            CancelCommand = new RelayCommand(Cancel);

            // Get regional-specific values for the Radius out of the Resource-File
            MinimumRadius = double.Parse(ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusMinimum"));
            MaximumRadius = double.Parse(ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusMaximum"));
            RadiusStepValue = double.Parse(ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusStep"));
            TickFrequency = double.Parse(ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusTickFrequency"));
        }

        #region *** Public methods ***

        /// <summary>
        /// Sets the shop that should be modified and assigns its values to the corresponding values.
        /// </summary>
        /// <param name="oldShop">The <c>Shop</c>-Object that should be modified.</param>
        public void SetShop(Shop oldShop)
        {
            // Set selected Shop (selected on the previous Page)
            this.oldShop = oldShop;

            // Initialize all fields with the values of the selected Shop
            Name = oldShop.Name;
            Address = oldShop.Address;
            Radius = oldShop.Radius;
            Location = oldShop.Location;
        }

        /// <summary>
        /// The <c>MapTapped</c> method gets invoked, when the user taps on the <c>MapControl</c> provided by the <c>EditShop</c>-View. 
        /// It gets the geographical position of the point, where the User tapped on the <c>MapControl</c>. With this position, 
        /// the method estimates an address and saves the gathered values to its Properties (<c>Location</c> and <c>Address</c>).
        /// 
        /// If there are no results for the address, the values for the Properties don't get set. 
        /// </summary>
        /// <param name="sender">The <c>MapControl</c> provided by the View, which was tapped by the User.</param>
        /// <param name="args">Contains the geographical position of the point, where the User tapped on the <c>MapControl</c>.</param>
        public async void MapTapped(MapControl sender, MapInputEventArgs args)
        {
            // Set tapped Location as selected location
            Location = args.Location.Position;

            // Find corresponding address of the location
            MapLocationFinderResult FinderResult = await MapLocationFinder.FindLocationsAtAsync(args.Location);

            // Check, if any address has been found
            if (FinderResult.Status == MapLocationFinderStatus.Success)
            {
                // Format and set address of the selected location
                var selectedLocation = FinderResult.Locations.First();
                string format = "{0} {1}, {2} {3}, {4}";

                Address = string.Format(format,
                    selectedLocation.Address.Street,
                    selectedLocation.Address.StreetNumber,
                    selectedLocation.Address.PostCode,
                    selectedLocation.Address.Town,
                    selectedLocation.Address.CountryCode);
            }
        }

        #endregion

        #region *** Command methods ***

        /// <summary>
        /// Checks, if all required values are inputted and valid.
        /// </summary>
        /// <returns>Returns <c>true</c> if all inputted values are valid, <c>false</c> if the provided data is invalid.</returns>
        private bool IsDataValid()
        {
            if (Name.Trim().Equals(string.Empty) || Location == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new <c>Shop</c>-Object with the modified values of the previously selected shop, 
        /// replaces the old object with the new one in the <c>Shops</c>-Collection (located in the <c>MainPageViewModel</c>) and navigates back to the previous page.
        /// </summary>
        private void Edit()
        {
            // Create new Shop object and replace old object with new one
            Shop newShop = new Shop(Guid.NewGuid().ToString(), Name.Trim(), Address, Radius, Location);
            ServiceLocator.Current.GetInstance<MainPageViewModel>().EditShop(oldShop, newShop);

            // Go back to previous Page
            navigationService.GoBack();
        }

        /// <summary>
        /// Cancels the Editting-Process of the selected <c>Shop</c>-Object and navigates back to the previous page.
        /// </summary>
        private async void Cancel()
        {
            // Show dialog
            bool result = await dialogService.ShowMessage(ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogText"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogTitle"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonProceed"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonCancel"),
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
