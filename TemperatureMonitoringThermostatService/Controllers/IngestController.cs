using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TemperatureMonitoringThermostatService.Models;

namespace TemperatureMonitoringThermostatService.Controllers
{
    [Authorize]
    public class IngestController : ApiController
    {
        [AllowAnonymous]
        public void Post([FromBody] IngestMessage message)
        {
            try
            {
                var dataContext = new TemperatureMonitoringDataContext();
                dataContext.TemperatureSamples.Add(new TemperatureSample
                {
                    Id = Guid.NewGuid().ToString()
                    ,
                    CreatedAt = DateTime.UtcNow
                    ,
                    Version = Encoding.UTF8.GetBytes("ABCD")
                    ,
                    Deleted = false
                    ,
                    SensorId = message.SensorId
                    ,
                    Value = message.Temp
                });
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw ex;
            }
        }
    }
}
