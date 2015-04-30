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
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ThermostatCore.Domain;

namespace ThermostatWPF.Domain
{
    public class ThermostatSimulator : IThermostat
    {
        public ThermostatSimulator()
        {
        }

        private DispatcherTimer _timer;

        public void Reset()
        {
        }

        public void NotificationHandler(Action<string, string> nh)
        {
            _nh = nh;
        }

        public bool On
        {
            get
            {
                return _timer != null;
            }
        }

        public void Switch()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                _timer.Tick += NotificationHandlerImpl;
            }
            else
            {
                PowerOff();
                Reset();
                _timer = null;
            }
        }

        public void PowerOn()
        {
            Write("POWERON");
        }

        public void PowerOff()
        {
            Write("POWEROFF");
        }

        public void HigherTemp()
        {
            Write("HIGHERTEMP");
        }

        public void LowerTemp()
        {
            Write("LOWERTEMP");
        }

        #region Private

        private Action<string, string> _nh;
        private void NotificationHandlerImpl(object sender, EventArgs e)
        {
            try
            {
                Write("GETSTATE");

                if (_queue.Count() == 0) return;
                var line = _queue.Dequeue();
                line = line.Trim('\r', '\n', '\r', ' ');
                if (string.IsNullOrWhiteSpace(line)) return;
                var response = line.Split('=');
                _nh(response[0].Trim().ToUpper(), response[1].Trim().ToUpper());
            }
            catch
            {
                Reset();
            }
        }

        private Queue<string> _queue = new Queue<string>();
        private decimal tempRef = 60;
        private int on = 0;

        private void Write(string text)
        {
            switch (text)
            {
                case "GETSTATE":
                    _queue.Enqueue(string.Format("STATE={0};{1};{2};{3}\r\n", tempRef, tempRef, on, on));
                    break;
                case "POWERON":
                    on = 1;
                    _timer.Start();
                    _queue.Enqueue("COMMANDRESULT=OK");
                    break;
                case "POWEROFF":
                    on = 0;
                    _timer.Stop();
                    _queue.Enqueue("COMMANDRESULT=OK");
                    break;
                case "HIGHERTEMP":
                    tempRef++;
                    _queue.Enqueue("COMMANDRESULT=OK");
                    break;
                case "LOWERTEMP":
                    tempRef--;
                    _queue.Enqueue("COMMANDRESULT=OK");
                    break;
            }
            _queue.Enqueue(text);
        }

        #endregion
    }
}
