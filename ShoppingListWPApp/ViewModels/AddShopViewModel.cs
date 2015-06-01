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
    class AddShopViewModel : ViewModelBase
    {
        #region *** Private Members ***

        private INavigationService navigationService;
        private IDialogService dialogService;

        #endregion

        #region *** Properties ***

        public string Name { get; set; }
        public string Address { get; set; }
        public double Radius { get; set; }
        public BasicGeoposition? Location { get; set; }

        public double MinimumRadius { get; set; }
        public double MaximumRadius { get; set; }
        public double RadiusStepValue { get; set; }
        public double TickFrequency { get; set; }

        public ICommand DoneCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        public AddShopViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = new DialogService();

            InitializeFields();

            DoneCommand = new DependentRelayCommand(CreateShop, IsDataValid, this, () => Name, () => Location);
            CancelCommand = new RelayCommand(Cancel);

            MinimumRadius = double.Parse(ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusMinimum"));
            MaximumRadius = double.Parse(ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusMaximum"));
            RadiusStepValue = double.Parse(ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusStep"));
            TickFrequency = double.Parse(ResourceLoader.GetForCurrentView().GetString("GeoFenceShopRadiusTickFrequency"));
        }

        public async void MapTapped(MapControl sender, MapInputEventArgs args)
        {
            Location = args.Location.Position;

            MapLocationFinderResult FinderResult = await MapLocationFinder.FindLocationsAtAsync(args.Location);

            if (FinderResult.Status == MapLocationFinderStatus.Success)
            {
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

        private bool IsDataValid()
        {
            if (Name.Trim().Equals(string.Empty) || Location == null)
            {
                return false;
            }

            return true;
        }

        private void CreateShop()
        {
            Shop shop = new Shop(Name, Address, Radius, Location);
            ServiceLocator.Current.GetInstance<MainPageViewModel>().AddShop(shop);

            // Reinitialize all properties and go back to the overview
            InitializeFields();
            navigationService.GoBack();
        }

        private async void Cancel()
        {
            bool result = true;

            if (!Name.Trim().Equals(string.Empty) || Location != null)
            {
                // Show a dialog
                result = await dialogService.ShowMessage(ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogText"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogTitle"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonProceed"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopCancelDialogButtonCancel"),
                    null);
            }

            if (result)
            {
                InitializeFields();
                navigationService.GoBack();
            }
        }

        private void InitializeFields()
        {
            Name = string.Empty;
            Address = string.Empty;
            Location = null;
        }
    }
}
