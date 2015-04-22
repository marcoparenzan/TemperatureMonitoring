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

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace TemperatureMonitoringMobileService.MobileServices
{

    // http://azure.microsoft.com/en-us/documentation/articles/mobile-services-dotnet-backend-windows-store-dotnet-aad-rbac/
    public static class AadAuthorization
    {
        private const string AadInstance = "https://login.windows.net/{0}";
        private const string GraphResourceId = "https://graph.windows.net/";
        private const string APIVersion = "?api-version=2013-04-05";

        public static bool UserInRole(this TableController that, AadRole role)
        {
            return true;
        }

        public static async Task<bool> XUserInRole(this TableController that, AadRole role)
        {
            HttpConfiguration config = that.ControllerContext.Configuration;
            var services = new ApiServices(config);

            bool isAuthorized = false;
            try
            {
                // Initialize a mapping for the group id to our enumerated type
                var groupIds = InitGroupIds(services);
                services.Log.Info("Groups loaded");

                // Retrieve a AAD token from ADAL
                var token = await AcquireTokenAsync(services);
                if (token == null)
                {
                    services.Log.Error("AuthorizeAadRole: Failed to get an AAD access token.");
                }
                else
                {
                    services.Log.Info("Got token " + token.ToString());

                    // Check group membership to see if the user is part of the group that corresponds to the role
                    if (!string.IsNullOrEmpty(groupIds[(int)role]))
                    {
                        ServiceUser serviceUser = that.User as ServiceUser;
                        if (serviceUser != null && serviceUser.Level == AuthorizationLevel.User)
                        {
                            var idents = await serviceUser.GetIdentitiesAsync();
                            AzureActiveDirectoryCredentials clientAadCredentials =
                                idents.OfType<AzureActiveDirectoryCredentials>().FirstOrDefault();
                            if (clientAadCredentials != null)
                            {
                                services.Log.Info("Now checking membership");

                                isAuthorized = CheckMembership(services, clientAadCredentials.ObjectId, role, groupIds);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                services.Log.Error(e.Message);
            }
            finally
            {
                if (isAuthorized == false)
                {
                    services.Log.Info("Denying access");
                    var response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden);
                    response.Content = new StringContent("User is not logged in or not a member of the required group");
                }
            }

            return isAuthorized;
        }

        private static Dictionary<int, string> InitGroupIds(ApiServices services)
        {
            var groupIds = new Dictionary<int, string>();
            string groupId;

            if (!groupIds.ContainsKey((int)AadRole.Sensor))
            {
                if (services.Settings.TryGetValue("AAD_SENSOR_GROUP_ID", out groupId))
                {
                    groupIds.Add((int)AadRole.Sensor, groupId);
                }
                else
                    services.Log.Error("AAD_SENSOR_GROUP_ID app setting not found.");
            }
            if (!groupIds.ContainsKey((int)AadRole.Supervisor))
            {
                if (services.Settings.TryGetValue("AAD_SUPERVISOR_GROUP_ID", out groupId))
                {
                    groupIds.Add((int)AadRole.Supervisor, groupId);
                }
                else
                    services.Log.Error("AAD_SUPERVISOR_GROUP_ID app setting not found.");
            }

            return groupIds;
        }

        public static async Task<AuthenticationResult> AcquireTokenAsync(this TableController that)
        {
            return await AcquireTokenAsync(that.Services);
        }

        // Use ADAL and the authentication app settings from the Mobile Service to 
        // get an AAD access token
        public static async Task<AuthenticationResult> AcquireTokenAsync(ApiServices services)
        {
            string tenantdomain;
            string clientid;
            string clientkey;

            string token = string.Empty;

            // Try to get the required AAD authentication app settings from the mobile service.  
            if (!(services.Settings.TryGetValue("AAD_CLIENT_ID", out clientid) &
                  services.Settings.TryGetValue("AAD_CLIENT_KEY", out clientkey) &
                  services.Settings.TryGetValue("AAD_TENANT_DOMAIN", out tenantdomain)))
            {
                services.Log.Error("GetAADToken() : Could not retrieve mobile service app settings.");
                return null;
            }

            ClientCredential clientCred = new ClientCredential(clientid, clientkey);
            services.Log.Info("ClientCred: " + clientCred.ClientId);
            string authority = String.Format(CultureInfo.InvariantCulture, AadInstance, tenantdomain);
            AuthenticationContext authContext = new AuthenticationContext(authority);
            services.Log.Info("authContext: " + authContext.Authority.ToString());
            AuthenticationResult result = await authContext.AcquireTokenAsync(GraphResourceId, clientCred);
            services.Log.Info("A/R JSON: " + JsonConvert.SerializeObject(result));

            return result;
        }


        // Given an AAD user id, check membership against the group associated with the role.
        private static bool CheckMembership(ApiServices services, string memberId, AadRole role, Dictionary<int, string> groupIds)
        {
            string tenantdomain;
            string clientid;
            string clientkey;

            string token = string.Empty;

            // Try to get the required AAD authentication app settings from the mobile service.  
            if (!(services.Settings.TryGetValue("AAD_CLIENT_ID", out clientid) &
                  services.Settings.TryGetValue("AAD_CLIENT_KEY", out clientkey) &
                  services.Settings.TryGetValue("AAD_TENANT_DOMAIN", out tenantdomain)))
            {
                services.Log.Error("GetAADToken() : Could not retrieve mobile service app settings.");
                return false;
            }
            
            bool membership = false;
            string url = GraphResourceId + tenantdomain + "/isMemberOf" + APIVersion;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // Use the Graph REST API to check group membership in the AAD
            try
            {
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", token);
                using (var sw = new StreamWriter(request.GetRequestStream()))
                {
                    // Request body must have the group id and a member id to check for membership
                    string body = String.Format("\"groupId\":\"{0}\",\"memberId\":\"{1}\"",
                        groupIds[(int)role], memberId);
                    sw.Write("{" + body + "}");
                }

                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string json = sr.ReadToEnd();
                MembershipResponse membershipResponse = JsonConvert.DeserializeObject<MembershipResponse>(json);
                membership = membershipResponse.value;
            }
            catch (Exception e)
            {
                services.Log.Error("OnAuthorization() exception : " + e.Message);
            }

            return membership;
        }

        private class MembershipResponse
        {
            public bool value;
        }
    }
}