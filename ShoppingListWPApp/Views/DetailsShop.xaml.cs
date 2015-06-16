using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
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
                Map.MapElements.Clear();

                // Add a MapIcon with the location of the selected Shop (selected on the MainPage) to the MapControl
                Geopoint point = new Geopoint((BasicGeoposition)ServiceLocator.Current.GetInstance<DetailsShopViewModel>().Location);
                MapIcon icon = new MapIcon { Location = point };

                // Center the selected location on the MapControl
                Map.MapElements.Add(icon);
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
    }
}
