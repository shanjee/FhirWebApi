using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DIPS.FHIR.Interface;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotCoreWebApi.Controllers
{

    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        HttpClient restClient = new HttpClient();

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<BodyTemperature> GetBodyTemperature()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new BodyTemperature
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }


        [HttpGet("[action]")]
        public async Task<IEnumerable<BodyTemperature>> GetBloodPressure()
        {

            var bundle = new Bundle { Entry = new List<Bundle.EntryComponent>() };

            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Add("Auth-Ticket", "3fd00df2-02dd-488d-9b58-403f385ccc49");


            var jsonString = await restClient.GetStringAsync("https://az-sea-fl-srv02.dipscloud.com/DIPS-WebAPI/HL7/FHIRDSTU2/Observation/ako1007372?_profile=DIPSVitalSignsObservation");

            JObject jObject = JObject.Parse(jsonString);
            string displayName = (string)jObject.SelectToken("resourceType");

            var systolic = (string)jObject.SelectToken(".entry[0].resource.extension[0].valueQuantity.value"); // systolic
            var diastoloic = (string)jObject.SelectToken("..entry[0].resource.component[0].valueQuantity.value");  // diastolic

            //var data = JsonConvert.DeserializeObject<Observation>(jsonString);            //return data;

            List<BodyTemperature> bodyTemperatures = new List<BodyTemperature>();
            bodyTemperatures.Add(new BodyTemperature { TemperatureC = Convert.ToInt32(systolic) });
            return bodyTemperatures;

        }

        #region AqlQueryRegion

        [HttpGet("[action]")]
        public async Task<List<BloodPressureFhirDto>> GetBloodPressureFromAql()
        {

            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Add("Auth-Ticket", "3fd00df2-02dd-488d-9b58-403f385ccc49");

            var jsonString = await restClient.GetStringAsync("https://az-sea-fl-srv02.dipscloud.com/DIPS-WebAPI/HL7/FHIRDSTU2/Observation/ako1007372?_profile=DIPSVitalSignsObservation");

            string requestUrl = "https://az-sea-fl-srv02.dipscloud.com:4443/api/v1/query";
            string jsonInString = "{\"aql\":\"SELECT tag(o, 'DocumentId') as DocumentId, \\r\\n\\r\\no /data/events/time/value As Date,\\r\\no /data[at0001]/events[at0006]/data[at0003]/items[at0004]/value/magnitude As Systolic, \\r\\no /data[at0001]/events[at0006]/data[at0003]/items[at0004]/value/units As SystolicUnits, \\r\\no /data[at0001]/events[at0006]/data[at0003]/items[at0005]/value/magnitude As Diastolic,\\r\\no /data[at0001]/events[at0006]/data[at0003]/items[at0005]/value/units As DiastolicUnits, \\r\\nFROM EHR e CONTAINS OBSERVATION o[openEHR-EHR-OBSERVATION.blood_pressure.v1]   \\r\\n\\r\\nwhere e/ehr_status/subject/external_ref/id/value = '1000239'\\r\\n\",\"tagScope\":{\"tags\":[]}}";


            var response = await restClient.PostAsync(requestUrl, new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            string postResponse = await response.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<RootObject>(postResponse);

            List<BloodPressureFhirDto> bloodPressureFhirs = new List<BloodPressureFhirDto>();

            foreach (var item in content.rows)
            {
                bloodPressureFhirs.Add(new BloodPressureFhirDto {
                    DocumentId = item[0].ToString(),
                    DateTaken = item[1].ToString(),
                    Systolic = Convert.ToInt64(item[2]),
                    Diastolic = Convert.ToInt64(item[4]),
                    UnitOfMessure = item[5].ToString()
                });
            }

            return bloodPressureFhirs;
        }

        #endregion

        public class BodyTemperature
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }

        #region Dto Related to Aql
        public class BloodPressureFhirDto
        {
            public double Systolic { get; set; }
            public double Diastolic { get; set; }
            public string UnitOfMessure { get; set; }
            public string DateTaken { get; set; }
            public string DocumentId { get; set; }
        }

        public class Meta
        {
            public string _type { get; set; }
            public string _schema_version { get; set; }
            public DateTime _created { get; set; }
            public string _generator { get; set; }
        }

        public class Column
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        public class RootObject
        {
            public string _type { get; set; }
            public Meta meta { get; set; }
            public object name { get; set; }
            public int totalResults { get; set; }
            public List<Column> columns { get; set; }
            public List<List<object>> rows { get; set; }
        }
        #endregion
    }
}
