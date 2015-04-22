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
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using TemperatureMonitoringMobileService.Models;
using TemperatureMonitoringMobileService.MobileServices;

namespace TemperatureMonitoringMobileService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class TemperatureSampleController : TableController<TemperatureSample>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TemperatureMonitoringDataContext context = new TemperatureMonitoringDataContext();
            DomainManager = new EntityDomainManager<TemperatureSample>(context, Request, Services);
        }

        public IQueryable<TemperatureSample> GetAllTemperatureSamples()
        {
            var query = Query();
            if (!this.UserInRole(AadRole.Supervisor))
            {
                query = query.Where(xx => xx.Id == User.Identity.Name);
            }
            return query;
        }

        public SingleResult<TemperatureSample> GetTemperatureSample(string id)
        {
            if (this.UserInRole(AadRole.Supervisor))
            {
                return Lookup(id);
            }
            else if (User.Identity.Name == id)
            {
                return Lookup(id);
            }
            else
                throw new UnauthorizedAccessException("You are not authorized to view these data");
        }

        public Task<TemperatureSample> PatchTemperatureSample(string id, Delta<TemperatureSample> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostTemperatureSample(TemperatureSample item)
        {
            TemperatureSample current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteTemperatureSample(string id)
        {
            return DeleteAsync(id);
        }
    }
}