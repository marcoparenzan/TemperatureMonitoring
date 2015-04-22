using Microsoft.WindowsAzure.Mobile.Service;

namespace TemperatureMonitoringMobileService.Models
{
    public class TemperatureSample : EntityData
    {
        public string SensorId { get; set; }
        public decimal Value { get; set; }
    }
}