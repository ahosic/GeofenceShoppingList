using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using GalaSoft.MvvmLight.Views;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.Common
{
    /// <summary>
    /// Helper class, which provides functionality for Geolocation and Geofencing.
    /// </summary>
    class GeoHelper
    {
        #region *** Private members ***

        /// <summary>
        /// This <c>IDialogService</c>-Object is used for displaying Dialogs on the Device-Screen.
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// This <c>Geolocator</c>-Object is used for retrieving the geographical position of the device.
        /// </summary>
        private Geolocator locator;

        #endregion

        #region *** Properties ***

        /// <summary>
        /// This <c>Geolocator</c>-Object is used for retrieving the geographical position of the device.
        /// </summary>
        public Geolocator Locator
        {
            get
            {
                switch (locator.LocationStatus)
                {
                    case PositionStatus.Disabled:
                        dialogService.ShowMessage(
                        ResourceLoader.GetForCurrentView().GetString("GPSStatusError"),
                        ResourceLoader.GetForCurrentView().GetString("ErrorTitle"));
                        return null;
                    case PositionStatus.NotAvailable:
                        dialogService.ShowMessage(
                        ResourceLoader.GetForCurrentView().GetString("GPSStatusError"),
                        ResourceLoader.GetForCurrentView().GetString("ErrorTitle"));
                        return null;
                }

                return locator;
            }
            private set { locator = value; }
        }

        #endregion

        public GeoHelper(IDialogService dialogService)
        {
            // Initialize Services
            this.dialogService = dialogService;
            this.Locator = new Geolocator();
        }

        #region *** Geofencing ***

        /// <summary>
        /// This method creates a Geofence for a given Shoppinglist.
        /// </summary>
        /// <param name="shop">Object for which a Geofence should be created.</param>
        private Geofence CreateGeofence(Shop shop)
        {
            Geocircle circle = new Geocircle((BasicGeoposition)shop.Location, (shop.Radius * 1000));

            //Selecting a subset of the events we need to interact with the geofence
            const MonitoredGeofenceStates geoFenceStates = MonitoredGeofenceStates.Entered;

            // Setting up how long you need to be in geofence for enter event to fire
            TimeSpan dwellTime = TimeSpan.FromSeconds(1);

            // Setting up how long the geofence should be active
            TimeSpan geoFenceDuration = TimeSpan.FromSeconds(0);

            // Setting up the start time of the geofence
            DateTimeOffset geoFenceStartTime = DateTimeOffset.Now;

            // Create object
            Geofence fence = new Geofence
                (shop.ID,
                circle,
                geoFenceStates,
                false,
                dwellTime,
                geoFenceStartTime,
                geoFenceDuration);

            return fence;
        }

        /// <summary>
        /// Creates a Geofence and adds it to the <c>GeofenceMonitor</c>.
        /// </summary>
        /// <param name="shop">Object for which a Geofence should be created.</param>
        public void AddGeofence(Shop shop)
        {
            //Add the geofence to the GeofenceMonitor
            GeofenceMonitor.Current.Geofences.Add(CreateGeofence(shop));
        }

        /// <summary>
        /// Replaces a Geofence with a new one.
        /// </summary>
        /// <param name="id">ID of the old <c>Geofence</c> object.</param>
        /// <param name="newShop">The new <c>Shop</c> object that should replace the old one.</param>
        public void ModifyGeofence(string id, Shop newShop)
        {
            // Get old object out of the GeofenceMonitor
            Geofence old = (from f in GeofenceMonitor.Current.Geofences
                            where f.Id.Equals(id)
                            select f).First();

            // Get position of old Geofence in the Geofences-Collection
            int idx = GeofenceMonitor.Current.Geofences.IndexOf(old);

            // Replace old Geofence with new one
            GeofenceMonitor.Current.Geofences.Remove(old);
            GeofenceMonitor.Current.Geofences.Insert(idx, CreateGeofence(newShop));
        }

        /// <summary>
        /// Removes a predefined Geofence from GeofenceMonitor.
        /// </summary>
        /// <param name="id">ID of Geofence that should be removed.</param>
        public void RemoveGeofence(string id)
        {
            // Get Geofence from GeofenceMonitor
            var result = (from f in GeofenceMonitor.Current.Geofences
                          where f.Id.Equals(id)
                          select f).ToList();

            // Check, if Geofences have been found
            if (result.Count > 0)
            {
                Geofence fence = result.First();

                // Remove Geofence from GeofenceMonitor
                GeofenceMonitor.Current.Geofences.Remove(fence);
            }
        }

        /// <summary>
        /// Initializes Geofencing for the app.
        /// </summary>
        public async void InitGeofencing()
        {
            // Check, if Background Task is already registered.
            if (IsTaskRegistered("ShoppinglistGeofenceTask"))
            {
                // Unregister Background Task
                Unregister("ShoppinglistGeofenceTask");
            }

            // Register Background Task
            await RegisterGeofenceTask();
        }

        /// <summary>
        /// Checks, if a Background Task with a specific name is already registered.
        /// </summary>
        /// <param name="taskName">The name of the Background Task.</param>
        /// <returns>False, if the Task is not registered, true if the task is already registered.</returns>
        private bool IsTaskRegistered(string taskName)
        {
            var Registered = false;
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(keyval => keyval.Value.Name == taskName);

            if (entry.Value != null)
                Registered = true;

            return Registered;
        }

        /// <summary>
        /// Unregisters a Background Task with a specified name.
        /// </summary>
        /// <param name="taskName">The name of the Background Task.</param>
        private void Unregister(string taskName)
        {
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(keyval => keyval.Value.Name == taskName);

            if (entry.Value != null)
                entry.Value.Unregister(true);
        }

        /// <summary>
        /// Registers a Background Task that is used for reacting on Geofence-Reports.
        /// </summary>
        /// <returns>A Background Task for reacting on Geofence-Reports.</returns>
        private async Task RegisterGeofenceTask()
        {
            BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            var geofenceTaskBuilder = new BackgroundTaskBuilder
            {
                Name = "ShoppinglistGeofenceTask",
                TaskEntryPoint = "GeofenceTask.BackgroundGeofenceTask"
            };

            var trigger = new LocationTrigger(LocationTriggerType.Geofence);
            geofenceTaskBuilder.SetTrigger(trigger);

            var geofenceTask = geofenceTaskBuilder.Register();

            switch (backgroundAccessStatus)
            {
                case BackgroundAccessStatus.Unspecified:
                case BackgroundAccessStatus.Denied:
                    await dialogService.ShowMessage(
                        ResourceLoader.GetForCurrentView().GetString("RegisterBackgroundTaskError"),
                        ResourceLoader.GetForCurrentView().GetString("AccessDeniedTitle"));
                    break;
            }
        }

        #endregion
    }
}
