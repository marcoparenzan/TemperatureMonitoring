using Microsoft.Owin;
using Owin;

namespace TemperatureMonitoringThermostatService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}