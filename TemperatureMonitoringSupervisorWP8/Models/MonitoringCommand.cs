using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TemperatureMonitoringSupervisorWP8.Models
{
    public class MonitoringCommand
    {
        public string Id { get; set; }
        public string SensorId { get; set; }
        public string Command { get; set; }
    }
}
