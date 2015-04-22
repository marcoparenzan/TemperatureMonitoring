using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using TemperatureMonitoringSupervisorWP8.Models;
using Windows.ApplicationModel.Activation;

namespace TemperatureMonitoringSupervisorWP8.Views
{
    sealed partial class TemperatureSensorPage: Page
    {
        private IMobileServiceTable<TemperatureSensor> _sensorTable;

        private async Task Initialize()
        {
            await MobileServices.InitializeAsync();
            if (_sensorTable == null)
            {
                _sensorTable = MobileServices.GetTable<TemperatureSensor>();
            }
        }

        private MobileServiceCollection<TemperatureSensor, TemperatureSensor> _sensors;

        public TemperatureSensorPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async Task Refresh()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                await Initialize();
                _sensors = await _sensorTable.ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = _sensors;
            }
        }

        private void ListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sensor = (TemperatureSensor) ListItems.SelectedItem;
            if (sensor != null)
            {
                this.Frame.Navigate(typeof(TemperatureSamplePage), sensor.Id);
            }
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ButtonRefresh.IsEnabled = false;

            await Refresh();

            ButtonRefresh.IsEnabled = true;
        }
    }
}
