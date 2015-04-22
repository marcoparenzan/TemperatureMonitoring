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
