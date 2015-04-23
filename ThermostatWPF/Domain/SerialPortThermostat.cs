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
using ThermostatCore.Domain;

namespace ThermostatWPF.Domain
{
    public class SerialPortThermostat: IThermostat
    {
        private string _serialPortName;
        
        public SerialPortThermostat()
        {
            _serialPortName = ConfigurationManager.AppSettings["ThermostatPort"];
        }

        public SerialPortThermostat(string port)
        {
            _serialPortName = port;
        }

        private SerialPort _serialPort;

        public void Reset()
        {
            _serialPort.DtrEnable = true;
            Thread.Sleep(100);
            _serialPort.DtrEnable = false;
        }

        public void NotificationHandler(Action<string, string> nh)
        {
            _nh = nh;
        }

        public bool On
        {
            get
            {
                return _serialPort != null;
            }
        }

        public void Switch()
        {
            if (_serialPort == null)
            {
                _serialPort = new SerialPort(_serialPortName, 9600, Parity.None, 8, StopBits.One);
                _serialPort.DataReceived += NotificationHandlerImpl;
                _serialPort.Open();
            }
            else
            {
                _serialPort.DataReceived -= NotificationHandlerImpl;
                PowerOff();
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                Reset();
                _serialPort.Close();
                _serialPort = null;
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
        private void NotificationHandlerImpl(object sender, SerialDataReceivedEventArgs e)
        {
            try
            { 
                var line = _serialPort.ReadLine();
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

        private void Write(string text)
        {
            _serialPort.Write(text + "\n");
        }

        #endregion
    }
}
