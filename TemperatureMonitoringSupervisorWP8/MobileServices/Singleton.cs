using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace TemperatureMonitoringSupervisorWP8
{
    public static class MobileServices
    {
        private static MobileServiceClient __singleton;

        public async static Task<MobileServiceClient> InitializeAsync()
        {
            if (__singleton == null)
            {
                __singleton = new MobileServiceClient(
                    "https://temperaturemonitoring.azure-mobile.net",
                    "QxzRNruxksNWqGpIfuvSYVIXQkQKwL17"
                );
                var user = await AuthenticateAsync();
                // http://blogs.msdn.com/b/cdndevs/archive/2014/11/11/part-3-azure-mobile-services-what-you-need-to-know-to-get-started-with-notification-hub.aspx
                await RegisterChannelAsync(user.UserId);
            }
            return __singleton;
        }

        // http://azure.microsoft.com/en-us/documentation/articles/mobile-services-dotnet-backend-windows-phone-get-started-users/
        private static MobileServiceUser __user;

        private static async Task<MobileServiceUser> AuthenticateAsync()
        {
            while (__user == null)
            {
                string message;
                try
                {
                    __user = await __singleton
                        .LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
                    message =
                        string.Format("You are now logged in - {0}", __user.UserId);
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }
            }

            return __user;
        }

        private static async Task RegisterChannelAsync(params string[] tags)
        {
            var channel = await Windows.Networking.PushNotifications.PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            await __singleton.GetPush().RegisterNativeAsync(channel.Uri, tags);
        }

        public static void LoginComplete(Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args)
        {
            __singleton.LoginComplete(args);
        }

        public static async Task SyncAsync(Action a)
        {
            string errorString = string.Empty;
            try
            {
                await MobileServices.__singleton.SyncContext.PushAsync();
                a();
            }

            catch (MobileServicePushFailedException ex)
            {
                errorString = "Push failed because of sync errors: " +
                  ex.PushResult.Errors.Count + " errors, message: " + ex.Message;
            }
            catch (Exception ex)
            {
                errorString = "Pull failed: " + ex.Message +
                  "\n\nIf you are still in an offline scenario, " +
                  "you can try your Pull again when connected with your Mobile Serice.";
            }

            if (!string.IsNullOrWhiteSpace(errorString))
            {
                MessageDialog d = new MessageDialog(errorString);
                await d.ShowAsync();
            }
        }

        public static IMobileServiceTable<T> GetTable<T>()
        {
            return __singleton.GetTable<T>();
        }

        public static async Task<IMobileServiceSyncTable<T>> GetSyncTable<T>()
        {
            //http://azure.microsoft.com/en-us/documentation/articles/mobile-services-windows-store-dotnet-get-started-offline-data/
            if (!__singleton.SyncContext.IsInitialized)
            {
                var store = new MobileServiceSQLiteStore("temperaturemonitoring-offlinesync-6.db");
                store.DefineTable<T>();
                await __singleton.SyncContext.InitializeAsync(store);
            }
            var xxx = MobileServices.__singleton.GetSyncTable<T>(); // offline sync

            return xxx;
        }
    }
}
