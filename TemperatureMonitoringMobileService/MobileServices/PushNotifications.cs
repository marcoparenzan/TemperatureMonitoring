/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Marco Parenzan (https://it.linkedin.com/in/marcoparenzan)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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