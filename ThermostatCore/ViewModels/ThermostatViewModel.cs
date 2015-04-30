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
using System.IO;
using System.Threading.Tasks;
using ThermostatCore.Common;
using ThermostatCore.Domain;

namespace ThermostatCore.ViewModels
{
    public partial class ThermostatViewModel
    {
        private INavigation _navigation;
        private IThermostat _thermostat;
        private ISupervisor _supervisor;
        private DateTime _lastIngest;
        private Action _exit1;
        
        public ThermostatViewModel(INavigation navigation, IThermostat thermostat, ISupervisor supervisor, Action exit)
        {
            this._navigation = navigation;
            this._thermostat = thermostat;
            this._supervisor = supervisor;
            this._exit1 = exit;

            thermostat.NotificationHandler(HandleNotificationAsync);
        }

        private async void HandleNotificationAsync(string messageType, string messageValue)
        {
            switch (messageType)
            {
                case "COMMANDRESULT":
                    break;
                case "STATE":
                    _navigation.Invoke(async () =>
                    {
                        try
                        {
                            var state = messageValue.Split(';');
                            Temp = decimal.Parse(state[0].Replace(".", ","));
                            TempRef = decimal.Parse(state[1]);
                            Power = state[2] == "1";
                            Resistor = state[3] == "1";
                            var now = DateTime.Now;
                            if ((now - _lastIngest).TotalSeconds > 5)
                            {
                                _lastIngest = now;
                                await _supervisor.Ingest(Temp, TempRef, Power, Resistor);
                            }
                        }
                        catch
                        {
                            _thermostat.Reset();
                        }
                    });
                    break;
            }
        }

        partial void OnPowerOff(ThermostatViewModel.PowerOffArgs args)
        {
            _thermostat.PowerOff();
        }

        partial void OnPowerOn(ThermostatViewModel.PowerOnArgs args)
        {
            _thermostat.PowerOn();
        }

        partial void OnHigherTemp(ThermostatViewModel.HigherTempArgs args)
        {
            _thermostat.HigherTemp();
        }

        partial void OnLowerTemp(ThermostatViewModel.LowerTempArgs args)
        {
            _thermostat.LowerTemp();
        }

        partial void OnExit(ThermostatViewModel.ExitArgs args)
        {
            _exit1();
        }

        partial void OnReset(ThermostatViewModel.ResetArgs args)
        {
            _thermostat.Reset();
        }
    }
}
