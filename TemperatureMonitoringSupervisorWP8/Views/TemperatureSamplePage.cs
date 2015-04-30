/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Marco Parenzan (https://it.linkedin.com/in/marcoparenzan)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace TemperatureMonitoringSupervisorWP8.Views
{
    sealed partial class TemperatureSamplePage: Page
    {
        private IMobileServiceTable<TemperatureSample> _sampleTable;
        //private IMobileServiceTable<MonitoringCommand> _monitoringCommandsTable;
        private IMobileServiceSyncTable<MonitoringCommand> _monitoringCommandsSyncTable;

        public TemperatureSamplePage()
        {
            this.InitializeComponent();
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, e) =>
            {
                if (this.Frame != null && this.Frame.CanGoBack && _sourcePageType != null)
                {
                    e.Handled = true;
                    this.Frame.Navigate(_sourcePageType);
                }
            };
        }

        private string _sensorId;
        private Type _sourcePageType;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _sourcePageType = e.SourcePageType;
            _sensorId = e.Parameter.ToString();
            SensorId.Text = _sensorId;
        }

        private async Task InitializeAsync()
        {
            await MobileServices.InitializeAsync();
            if (_sampleTable == null)
            {
                _sampleTable = MobileServices.GetTable<TemperatureSample>();
                //_monitoringCommandsTable = MobileServices.GetTable<MonitoringCommand>();
                _monitoringCommandsSyncTable = await MobileServices.GetSyncTable<MonitoringCommand>();
            }
        }

        private async Task Refresh()
        {
            ButtonRefresh.IsEnabled = false;
            MobileServiceInvalidOperationException exception = null;
            try
            {
                await InitializeAsync();
                await MobileServices.SyncAsync(async () =>
                {
                    await _monitoringCommandsSyncTable.PullAsync("MonitoringCommands", _monitoringCommandsSyncTable.CreateQuery());
                });
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }
            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }

            try
            {
                _temperatureSamples =
                    await _sampleTable
                    .Where(xx => xx.SensorId == _sensorId)
                    .OrderByDescending(xx => xx.CreatedAt)
                    .Take(10)
                    .ToCollectionAsync();
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
                ListItems.ItemsSource = _temperatureSamples;
            }
            ButtonRefresh.IsEnabled = true;
        }




        private MobileServiceCollection<TemperatureSample, TemperatureSample> _temperatureSamples;

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async Task InsertMonitoringCommand(MonitoringCommand monitoringCommand)
        {
            await _monitoringCommandsSyncTable.InsertAsync(monitoringCommand);
            await Refresh(); // offline sync
        }

        private async void MonitoringCommand(string command)
        {
            await InsertMonitoringCommand(new MonitoringCommand { SensorId = _sensorId, Command = command, Id = Guid.NewGuid().ToString() });
        }

        private void PowerOn_Click(object sender, RoutedEventArgs e)
        {
            MonitoringCommand("POWERON");
        }

        private void PowerOff_Click(object sender, RoutedEventArgs e)
        {
            MonitoringCommand("POWEROFF");
        }

        private void HigherTemp_Click(object sender, RoutedEventArgs e)
        {
            MonitoringCommand("HIGHERTEMP");
        }

        private void LowerTemp_Click(object sender, RoutedEventArgs e)
        {
            MonitoringCommand("LOWERTEMP");
        }
    }
}
