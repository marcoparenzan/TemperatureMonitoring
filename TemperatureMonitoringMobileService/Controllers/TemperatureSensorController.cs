using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using TemperatureMonitoringMobileService.Models;
using System.Text;
using System;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.WindowsAzure.Mobile.Service;
using TemperatureMonitoringMobileService.MobileServices;

namespace TemperatureMonitoringMobileService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class TemperatureSensorController : TableController<TemperatureSensor>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TemperatureMonitoringDataContext context = new TemperatureMonitoringDataContext();
            DomainManager = new EntityDomainManager<TemperatureSensor>(context, Request, Services);
        }

        public IQueryable<TemperatureSensor> GetAllTemperatureSensors()
        {
            var query = Query();
            if (!this.UserInRole(AadRole.Supervisor))
            {
                query = query.Where(xx => xx.Id == User.Identity.Name);
            }
            return query;
        }

        public SingleResult<TemperatureSensor> GetTemperatureSensor(string id)
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

        public Task<TemperatureSensor> PatchTemperatureSensor(string id, Delta<TemperatureSensor> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostTemperatureSensor(TemperatureSensor item)
        {
            TemperatureSensor current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteTemperatureSensor(string id)
        {
            return DeleteAsync(id);
        }
    }
}