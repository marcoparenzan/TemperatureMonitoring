using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.Models;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SignalRThermostat _thermostat;

        public MainWindow()
        {
            InitializeComponent();
            _thermostat = new SignalRThermostat(ConfigurationManager.AppSettings["SensorId"]);
            _thermostat.Notify += (s, e) => {

                MessageBox.Show(e);
            
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _thermostat.Unregister();
        }

        private void Message(string message)
        {
            Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message);
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Loopback("PowerOn");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Loopback("PowerOff");
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await Loopback("HigherTemp");
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            await Loopback("LowerTemp");
        }

        public async Task Loopback(string command)
        {
            await Command("loopback", ConfigurationManager.AppSettings["SensorId"], command);
        }

        private async Task Command(string api, string sensorId, string command)
        {
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                command
                ,
                sensorId
            }));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.PostAsync(_thermostat.HubUrl + "api/" + api, content).Wait();
        }
    }
}
