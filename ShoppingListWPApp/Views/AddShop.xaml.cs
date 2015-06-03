using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.Common;
using ShoppingListWPApp.ViewModels;

namespace ShoppingListWPApp.Views
{
    public sealed partial class AddShop : Page
    {
        private NavigationHelper navigationHelper;

        public AddShop()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            Map.MapTapped += ServiceLocator.Current.GetInstance<AddShopViewModel>().MapTapped;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            GetMyLocation();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            try
            {
                Map.MapElements.Clear();

                MapIcon icon = new MapIcon();
                icon.Location = args.Location;

                Map.MapElements.Add(icon);
            }
            catch (Exception ex) { }

        }

        private void AbtnFindMe_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GetMyLocation();
        }

        private async void BtnFind_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            FindAddress();
            AbtnFind.Flyout.Hide();
        }

        private void BtnCancel_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AbtnFind.Flyout.Hide();
        }

        private void TbxAddress_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key.Equals(VirtualKey.Enter))
            {
                FindAddress();
                InputPane.GetForCurrentView().TryHide();
                AbtnFind.Flyout.Hide();
            }
        }

        private async void GetMyLocation()
        {
            try
            {
                App.ToggleProgressBar(true, ResourceLoader.GetForCurrentView().GetString("StatusBarGettingLocationText"));

                Geolocator locator = new Geolocator();
                Geoposition position = await locator.GetGeopositionAsync();

                App.ToggleProgressBar(false, null);

                Map.Center = position.Coordinate.Point;
                Map.DesiredPitch = 0;

                await Map.TrySetViewAsync(position.Coordinate.Point, 15);
            }
            catch (Exception ex)
            {
                App.ToggleProgressBar(false, null);
            }
        }

        private async void FindAddress()
        {
            try
            {
                App.ToggleProgressBar(true, ResourceLoader.GetForCurrentView().GetString("StatusBarGettingLocationText"));
                Geoposition position = await ServiceLocator.Current.GetInstance<Geolocator>().GetGeopositionAsync();
                App.ToggleProgressBar(false, null);

                App.ToggleProgressBar(true, ResourceLoader.GetForCurrentView().GetString("StatusBarSearchingAddress"));
                MapLocationFinderResult FinderResult = await MapLocationFinder.FindLocationsAsync(TbxAddress.Text, position.Coordinate.Point);
                App.ToggleProgressBar(false, null);

                if (FinderResult.Status == MapLocationFinderStatus.Success && FinderResult.Locations.Count > 0)
                {
                    ServiceLocator.Current.GetInstance<AddShopViewModel>().Location =
                        FinderResult.Locations.First().Point.Position;

                    var selectedLocation = FinderResult.Locations.First();
                    string format = "{0} {1}, {2} {3}, {4}";

                    ServiceLocator.Current.GetInstance<AddShopViewModel>().Address = string.Format(format,
                        selectedLocation.Address.Street,
                        selectedLocation.Address.StreetNumber,
                        selectedLocation.Address.PostCode,
                        selectedLocation.Address.Town,
                        selectedLocation.Address.CountryCode);

                    Map.MapElements.Clear();

                    MapIcon icon = new MapIcon { Location = FinderResult.Locations.First().Point };

                    Map.MapElements.Add(icon);

                    Map.Center = FinderResult.Locations.First().Point;
                    Map.DesiredPitch = 0;

                    await Map.TrySetViewAsync(FinderResult.Locations.First().Point, 15);
                }
                else
                {
                    await new MessageDialog(ResourceLoader.GetForCurrentView().GetString("AddShopFindAddressDialogNotFoundText"),
                        ResourceLoader.GetForCurrentView().GetString("AddShopFindAddressDialogNotFoundTitle")).ShowAsync();
                }

            }
            catch (Exception ex) { }
        }
    }
}
