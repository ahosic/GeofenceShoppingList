using System;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.Common;
using ShoppingListWPApp.ViewModels;

namespace ShoppingListWPApp.Views
{
    /// <summary>
    /// This is the corresponding View of the DetailsShopViewModel.
    /// </summary>
    public sealed partial class DetailsShop : Page
    {
        /// <summary>
        /// NavigationHelper aids in navigation between pages.
        /// </summary>
        private NavigationHelper navigationHelper;

        public DetailsShop()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            // Initialize Map styles
            MapStyles.Items.Add(ResourceLoader.GetForCurrentView().GetString("MapStyleStandard"));
            MapStyles.Items.Add(ResourceLoader.GetForCurrentView().GetString("MapStyleAerial"));
            MapStyles.Items.Add(ResourceLoader.GetForCurrentView().GetString("MapStyleAerialWithRoads"));
            MapStyles.Items.Add(ResourceLoader.GetForCurrentView().GetString("MapStyleTerrain"));
            MapStyles.Items.Add(ResourceLoader.GetForCurrentView().GetString("MapStyleRoads"));
            MapStyles.Items.Add(ResourceLoader.GetForCurrentView().GetString("MapStyleNone"));

            MapStyles.SelectedIndex = 0;
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
            // Set selected shop (selected on the MainPage) in the DetailsShopViewModel
            ServiceLocator.Current.GetInstance<DetailsShopViewModel>().SetShop((int)e.NavigationParameter);
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            try
            {
                // Clear all MapIcons of the MapControl
                Map.Children.Clear();

                // Add a MapIcon with the location of the selected Shop (selected on the MainPage) to the MapControl
                Geopoint point = new Geopoint((BasicGeoposition)ServiceLocator.Current.GetInstance<DetailsShopViewModel>().Location);

                // Create pushpin
                Ellipse pushpin = new Ellipse
                {
                    Fill = new SolidColorBrush(Color.FromArgb(255, 27, 161, 226)),
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 2,
                    Width = 20,
                    Height = 20,
                };

                // Add Pin to MapControl
                MapControl.SetLocation(pushpin, point);
                MapControl.SetNormalizedAnchorPoint(pushpin, new Point(0.5, 0.5));
                Map.Children.Add(pushpin);

                // Center the selected location on the MapControl
                Map.Center = point;
                Map.DesiredPitch = 0;
                await Map.TrySetViewAsync(point, 15);
            }
            catch (Exception ex) { }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        /// <summary>
        /// Sets the style of the MapControl on the DetailsShop-View.
        /// 
        /// This method gets invoked, when the selected item of the <c>MapStyles</c> combobox changes.
        /// </summary>
        /// <param name="sender">The combobox, for which the <c>SelectedItem</c> property has changed.</param>
        /// <param name="e">Event arguments.</param>
        private void MapStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MapStyles.SelectedIndex)
            {
                case 0:
                    Map.Style = MapStyle.Road;
                    break;
                case 1:
                    Map.Style = MapStyle.Aerial;
                    break;
                case 2:
                    Map.Style = MapStyle.AerialWithRoads;
                    break;
                case 3:
                    Map.Style = MapStyle.Terrain;
                    break;
                case 4:
                    Map.Style = MapStyle.Road;
                    break;
                case 5:
                    Map.Style = MapStyle.None;
                    break;
                default:
                    Map.Style = MapStyle.None;
                    break;
            }

            // Close Flyout
            AbtnMapStyle.Flyout.Hide();
        }
    }
}
