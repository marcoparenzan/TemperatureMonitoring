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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThermostatCore.Domain;

namespace ThermostatWPF.Domain
{
    public class RestSupervisor: ISupervisor
    {
        private string _sensorId;
        private string _ingestUrl;

        public RestSupervisor()
        {
            _sensorId = ConfigurationManager.AppSettings["SensorId"];
            _ingestUrl = ConfigurationManager.AppSettings["IngestUrl"];
        }

        public RestSupervisor(string sensorId, string supervisorRestUrl)
        {
            _sensorId = sensorId;
            _ingestUrl = supervisorRestUrl;
        }

        public async Task Ingest(decimal temp, decimal refTemp, bool power, bool resistor)
        {
            var client = new HttpClient();
            var message = new {
                sensorId = _sensorId
                ,
                temp
                ,
                refTemp
                ,
                power
                ,
                resistor
            };
            var json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(_ingestUrl, content);
        }
    }
}
