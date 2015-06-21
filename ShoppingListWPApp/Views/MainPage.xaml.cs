using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.Common;

namespace ShoppingListWPApp.Views
{
    /// <summary>
    /// This is the corresponding View of the MainPageViewModel.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// NavigationHelper aids in navigation between pages.
        /// </summary>
        private NavigationHelper navigationHelper;

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region *** Event methods ***

        /// <summary>
        /// Sets the Visibiity of the AppbarButtons that are specific for a <c>PivotItem</c>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PvMain_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            // Check, if the Map-PivoItem is loaded
            if (args.Item.Equals(PviMap))
            {
                AbtnFindMe.Visibility = Visibility.Visible;
                AbtnFind.Visibility = Visibility.Visible;
                GetMyLocation();
            }
            else
            {
                AbtnFindMe.Visibility = Visibility.Collapsed;
                AbtnFind.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// This event gets fired, when the <c>AppbarButton</c> for device location gets tapped.
        /// 
        /// This event gets the current geographical position of the device and centers this position in the <c>MapControl</c>.
        /// </summary>
        /// <param name="sender">The <c>AppbarButton</c> that has been tapped by the user.</param>
        /// <param name="e">Event arguments</param>
        /// <seealso cref="GetMyLocation()"/>
        private void AbtnFindMe_Click(object sender, RoutedEventArgs e)
        {
            GetMyLocation();
        }

        /// <summary>
        /// This event gets fired, when the Find-<c>Button</c>  in the Flyout for finding addresses gets tapped. 
        /// This event executes the <c>FindAddress</c> method.
        /// </summary>
        /// <param name="sender">The <c>Button</c> that has been tapped by the user.</param>
        /// <param name="e">Event arguments</param>
        /// <seealso cref="FindAddress(string)"/>
        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {

            FindAddress(TbxAddress.Text);
            AbtnFind.Flyout.Hide();
        }

        /// <summary>
        /// This event gets fired, when the Cancel-<c>Button</c>  in the Flyout for finding addresses gets tapped.
        /// This event hides the Flyout for finding addresses.
        /// </summary>
        /// <param name="sender">The <c>Button</c> that has been tapped by the user.</param>
        /// <param name="e">Event arguments</param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            AbtnFind.Flyout.Hide();
        }

        /// <summary>
        /// This method hides the soft-keyboard after pressing the Enter-Button in the sof-keyboard and executes the <c>FindAddress</c> method.
        /// </summary>
        /// <param name="sender">The sender of the <c>KeyDown</c> event.</param>
        /// <param name="e">Contains the key that has been pressed by the user.</param>
        /// <seealso cref="FindAddress(string)"/>
        private void TbxAddress_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Check, if the Enter-Button has been pressed
            if (e.Key.Equals(VirtualKey.Enter))
            {
                FindAddress(TbxAddress.Text);
                InputPane.GetForCurrentView().TryHide();
                AbtnFind.Flyout.Hide();
            }
        }

        #endregion

        #region *** Private methods ***

        /// <summary>
        /// Gets the current location of the device and centers the current position on the <c>MapControl</c>.
        /// </summary>
        private async void GetMyLocation()
        {
            try
            {
                // Getting current position of the device
                App.ToggleProgressBar(true, ResourceLoader.GetForCurrentView().GetString("StatusBarGettingLocationText"));
                Geoposition position = await ServiceLocator.Current.GetInstance<Geolocator>().GetGeopositionAsync();
                App.ToggleProgressBar(false, null);

                // Center current position of the device on the MapControl
                Map.Center = position.Coordinate.Point;
                Map.DesiredPitch = 0;
                await Map.TrySetViewAsync(position.Coordinate.Point, 15);
            }
            catch (Exception ex)
            {
                App.ToggleProgressBar(false, null);
            }
        }

        /// <summary>
        /// This method tries to find the geographical position of a given address.
        /// </summary>
        /// <param name="address">Address for which the geographical position should be found.</param>
        private async void FindAddress(string address)
        {
            try
            {
                // Getting current location of device
                App.ToggleProgressBar(true, ResourceLoader.GetForCurrentView().GetString("StatusBarGettingLocationText"));
                Geoposition position = await ServiceLocator.Current.GetInstance<Geolocator>().GetGeopositionAsync();
                App.ToggleProgressBar(false, null);

                // Finding geographical position of a given address
                App.ToggleProgressBar(true, ResourceLoader.GetForCurrentView().GetString("StatusBarSearchingAddress"));
                MapLocationFinderResult FinderResult = await MapLocationFinder.FindLocationsAsync(address, position.Coordinate.Point);
                App.ToggleProgressBar(false, null);

                // Check, if any positions have been found
                if (FinderResult.Status == MapLocationFinderStatus.Success && FinderResult.Locations.Count > 0)
                {
                    // Center found location on the MapControl
                    Map.Center = FinderResult.Locations.First().Point;
                    Map.DesiredPitch = 0;

                    await Map.TrySetViewAsync(FinderResult.Locations.First().Point, 15);
                }
                else
                {
                    await new MessageDialog(
                        ResourceLoader.GetForCurrentView().GetString("AddShopFindAddressDialogNotFoundText"),
                        ResourceLoader.GetForCurrentView().GetString("AddShopFindAddressDialogNotFoundTitle")).ShowAsync();
                }

            }
            catch (Exception ex) { }
        }

        #endregion
    }
}
