using Microsoft.WindowsAzure.Mobile.Service;
namespace TemperatureMonitoringMobileService.Models
{
    public class MonitoringCommand : EntityData
    {
        public string Command { get; set; }
        public string SensorId { get; set; }

    }
}