using System;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ShoppingListWPApp.Common;

namespace ShoppingListWPApp.Views
{
    public sealed partial class MainPage : Page
    {
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

        private void PvMain_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            if (args.Item.Equals(PviMap))
            {
                AbtnFindMe.Visibility = Visibility.Visible;
                GetMyLocation();
            }
            else
            {
                AbtnFindMe.Visibility = Visibility.Collapsed;
            }
        }

        private void AbtnFindMe_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GetMyLocation();
        }
    }
}
