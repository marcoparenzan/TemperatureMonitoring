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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TemperatureMonitoringMobileService.MobileServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeAadRole : AuthorizationFilterAttribute
    {
        private bool isInitialized;
        private bool isHosted;
        private ApiServices services = null;

        // Constants used with ADAL and the Graph REST API for AAD
        private const string AadInstance = "https://login.windows.net/{0}";
        private const string GraphResourceId = "https://graph.windows.net/";
        private const string APIVersion = "?api-version=2013-04-05";

        // App settings pulled from the Mobile Service
        private string tenantdomain;
        private string clientid;
        private string clientkey;
        private Dictionary<int, string> groupIds = new Dictionary<int, string>();

        private string token = null;

        public AuthorizeAadRole(AadRole role)
        {
            this.Role = role;
        }

        // private class used to serialize the Graph REST API web response
        private class MembershipResponse
        {
            public bool value;
        }

        public AadRole Role { get; private set; }

        // Generate a local dictionary for the role group ids configured as 
        // Mobile Service app settings
        private void InitGroupIds()
        {
            string groupId;

            if (services == null)
                return;

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
        }

        // Use ADAL and the authentication app settings from the Mobile Service to 
        // get an AAD access token
        private string GetAADToken()
        {
            // Try to get the required AAD authentication app settings from the mobile service.  
            if (!(services.Settings.TryGetValue("AAD_CLIENT_ID", out clientid) &
                  services.Settings.TryGetValue("AAD_CLIENT_KEY", out clientkey) &
                  services.Settings.TryGetValue("AAD_TENANT_DOMAIN", out tenantdomain)))
            {
                services.Log.Error("GetAADToken() : Could not retrieve mobile service app settings.");
                return null;
            }

            ClientCredential clientCred = new ClientCredential(clientid, clientkey);
            string authority = String.Format(CultureInfo.InvariantCulture, AadInstance, tenantdomain);
            AuthenticationContext authContext = new AuthenticationContext(authority);
            AuthenticationResult result = authContext.AcquireTokenAsync(GraphResourceId, clientCred).Result;

            if (result != null)
                token = result.AccessToken;
            else
                services.Log.Error("GetAADToken() : Failed to return a token.");

            return token;
        }

        // Given an AAD user id, check membership against the group associated with the role.
        private bool CheckMembership(string memberId)
        {
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
                        groupIds[(int)Role], memberId);
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

        // Called when the user is attempting authorization
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            services = new ApiServices(actionContext.ControllerContext.Configuration);

            // Check whether we are running in a mode where local host access is allowed 
            // through without authentication.
            if (!this.isInitialized)
            {
                HttpConfiguration config = actionContext.ControllerContext.Configuration;
                this.isHosted = config.GetIsHosted();
                this.isInitialized = true;
            }

            // No security when hosted locally
            if (!this.isHosted && actionContext.RequestContext.IsLocal)
            {
                services.Log.Warn("AuthorizeAadRole: Local Hosting.");
                return;
            }

            ApiController controller = actionContext.ControllerContext.Controller as ApiController;
            if (controller == null)
            {
                services.Log.Error("AuthorizeAadRole: No ApiController.");
            }

            bool isAuthorized = false;
            try
            {
                // Initialize a mapping for the group id to our enumerated type
                InitGroupIds();

                // Retrieve a AAD token from ADAL
                GetAADToken();
                if (token == null)
                {
                    services.Log.Error("AuthorizeAadRole: Failed to get an AAD access token.");
                }
                else
                {
                    // Check group membership to see if the user is part of the group that corresponds to the role
                    if (!string.IsNullOrEmpty(groupIds[(int)Role]))
                    {
                        ServiceUser serviceUser = controller.User as ServiceUser;
                        if (serviceUser != null && serviceUser.Level == AuthorizationLevel.User)
                        {
                            var idents = serviceUser.GetIdentitiesAsync().Result;
                            AzureActiveDirectoryCredentials clientAadCredentials =
                                idents.OfType<AzureActiveDirectoryCredentials>().FirstOrDefault();
                            if (clientAadCredentials != null)
                            {
                                isAuthorized = CheckMembership(clientAadCredentials.ObjectId);
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
                    var response =new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden);
                    response.Content = new StringContent("User is not logged in or not a member of the required group");
                }
            }
        }
    }
}