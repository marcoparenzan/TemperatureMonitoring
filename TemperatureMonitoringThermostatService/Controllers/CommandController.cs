using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TemperatureMonitoringThermostatService.Models;
using TemperatureMonitoringThermostatService.SignalR;

namespace TemperatureMonitoringThermostatService.Controllers
{
    public class CommandController : ApiController
    {
        public void Post([FromBody]CommandMessage message)
        {
            switch (message.Command)
            { 
                case "PowerOn":
                    ThermostatCommandsHub.PowerOn(message.SensorId);
                    break;
                case "PowerOff":
                    ThermostatCommandsHub.PowerOff(message.SensorId);
                    break;
                case "HigherTemp":
                    ThermostatCommandsHub.HigherTemp(message.SensorId);
                    break;
                case "LowerTemp":
                    ThermostatCommandsHub.LowerTemp(message.SensorId);
                    break;
                default:
                    ThermostatCommandsHub.InvalidCommand(message.SensorId, message.Command);
                    break;
            }
        }
    }
}
