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
    // http://azure.microsoft.com/en-us/documentation/articles/mobile-services-dotnet-backend-service-side-authorization/
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class MonitoringCommandController : TableController<MonitoringCommand>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TemperatureMonitoringDataContext context = new TemperatureMonitoringDataContext();
            DomainManager = new EntityDomainManager<MonitoringCommand>(context, Request, Services);
        }

        public IQueryable<MonitoringCommand> GetAllMonitoringCommands()
        {
            var query = Query();
            if (!this.UserInRole(AadRole.Supervisor))
            {
                query = query.Where(xx => xx.Id == User.Identity.Name);
            }
            return Query();
        }

        public SingleResult<MonitoringCommand> GetMonitoringCommand(string id)
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

        public Task<MonitoringCommand> PatchMonitoringCommand(string id, Delta<MonitoringCommand> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostMonitoringCommand(MonitoringCommand item)
        {
            MonitoringCommand current = await InsertAsync(item);
            ServiceUser serviceUser = (ServiceUser)this.User;
            var message = item.Command + " from " + serviceUser.Id;
            Services.Log.Info(message);
            await this.ToastAsync(message, serviceUser.Id);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteMonitoringCommand(string id)
        {
            return DeleteAsync(id);
        }
    }
}