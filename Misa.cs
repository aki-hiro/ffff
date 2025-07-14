using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Misa : CsvReadBase
    {
        [Index(0)]
        public string KaiCode { get; set; }

        [Index(4)]
        public string DenNo { get; set; }

        [Index(5)]
        public string DenId { get; set; }

        [Index(9)]
        public string RecordKbn { get; set; }
    }
}
