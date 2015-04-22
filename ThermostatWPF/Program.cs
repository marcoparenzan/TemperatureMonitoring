using System;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Windows;
using ThermostatWPF.Views;
using ThermostatCore.Common;
using ThermostatCore.ViewModels;

namespace ThermostatWPF
{
    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            var view = new NavigationWindow();
            view.WindowStyle = WindowStyle.None;
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var navigation = (INavigation)view;

            var serialPortName = ConfigurationManager.AppSettings["ThermostatPort"];
            var serial = new SerialPort(serialPortName, 9600, Parity.None, 8, StopBits.One);
            serial.Open();

            navigation.View("ThermostatView").ViewModel(new ThermostatViewModel(
                navigation
                , () => {
                    if (serial.BytesToRead == 0) return string.Empty;
                    return serial.ReadLine();
                }
                , _ => { serial.WriteLine(_); }
            ));

            var application = new Application();
            application.Run(view);

            serial.Close();
        }
    }
}
