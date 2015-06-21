using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeofenceTask
{
    /// <summary>
    /// This Background Task is used for responding on Geofence events.
    /// 
    /// This Background Task gets triggered, when the device enters a Geofence that was created by the App.
    /// </summary>
    public sealed class BackgroundGeofenceTask : IBackgroundTask
    {
        /// <summary>
        /// A Collection of <c>ShoppingList</c> objects in JSON format.
        /// 
        /// This collection gets populated by the <c>Run</c> method while loading shopping list data out of the isolated storage.
        /// </summary>
        private JArray shoppingLists;

        /// <summary>
        /// Reads out current Geofence-Reports and creates corresponding Toast Notifications and displays them.
        /// 
        /// This method gets invoked, when the device enters a Geofence that was created by the App.
        /// </summary>
        /// <param name="taskInstance">The current Instance of the Background Task.</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get entered Geofences
            var Reports = GeofenceMonitor.Current.ReadReports();

            //Get all Shopping Lists
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            try
            {
                // Get App's folder and create file
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync("lists.json");

                // Load data out of file
                string data = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
                shoppingLists = (JArray)JsonConvert.DeserializeObject(data) ?? new JArray();
            }
            catch (Exception ex) { }

            // Check, if shopping lists were loaded correctly and if the list contains elements
            if (shoppingLists == null || shoppingLists.Count == 0)
            {
                return;
            }

            // Iterate through all entered Geofences
            foreach (var rep in Reports)
            {
                // Get Shop ID (Geofence ID == Shop ID)
                string id = rep.Geofence.Id;

                // Get all ShoppingLists of a shop
                IEnumerable<JToken> lists = (from list in shoppingLists
                                             where list.Value<JObject>("Shop").Value<string>("ID").Equals(id)
                                             select list);

                // Iterate through all selected lists
                foreach (JToken list in lists)
                {
                    // Get Shopping List information
                    string listID = list.Value<string>("ID");
                    string listName = list.Value<string>("ListName");

                    // Get Shop information
                    string shopName = list.Value<JObject>("Shop").Value<string>("Name");
                    string shopAddress = list.Value<JObject>("Shop").Value<string>("Address") ?? string.Empty;

                    string formattedShop;

                    // Check, if Address field is set
                    if (shopAddress.Trim().Equals(string.Empty))
                    {
                        formattedShop = shopName;
                    }
                    else
                    {
                        formattedShop = shopName + ", " + shopAddress;
                    }

                    // Create Toast template
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");

                    // Add content to Toast
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(listName));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode(formattedShop));

                    // Set parameters of Toast
                    IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                    ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"id\":\"" + listID + "\"}");

                    // Create and display Toast
                    ToastNotification toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            }

            deferral.Complete();
        }
    }
}
