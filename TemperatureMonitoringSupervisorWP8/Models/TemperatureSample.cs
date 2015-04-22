using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TemperatureMonitoringSupervisorWP8.Models
{
    public class TemperatureSample
    {
        public string Id { get; set; }
        public string SensorId { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
