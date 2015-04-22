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
