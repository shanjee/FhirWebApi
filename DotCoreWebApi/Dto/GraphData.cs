using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotCoreWebApi.Dto
{
    public class GraphData
    {
        public double[] Data { get; set; }
        public string Label { get; set; }
    }

    public class GraphDataCollection
    {
        public List<GraphData> WeatherList { get; set; }
        public string[] ChartLabels { get; set; }
    }
}
