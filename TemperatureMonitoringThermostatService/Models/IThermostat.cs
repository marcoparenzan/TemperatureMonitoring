using System;
namespace TemperatureMonitoringThermostatService.Models
{
    public interface IThermostat
    {
        void PowerOn();
        void PowerOff();
        void HigherTemp();
        void LowerTemp();

        void InvalidCommand(string command);
    }
}
