using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThermostatCore.Thermostat
{
    public interface IThermostat
    {
        decimal MaxWeight { get; }
        decimal MinWeight { get; }

        void Start();
        bool Stable { get; }
        decimal Current { get; }
        void Reset();
        void Stop();

        event EventHandler Value;

        void Suspend();

        void Resume();
    }
}
