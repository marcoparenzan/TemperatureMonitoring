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
using System.Windows;
using ThermostatWPF.Views;
using ThermostatCore.Common;
using ThermostatCore.ViewModels;
using ThermostatCore.Domain;
using ThermostatWPF.Domain;

namespace ThermostatWPF
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IThermostat thermostat = new SerialPortThermostat();
            ISupervisor supervisor = new RestSupervisor();

            var view = new NavigationWindow();
            view.WindowStyle = WindowStyle.None;
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.WindowState = WindowState.Maximized;
            var navigation = (INavigation)view;

            navigation.View("ThermostatView").ViewModel(new ThermostatViewModel(
                navigation
                , thermostat
                , () => { view.Close(); }
            ));

            if (!thermostat.On) thermostat.Switch();

            var application = new Application();
            application.Run(view);

            if (thermostat.On) thermostat.Switch();
        }
    }
}
