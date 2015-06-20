using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;

namespace GeofenceTask
{
    public sealed class BackgroundGeofenceTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var Reports = GeofenceMonitor.Current.ReadReports();
            var SelectedReport =
                Reports.FirstOrDefault(report => (report.Geofence.Id == "Maximarkt") && (report.NewState == GeofenceState.Entered || report.NewState == GeofenceState.Exited));

            if (SelectedReport == null)
            {
                return;
            }

            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");

            toastTextElements[0].AppendChild(toastXml.CreateTextNode("You are pretty close to your shop"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(SelectedReport.Geofence.Id));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"id\":\"" + SelectedReport.Geofence.Id.Trim() + "\"}");


            //var ToastContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            //var TextNodes = ToastContent.GetElementsByTagName("text");

            //if (SelectedReport.NewState == GeofenceState.Entered)
            //{
            //    TextNodes[0].AppendChild(ToastContent.CreateTextNode("You are pretty close to your shop"));
            //    TextNodes[1].AppendChild(ToastContent.CreateTextNode(SelectedReport.Geofence.Id));

            //}

            //var settings = ApplicationData.Current.LocalSettings;

            //if (settings.Values.ContainsKey("Status"))
            //{
            //    settings.Values["Status"] = SelectedReport.NewState;
            //}
            //else
            //{
            //    settings.Values.Add(new KeyValuePair<string, object>("Status", SelectedReport.NewState.ToString()));
            //}

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
