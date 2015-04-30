using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TemperatureMonitoringThermostatService.Models;

namespace TemperatureMonitoringThermostatService.SignalR
{
    // http://www.asp.net/signalr/overview/guide-to-the-api/hubs-api-guide-net-client
    public class ThermostatCommandsHub: Hub
    { 
        private readonly static IHubContext<IThermostat> __context;
        private readonly static ThermostatHubMapping __connections;

        static ThermostatCommandsHub()
        {
            __context = GlobalHost.ConnectionManager.GetHubContext<IThermostat>("ThermostatCommandsHub");
            __connections = new ThermostatHubMapping();
        }

        #region Connections

        public override Task OnConnected()
        {
            //string name = Context.User.Identity.Name;

            //__connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            //string name = Context.User.Identity.Name;

            //__connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            //string name = Context.User.Identity.Name;

            //if (!__connections.GetConnections(name).Contains(Context.ConnectionId))
            //{
            //    __connections.Add(name, Context.ConnectionId);
            //}

            return base.OnReconnected();
        }

        #endregion

        // to clients

        private static void Sensor(string sensorId, Action<IThermostat> action)
        {
            foreach (var connectionId in __connections.GetConnections(sensorId))
            {
                action(__context.Clients.Client(connectionId));
            }
        }

        public static void PowerOn(string sensorId)
        {
            Sensor(sensorId, _ => { _.PowerOn(); });
        }

        public static void PowerOff(string sensorId)
        {
            Sensor(sensorId, _ => { _.PowerOff(); });
        }

        public static void HigherTemp(string sensorId)
        {
            Sensor(sensorId, _ => { _.HigherTemp(); });
        }

        public static void LowerTemp(string sensorId)
        {
            Sensor(sensorId, _ => { _.LowerTemp(); });
        }

        public static void InvalidCommand(string sensorId, string command)
        {
            Sensor(sensorId, _ => { _.InvalidCommand(command); });
        }

        // from client

        public void RegisterSensor(string sensorId)
        {
            __connections.Add(sensorId, Context.ConnectionId);
        }

        public void UnregisterSensor(string sensorId)
        {
            __connections.Remove(sensorId, Context.ConnectionId);
        }
    }
}