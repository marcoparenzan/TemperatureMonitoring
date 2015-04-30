using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemperatureMonitoringThermostatService.Models
{
    public class CommandMessage
    {
        public string SensorId { get; set; }
        public string Command { get; set; }
    }
}
