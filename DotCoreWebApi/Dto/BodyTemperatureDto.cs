using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotCoreWebApi.Dto
{
    public class BodyTemperatureDto
    {
        public string DocumentId { get; set; }
        public string DateFormatted { get; set; }
        public double Temperature { get; set; }       
        public string UnitOfMessure { get; set; }
        public string Date { get; set; }       
    }
}
