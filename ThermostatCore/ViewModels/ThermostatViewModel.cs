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

namespace ThermostatCore.ViewModels
{
    public partial class ThermostatViewModel
    {
        private Func<string> _read;
        private Action<string> _write;

        private Queue<string> _commands;

        public ThermostatViewModel(INavigation navigation, Func<string> read, Action<string> write)
        {
            this._read = read;
            this._write = write;
            this._commands = new Queue<string>();

            Task poll = null;
            poll = Task.Run(() =>
            {
                while (true)
                {
                    if (_commands.Count == 0)
                    {
                        _commands.Enqueue("GETSTATE");
                        poll.Wait(100);
                        continue;
                    }
                    else
                    {
                        while (true)
                        {
                            if (_commands.Count == 0) break;
                            var command = _commands.Dequeue();
                            write(command);
                        }
                    }
                    var line = read();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        poll.Wait(100);
                        continue;
                    }
                    else
                    {
                        var response = line.Split('=');
                        switch (response[0].Trim().ToUpper())
                        {
                            case "COMMANDRESULT":
                                break;
                            case "STATE":
                                navigation.Invoke(() =>
                                {
                                    var state = response[1].Trim().ToUpper().Split(';');
                                    Temp = decimal.Parse(state[0]);
                                    TempRef = decimal.Parse(state[1]);
                                    Power = state[2] == "1";
                                    Resistor = state[3] == "1";
                                });
                                break;
                        }
                    }
                }
            });
        }

        partial void OnPowerOff(ThermostatViewModel.PowerOffArgs args)
        {
            _commands.Enqueue("POWEROFF");
        }

        partial void OnPowerOn(ThermostatViewModel.PowerOnArgs args)
        {
            _commands.Enqueue("POWERON");
        }

        partial void OnHigherTemp(ThermostatViewModel.HigherTempArgs args)
        {
            _commands.Enqueue("HIGHERTEMP");
        }

        partial void OnLowerTemp(ThermostatViewModel.LowerTempArgs args)
        {
            _commands.Enqueue("LOWERTEMP");
        }
    }
}
