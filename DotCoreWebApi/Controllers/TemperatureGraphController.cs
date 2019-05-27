using DotCoreWebApi.Dto;
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
using System.Threading;
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

        public TemperatureGraphController(IHostingEnvironment env, IConfiguration iConfiguration)
        {
            _env = env;
            m_configuration = iConfiguration;
        }

        #region Reading Data
        [HttpGet("[action]")]
        public async Task<GraphDataCollection> GetBodyTemperatureAql(string startdate, string enddate)
        {
            var PatientId = m_configuration.GetSection("PatientId").Value;
            var finalAqlQuery = "";

            var aqlSelect = "{\"aql\":\"\\r\\n\\r\\nSELECT tag(o, 'DocumentId') as DocumentId, " +
                                    "\\r\\no /data/events/time/value As Date," +
                                    "\\r\\no /data[at0002]/events[at0003]/data[at0001]/items[at0004]/value/magnitude As Temperature," +
                                    "\\r\\no /data[at0002]/events[at0003]/data[at0001]/items[at0004]/value/units As TemperatureUnits " +
                                    "\\r\\nFROM EHR e CONTAINS OBSERVATION o[openEHR-EHR-OBSERVATION.body_temperature.v2] " +
                                    "\\r\\n\\r\\nwhere e/ehr_status/subject/external_ref/id/value =";

            var aqlTag = "\\r\\n\",\"tagScope\":{\"tags\":[]}}";

            if (!string.IsNullOrEmpty(startdate) && (!string.IsNullOrEmpty(enddate)))
            {
                string dateFilter = $"and o /data/events/time/value > '{startdate}' and o /data/events/time/value < '{enddate}'";
                finalAqlQuery = string.Concat(aqlSelect, "'", PatientId, "'", dateFilter, aqlTag);
            }
            else
            {
                if (m_configuration.GetSection("CanFilterByDate").Value.ToLower() == "true")
                {
                    finalAqlQuery = string.Concat(aqlSelect, "'", PatientId, "'", m_configuration.GetSection("DateRangeFilter").Value, aqlTag);
                }
                else
                {
                    finalAqlQuery = string.Concat(aqlSelect, "'", PatientId, "'", aqlTag);
                }
            }

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

            List<string> lineChartFullLabelsList = new List<string>();

            var result = bodyTemperatureResult.Aggregate(new BodyTemperatureDto(),
                                    (r, f) =>
                                    {
                                        //r.DocumentId =  string.Join(" - ", f.DateFormatted, string.Format("{0}°C", f.Temperature));
                                        r.DocumentId = string.Join(" - ", f.DateFormatted);
                                        lineChartFullLabelsList.Add(r.DocumentId);
                                        return r;
                                    });


            var temperatureData = new List<GraphData>
            {
                new GraphData { Label = "Temperature °C", Data = bodyTemperatureResult.Select(t => t.Temperature).ToArray() }
            };

            //return new GraphDataCollection { WeatherList = temperatureData, ChartLabels = lineChartLabelsList.ToArray() };
            return new GraphDataCollection { WeatherList = temperatureData, ChartLabels = lineChartFullLabelsList.ToArray() };
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

                SavedRespone savedRespone = JsonConvert.DeserializeObject<SavedRespone>(postResponse);
                savedRespone.comments = $"New Observation {savedRespone.id} has been created";
                return savedRespone;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        [HttpGet("[action]")]
        public async Task<SavedRespone> GenerateTemperatureAsAuto()
        {
            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Add("Auth-Ticket", m_configuration.GetSection("Auth-Ticket").Value);
            string postResponse = string.Empty;

            try
            {
                var assembly = typeof(DotCoreWebApi.Controllers.TemperatureGraphController).Assembly;
                Stream resource = assembly.GetManifestResourceStream("JsonDataStore._fonts.BodyTemperature_V1.json");

                var webRoot = _env.ContentRootPath;
                var file = System.IO.Path.Combine(webRoot, "JsonDataStore/BodyTemperature_V1.json");

                for (int i = 0; i < 5; i++)
                {
                    double temperatureGenerated = GetRandomTemperature(36.5, 41.2);
                    var JsonData = System.IO.File.ReadAllText(file).Replace("[TEMPERATURE]", String.Format("{0:0.00}", temperatureGenerated));

                    var response = await restClient.PostAsync(m_configuration.GetSection("VitalSignCreateUrl").Value, new StringContent(JsonData, Encoding.UTF8, "application/json"));
                    postResponse = await response.Content.ReadAsStringAsync();
                    Thread.Sleep(5000);
                }

                SavedRespone savedRespone = JsonConvert.DeserializeObject<SavedRespone>(postResponse);
                savedRespone.comments = $"New Observation {savedRespone.id} has been created";
                return savedRespone;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        // Generate a random number between two numbers
        private double GetRandomTemperature(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        #endregion
    }
}