using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TemperatureMonitoringThermostatService.Models
{
    public class IngestMessage
    {
        [JsonProperty("sensorId")]
        public string SensorId { get; set; }
        [JsonProperty("temp")]
        public decimal Temp { get; set; }
        [JsonProperty("reftemp")]
        public decimal RefTemp { get; set; }
        [JsonProperty("power")]
        public bool Power { get; set; }
        [JsonProperty("resistor")]
        public bool Resistor { get; set; }
    }
}