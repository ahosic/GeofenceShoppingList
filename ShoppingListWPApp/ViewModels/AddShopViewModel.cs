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
    /// This is the ViewModel for the <c>AddShop</c>-View.
    /// </summary>
    class AddShopViewModel : ViewModelBase
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

        #endregion

        #region *** Properties ***

        /// <summary>
        /// Gets or Sets the Name of the new Shop.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the Address of the new Shop.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Gets or Sets the Radius of Notification of the new Shop.
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Gets or Sets the Geographical Position (Longitude, Latitude, Altitude) of the new Shop.
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
        /// Gets or Sets the Command that is issued by the user, in order to finish the creation-process of a new Shop.
        /// 
        /// If the Properties <c>Name</c> and <c>Location</c> are not set, the Button for the <c>DoneCommand</c> is disabled.
        /// </summary>
        public ICommand DoneCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to cancel the creation-process of a new Shop.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        #endregion

        public AddShopViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = new DialogService();

            // Initialize all Fields with standard values
            InitializeFields();

            // Commands
            DoneCommand = new DependentRelayCommand(CreateShop, IsDataValid, this, () => Name, () => Location);
            CancelCommand = new RelayCommand(Cancel);

            // Get regional-specific values for the Radius out of the Resource-File
            MinimumRadius = 0.05;
            MaximumRadius = 1;
            RadiusStepValue = 0.05;
            TickFrequency = 0.2;
        }

        #region *** Public methods ***

        /// <summary>
        /// The <c>MapTapped</c> method gets invoked, when the user taps on the <c>MapControl</c> provided by the <c>AddShop</c>-View. 
        /// It gets the geographical position of the point, where the User tapped on the <c>MapControl</c>. With this position, 
        /// the method estimates an address and saves the gathered values to its Properties (<c>Location</c> and <c>Address</c>).
        /// 
        /// If there are no results for the address, the values for the Properties don't get set. 
        /// </summary>
        /// <param name="sender">The <c>MapControl</c> provided by the View, which was tapped by the User.</param>
        /// <param name="args">Contains the geographical position of the point, where the User tapped on the <c>MapControl</c>.</param>
        public async void MapTapped(MapControl sender, MapInputEventArgs args)
        {
            try
            {
                // Set tapped Location as selected location
                Location = args.Location.Position;

                // Find corresponding address of the location
                MapLocationFinderResult FinderResult = await MapLocationFinder.FindLocationsAtAsync(args.Location);

                // Check, if any address has been found
                if (FinderResult.Status == MapLocationFinderStatus.Success && FinderResult.Locations.Count > 0)
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
            catch (Exception ex)
            {
                Address = string.Empty;
                dialogService.ShowMessage(
                    ResourceLoader.GetForCurrentView().GetString("FindAddressError"),
                    ResourceLoader.GetForCurrentView().GetString("ErrorTitle"));
            }
        }

        #endregion

        #region *** Private methods ***

        /// <summary>
        /// Initializes all Fields with standard values.
        /// </summary>
        private void InitializeFields()
        {
            Name = string.Empty;
            Address = string.Empty;
            Location = null;
        }

        #endregion

        #region *** Command methods ***

        /// <summary>
        /// Checks, if all required values are set and valid.
        /// </summary>
        /// <returns>Returns <c>true</c> if all set values are valid, <c>false</c> if the provided data is invalid.</returns>
        private bool IsDataValid()
        {
            if (Name.Trim().Equals(string.Empty) || Location == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new <c>Shop</c>-Object, adds it to the <c>Shops</c>-Collection (located in the <c>MainPageViewModel</c>) and navigates back to the previous page.
        /// </summary>
        private void CreateShop()
        {
            // Create Shop object and save it
            Shop shop = new Shop(Guid.NewGuid().ToString(), Name.Trim(), Address, Radius, Location);
            ServiceLocator.Current.GetInstance<MainPageViewModel>().AddShop(shop);

            // Reinitialize all properties and go back to previous Page
            InitializeFields();
            navigationService.GoBack();
        }

        /// <summary>
        /// Cancels the Creation-Process of a new <c>Shop-Object</c> and navigates back to the previous page.
        /// </summary>
        private async void Cancel()
        {
            bool result = true;

            if (!Name.Trim().Equals(string.Empty) || Location != null)
            {
                // Show dialog
                result = await dialogService.ShowMessage(ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogText"),
                    ResourceLoader.GetForCurrentView().GetString("WarningTitle"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonProceed"),
                    ResourceLoader.GetForCurrentView().GetString("CancelText"),
                    null);
            }

            // Check, if user pressed the "Proceed-Button"
            if (result)
            {
                // Reinitialize all fields and go back to the previous page
                InitializeFields();
                navigationService.GoBack();
            }
        }

        #endregion
    }
}
