using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThermostatCore.Domain;

namespace ThermostatWPF.Domain
{
    public class ThermostatSimulator: IThermostat
    {
        public ThermostatSimulator()
        {
        }

        public void Reset()
        {
        }

        public void PowerOn()
        {
        }

        public void PowerOff()
        {
        }

        public void HigherTemp()
        {
        }

        public void LowerTemp()
        {
        }

        public void NotificationHandler(Action<string, string> nh)
        {
        }

        public void Switch()
        {
        }

        public bool On {
            get {
                return false;
            }
        }
    }
}
