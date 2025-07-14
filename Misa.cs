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
    }
}
