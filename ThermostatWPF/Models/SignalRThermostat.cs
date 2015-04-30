using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatWPF.Models
{
    public class SignalRThermostat
    {
        private string _sensorId;
        private string _hubUrl;

        public string HubUrl
        {
            get { return _hubUrl; }
        }

        private HubConnection _hubConnection;
        private IHubProxy _hubProxy;

        public SignalRThermostat(string sensorId)
        {
            _sensorId = sensorId;
            _hubUrl = ConfigurationManager.AppSettings["endPoint"];
            _hubConnection = new HubConnection(_hubUrl + "signalr", false);
            _hubProxy = _hubConnection.CreateHubProxy("ThermostatCommandsHub");
            _hubProxy.On("PowerOn", () => { OnNotify("PowerOn"); });
            _hubProxy.On("PowerOff", () => { OnNotify("PowerOff"); });
            _hubProxy.On("HigherTemp", () => { OnNotify("HigherTemp"); });
            _hubProxy.On("LowerTemp", () => { OnNotify("LowerTemp"); });
            _hubProxy.On<string>("InvalidCommand", command => { OnNotify("InvalidCommand: " + command); });
            _hubConnection.Start().ContinueWith(_ => {
                _hubProxy.Invoke("RegisterSensor", ConfigurationManager.AppSettings["SensorId"]);
            });
        }

        private void OnNotify(string message)
        {
            if (Notify != null) Notify(this, message);
        }

        public event EventHandler<string> Notify;

        public void Unregister()
        { 
            _hubProxy.Invoke("UnregisterSensor", _sensorId);
        }
    }
}
