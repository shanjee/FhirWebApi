using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotCoreWebApi.Dto
{
    public class AqlResultSetDtocs
    {
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
}
