using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace TemperatureMonitoringMobileService.MobileServices
{
    public static class PushNotifications
    {
        private static string __wnsTemplate = 
            @"<?xml version=""1.0"" encoding=""utf-8""?>" +
            @"<toast><visual><binding template=""ToastText01"">" +
            @"<text id=""1"">{0}</text>" +
            @"</binding></visual></toast>";

        public static async Task<bool> ToastAsync(this ApiController that, string payload, params string[] tags)
        {
            HttpConfiguration config = that.ControllerContext.Configuration;
            var services = new ApiServices(config);
            
            try
            {
                var wnsToast = string.Format(__wnsTemplate, payload);
                var message = new WindowsPushMessage();
                message.XmlPayload = wnsToast;
                await services.Push.SendAsync(message, tags);
                return true;
            }
            catch (Exception e)
            {
                services.Log.Error(e.ToString());
            }
            return false;
        }
    }
}