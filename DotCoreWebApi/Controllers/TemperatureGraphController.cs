﻿using DotCoreWebApi.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotCoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureGraphController : ControllerBase
    {
        #region Private variables
        private HttpClient restClient = new HttpClient();
        private IHostingEnvironment _env;
        private IConfiguration m_configuration; 
        #endregion

        public TemperatureGraphController(IHostingEnvironment env , IConfiguration iConfiguration)
        {
            _env = env;
            m_configuration = iConfiguration;
        }

        #region Reading Data
        [HttpGet("[action]")]
        public async Task<GraphDataCollection> GetBodyTemperatureAql()
        {
            var PatientId = m_configuration.GetSection("PatientId").Value;

            var aqlSelect = "{\"aql\":\"\\r\\n\\r\\nSELECT tag(o, 'DocumentId') as DocumentId, " +
                                    "\\r\\no /data/events/time/value As Date," +
                                    "\\r\\no /data[at0002]/events[at0003]/data[at0001]/items[at0004]/value/magnitude As Temperature," +
                                    "\\r\\no /data[at0002]/events[at0003]/data[at0001]/items[at0004]/value/units As TemperatureUnits " +
                                    "\\r\\nFROM EHR e CONTAINS OBSERVATION o[openEHR-EHR-OBSERVATION.body_temperature.v2] " +
                                    "\\r\\n\\r\\nwhere e/ehr_status/subject/external_ref/id/value =";

            var aqlTag = "\\r\\n\",\"tagScope\":{\"tags\":[]}}";

            var finalAqlQuery = string.Concat(aqlSelect, "'", PatientId, "'", aqlTag);

            var response = await restClient.PostAsync(m_configuration.GetSection("EhrStoreServerUrl").Value, new StringContent(finalAqlQuery, Encoding.UTF8, "application/json"));

            string postResponse = await response.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<RootObject>(postResponse);

            List<BodyTemperatureDto> bodyTemperatureResult = new List<BodyTemperatureDto>();

            try
            {
                int i = 0;
                foreach (var item in content.rows)
                {
                    bodyTemperatureResult.Add(new BodyTemperatureDto
                    {
                        DocumentId = item[0].ToString(),
                        DateFormatted = item[1].ToString(),
                        Date = item[1].ToString(),
                        Temperature = Convert.ToDouble(item[2].ToString()),
                        UnitOfMessure = item[3].ToString()
                    });

                    i++;
                }
            }
            catch (Exception e)
            {
            }

            List<string> lineChartLabelsList = bodyTemperatureResult.Select(time => time.DateFormatted).ToList();
            var temperatureData = new List<GraphData>
            {
                new GraphData { Label = "Temperature C", Data = bodyTemperatureResult.Select(t => t.Temperature).ToArray() }
            };

            return new GraphDataCollection { WeatherList = temperatureData, ChartLabels = lineChartLabelsList.ToArray() };
        }
        #endregion

        #region PostingData
        [HttpPost("[action]")]
        public async Task<SavedRespone> CreateBodyTemperature(BodyTemperatureDto temperatureModel)
        {
            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Add("Auth-Ticket", m_configuration.GetSection("Auth-Ticket").Value);
            //string requestUrl = "https://az-sea-fl-srv02.dipscloud.com/DIPS-WebAPI/HL7/FHIRDSTU2/Observation?_profile=DIPSVitalSignsObservation";

            try
            {
                var assembly = typeof(DotCoreWebApi.Controllers.TemperatureGraphController).Assembly;
                Stream resource = assembly.GetManifestResourceStream("JsonDataStore._fonts.BodyTemperature_V1.json");

                var webRoot = _env.ContentRootPath;
                var file = System.IO.Path.Combine(webRoot, "JsonDataStore/BodyTemperature_V1.json");

                var JsonData = System.IO.File.ReadAllText(file).Replace("[TEMPERATURE]", temperatureModel.Temperature.ToString());

                var response = await restClient.PostAsync(m_configuration.GetSection("VitalSignCreateUrl").Value, new StringContent(JsonData, Encoding.UTF8, "application/json"));

                string postResponse = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<SavedRespone>(postResponse);
            }
            catch (Exception e)
            {
                return null;
            }
          
        }
        #endregion
    }
}