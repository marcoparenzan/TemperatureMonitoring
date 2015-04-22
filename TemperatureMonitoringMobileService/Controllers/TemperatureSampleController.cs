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